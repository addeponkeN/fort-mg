#!/usr/bin/env node
/*
 * Reasonix PreToolUse path guard for the Fort.MG / Fort sibling repositories.
 *
 * Policy (mirrored in AGENTS.md):
 *   - read + write are allowed only inside the related local repositories
 *   - every other path is denied, including %TEMP% / %TMP% / /tmp
 *   - agent scratch files go to <repo>/_temp/ (gitignored), never %TEMP%
 *
 * Contract (see the Hooks section of the Reasonix docs): Reasonix writes ONE
 * LINE OF JSON to this command's stdin for PreToolUse:
 *   {"event":"PreToolUse","cwd":"<workspace>","toolName":"bash","toolArgs":{...}}
 * exit 0 = allow, exit 2 = block, and stderr is fed back to the model.
 *
 * Honest limitation: this is a tool-argument guard, NOT a sandbox. It only sees
 * paths that appear in tool arguments, so it cannot observe what a shell child
 * process opens by other means (environment variables, globs, scripts it runs).
 * Its job is to keep the agent inside the related repositories and to redirect
 * scratch files to _temp/ — not to stop an adversary.
 *
 * Escape hatch: set REASONIX_PATH_GUARD=off to disable.
 */

"use strict";

const fs = require("fs");
const os = require("os");
const path = require("path");

if (String(process.env.REASONIX_PATH_GUARD || "").toLowerCase() === "off") {
  process.exit(0);
}

/* ---------------------------------------------------------------- policy -- */

// This file lives at <repo>/.reasonix/hooks/restrict-paths.js
const WORKSPACE = path.resolve(__dirname, "..", "..");

// Related local repositories that may be read AND written. Kept as folder names
// (not absolute paths) so the identical file can live in both repositories.
const SIBLING_REPOS = ["Fort", "Fort.MG"];

// Read-only roots: toolchain locations a legitimate build/restore needs, but
// which the agent must never modify. Extend this list if a build needs more.
const READ_ONLY_ROOTS = [
  process.env.ProgramFiles && path.join(process.env.ProgramFiles, "dotnet"),
  process.env.ProgramFiles && path.join(process.env.ProgramFiles, "Git"),
  path.join(os.homedir(), ".nuget", "packages"),
  path.join(os.homedir(), ".dotnet"),
  path.join(os.homedir(), ".gitconfig"),
  path.join(os.homedir(), ".config", "git"),
  // Fort.MG.sln references ..\..\Clones\Nopipeline\Nopipeline\... (build only)
  path.resolve(WORKSPACE, "..", "..", "Clones", "Nopipeline"),
];

const TEMP_ROOTS = unique([os.tmpdir(), process.env.TEMP, process.env.TMP]);
const SCRATCH = path.join(WORKSPACE, "_temp");

/* --------------------------------------------------------------- helpers -- */

function unique(list) {
  return [...new Set(list.filter(Boolean))];
}

function norm(p) {
  const resolved = path.resolve(p);
  return process.platform === "win32" ? resolved.toLowerCase() : resolved;
}

function isInside(candidate, root) {
  const c = norm(candidate);
  const r = norm(root);
  return c === r || c.startsWith(r.endsWith(path.sep) ? r : r + path.sep);
}

function insideAny(candidate, roots) {
  return roots.some((root) => isInside(candidate, root));
}

function exists(p) {
  try {
    return fs.existsSync(p);
  } catch {
    return false;
  }
}

// Best-effort canonicalisation so symlinked/junctioned paths cannot slip past
// the root check. Falls back to the input when nothing can be resolved.
function realish(p) {
  try {
    return fs.realpathSync.native(p);
  } catch {
    /* path may not exist yet */
  }
  try {
    let dir = p;
    let suffix = "";
    while (!fs.existsSync(dir)) {
      suffix = path.join(path.basename(dir), suffix);
      const parent = path.dirname(dir);
      if (parent === dir) return p;
      dir = parent;
    }
    return path.join(fs.realpathSync.native(dir), suffix);
  } catch {
    return p;
  }
}

/* ------------------------------------------------------------ extraction -- */

// Tool-argument keys whose *values* are payload text, not paths. Scanning them
// would reject legitimate writes whose code/content merely mentions a path.
const SKIP_KEYS = /^(content|text|new_string|old_string|new_text|old_text|body|snippet|code|prompt|task|message|description|summary|title|query|pattern|regex|replacement)$/i;

const SHELL_WRITE_RE = /(^|[\s;&|()])(>>?|tee\b|mkdir\b|rmdir\b|rm\b|del\b|erase\b|rd\b|mv\b|move\b|cp\b|copy\b|xcopy\b|robocopy\b|touch\b|truncate\b|dd\b|chmod\b|chown\b|attrib\b|ren\b|rename\b|ln\b|sed\s+-i|perl\s+-i|patch\b|git\s+(add|commit|rm|mv|clean|checkout|restore|reset|stash|apply|init|clone|switch)\b|npm\s+(i|install|ci)\b|dotnet\s+(build|publish|pack|test|restore|clean|new)\b|msbuild\b|New-Item|Set-Content|Add-Content|Out-File|Remove-Item|Move-Item|Copy-Item|New-ItemProperty)/i;

const TOOL_WRITE_RE = /(write|edit|create|delete|remove|move|copy|rename|patch|apply|mkdir|touch|insert|replace)/i;
const TOOL_READ_RE = /(read|view|list|glob|grep|search|find|ls|stat|head|tail|open|image)/i;

