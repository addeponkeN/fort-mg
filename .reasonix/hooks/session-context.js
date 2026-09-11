#!/usr/bin/env node
/*
 * Reasonix SessionStart hook: inject the repository path policy into the next
 * model request as one-shot hook context (documented `hookSpecificOutput`
 * shape). Keep it short — the durable version lives in AGENTS.md.
 */

"use strict";

const path = require("path");

const workspace = path.resolve(__dirname, "..", "..");

const additionalContext = [
  "Repository path policy (enforced by .reasonix/hooks/restrict-paths.js):",
  "- Read and write only inside the related local repositories (Fort.MG and its sibling Fort).",
  "- Never use %TEMP% / %TMP% / /tmp. Scratch files, probes and generated output go to",
  `  <repo>/_temp/ (gitignored), e.g. ${path.join(workspace, "_temp", "probe.js")}.`,
  "- Keep scratch out of the repositories' tracked files, and do not try to bypass the guard.",
].join("\n");

process.stdout.write(
  JSON.stringify({
    hookSpecificOutput: {
      hookEventName: "SessionStart",
      additionalContext,
    },
  }) + "\n",
);
