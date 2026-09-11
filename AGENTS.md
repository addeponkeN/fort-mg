# AGENTS.md — Fort.MG workspace

Agent instructions for this workspace. The durable project documentation is
`AI_CONTEXT.md`; this file covers **where an agent may read and write**.

## Repositories in scope

| Repository | Path | Access |
| --- | --- | --- |
| `Fort.MG` (this workspace) | `X:\dev\Projects\Fort.MG` | read + write |
| `Fort` (sibling, engine-agnostic libraries) | `X:\dev\Projects\Fort` | read + write |
| `Nopipeline` (build-time dependency of `Fort.MG.sln`) | `X:\dev\Clones\Nopipeline` | read-only |
| .NET SDK, Git, NuGet cache (`~/.nuget/packages`), git config | toolchain locations | read-only |

`Fort.MG` builds only with `Fort` checked out as a sibling folder — see
`AI_CONTEXT.md` §1.

## Path policy (enforced)

`.reasonix/settings.json` wires a blocking `PreToolUse` hook,
`.reasonix/hooks/restrict-paths.js`, which **denies reads and writes outside the
in-scope repositories** listed above:

- inside `Fort.MG` or `Fort` → allowed (read + write)
- read-only locations (toolchain, `Nopipeline`) → reads allowed, writes blocked
- anything else, **including `%TEMP%` / `%TMP%` / `/tmp`** → blocked (exit 2, the
  reason is fed back to the model)

The guard inspects paths that appear in tool arguments (structured path args and
command text). It is a policy guard, **not a sandbox**, and it cannot see what a
child process opens by other means. Do not try to work around a denial: stay
inside the repositories, or ask the user. Disable temporarily with
`REASONIX_PATH_GUARD=off` if the guard itself is wrong.

## Scratch files: use `_temp/`, never `%TEMP%`

Every repository has a gitignored `_temp/` folder for agent scratch work:

- build/verify probes, throwaway scripts, generated output, log dumps, payload
  captures → `<repo>/_temp/…` (e.g. `Fort.MG/_temp/probe.js`)
- never `%TEMP%`, `%TMP%`, `/tmp`, or a tracked path in the repository
- delete scratch you no longer need; nothing in `_temp/` is committed anyway

## Hook maintenance

- Hooks are read when a session is constructed — **restart Reasonix** after
  editing `.reasonix/settings.json`.
- Inspect loaded hooks with `reasonix hook list --json` / `reasonix hook status --json`.
- `SessionStart` hook `.reasonix/hooks/session-context.js` re-injects this policy
  into each new session.
- Verification harnesses live in `_temp/` (gitignored): `node _temp/guard-tests.js`
  (guard exit codes) and `node _temp/verify-config.js` (hook wiring).
- The same files are mirrored into the `Fort` repository so the policy also
  applies when `Fort` is opened as its own workspace.