const TOKEN_RE = /"[^"]*"|'[^']*'|\S+/g;
const DEV_NULL_RE = /^\/(dev\/(null|stdin|stdout|stderr))$/i;

// Map a token to a checkable absolute path, or null when it is not a path (or
// is a device we deliberately ignore). Relative paths with no separator cannot
// escape the working directory, so they are ignored as well.
function expandToken(token) {
  const temp = TEMP_ROOTS[0] || os.tmpdir();
  let m;

  if (DEV_NULL_RE.test(token)) return null;

  if ((m = /^%(TEMP|TMP|TMPDIR)%(.*)$/i.exec(token))) {
    return path.join(temp, m[2].replace(/^[\\/]/, ""));
  }
  if ((m = /^\$(?:env:)?(TEMP|TMP)\b[\\/]?(.*)$/i.exec(token))) {
    return path.join(temp, m[2].replace(/^[\\/]/, ""));
  }
  if (/^~($|[\\/])/.test(token)) return path.join(os.homedir(), token.slice(1));

  // git-bash style mounts: /c/Users/... -> C:\Users\..., /tmp -> %TEMP%
  if ((m = /^\/([a-zA-Z])(?=\/|$)(.*)$/.exec(token))) {
    const rest = m[2].replace(/^[\\/]/, "").replace(/\//g, path.sep);
    return m[1].toUpperCase() + ":" + path.sep + rest;
  }
  if ((m = /^\/(?:var\/)?tmp(?:\/(.*))?$/.exec(token))) {
    return path.join(temp, m[1] || "");
  }

  const absolute = path.isAbsolute(token) || /^\\\\/.test(token);
  const escaping = /(^|[\\/])\.\.([\\/]|$)/.test(token);
  const pathish = /[\\/]/.test(token);
  if (!absolute && !escaping && !pathish) return null;
  return token;
}

function tokensOf(value, out) {
  for (const raw of value.match(TOKEN_RE) || []) {
    let token = raw
      .replace(/^["'([{]+/, "")
      .replace(/["')\]}]+$/, "")
      .replace(/[,;:]+$/, "");
    if (!token || token.startsWith("-") || token.includes("://")) continue;
    const expanded = expandToken(token);
    if (expanded) out.push(expanded);
  }
}

function collectStrings(value, key, out) {
  if (typeof value === "string") {
    if (!SKIP_KEYS.test(key || "")) out.push(value);
    return;
  }
  if (Array.isArray(value)) {
    for (const item of value) collectStrings(item, key, out);
    return;
  }
  if (value && typeof value === "object") {
    for (const [k, v] of Object.entries(value)) collectStrings(v, k, out);
  }
}

/* -------------------------------------------------------------- verdicts -- */

const WRITE_ROOTS = unique([WORKSPACE, ...SIBLING_REPOS.map((name) => path.resolve(WORKSPACE, "..", name))]).filter(
  (root) => root === WORKSPACE || exists(root),
);

function deny(target, mode, reason) {
  const lines = [`reasonix path guard: blocked ${mode} of ${target}`, ""];
  if (reason === "temp") {
    lines.push("reason: %TEMP% / %TMP% is off limits for this workspace.");
    lines.push(`instead: put scratch files in ${SCRATCH} (gitignored), e.g. ${path.join(SCRATCH, "probe.js")}`);
  } else if (reason === "readonly") {
    lines.push("reason: read-only toolchain path (builds may read it, the agent may not write it).");
  } else {
    lines.push("reason: the path is outside the related local repositories.");
  }
  lines.push("");
  lines.push(`read+write allowed: ${WRITE_ROOTS.join(", ")}`);
  if (READ_ONLY_ROOTS.length) lines.push(`read-only allowed: ${READ_ONLY_ROOTS.join(", ")}`);
  lines.push("Do not work around this: use the repositories above, or ask the user.");
  process.stderr.write(lines.join("\n") + "\n");
  process.exit(2);
}

function judge(target, mode) {
  const p = realish(target);
  if (insideAny(p, TEMP_ROOTS)) deny(p, mode, "temp");
  if (insideAny(p, WRITE_ROOTS)) return;
  if (insideAny(p, READ_ONLY_ROOTS)) {
    if (mode === "write") deny(p, mode, "readonly");
    return;
  }
  deny(p, mode, "outside");
}

/* ------------------------------------------------------------------ main -- */

let payload;
try {
  payload = JSON.parse(fs.readFileSync(0, "utf8"));
} catch {
  process.exit(0); // never block on our own malformed input
}

if (!payload || payload.event !== "PreToolUse") process.exit(0);

const toolName = String(payload.toolName || "");
const toolArgs = payload.toolArgs && typeof payload.toolArgs === "object" ? payload.toolArgs : {};
const cwd = typeof payload.cwd === "string" && payload.cwd ? payload.cwd : process.cwd();

const command = typeof toolArgs.command === "string" ? toolArgs.command : "";
const mode = command
  ? SHELL_WRITE_RE.test(command)
    ? "write"
    : "read"
  : TOOL_WRITE_RE.test(toolName)
    ? "write"
    : TOOL_READ_RE.test(toolName)
      ? "read"
      : "read";

const raw = [];
collectStrings(toolArgs, "", raw);
const candidates = [];
for (const value of raw) tokensOf(value, candidates);

for (const candidate of candidates) {
  judge(path.resolve(cwd, candidate), mode);
}

process.exit(0);
