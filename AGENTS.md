# WhoHolds Agent Guide

---

Use the specified SDK version in [`global.json`](global.json). Solution uses central package management: [`Directory.Packages.props`](Directory.Packages.props).

## Working in this repository

- Release targets Native AOT, because of this: reflection is not used in source-code. Reflection may be used in tests.
- Choose the closest topic from [References](#references) or the targeted website containing the documents and fetch that document before choosing APIs or commands.
- Local/CLI Agents only: you may find that a feature requires Windows but the host machine has Linux. In this case a VM with Windows Server is set up with the required tools and is accessible with the `ssh winvm` command or scripts inside [`windows-server-vm`](scripts/windows-server-vm).
- After any change, check the References table whether the touched topic requires documentation update. Only the documents in the table that are marked _Yes_ under the _Keep updated_ column are required to be kept up-to-date.

## Validation

- Run tests relevant to the change before pushing code.

## Git workflow

- When asked to open an issue do not create a branch or push changes - open the issue.
- Do not subscribe to the PR or do scheduled check ins unless you're explicitly asked by the user.
- Use conventional naming when it comes to commits and branches; only add a commit body for non trivial cases.
- This is not a must, but try to keep commits atomic (e.g. no mixing of feat, fix, chore changes inside commits) - this improves readability of your feature branch.
- Never use the Claude Code Web default branch: `claude/`, create a new branch for your changes.
- Use a feature branch and a ready-for-review PR by default. Create drafts only when requested.
- Trivial documentation and typo changes may go directly into main.

---

## References

| Topic                          | Description                         | Document                                                      | Keep updated |
|--------------------------------|-------------------------------------|---------------------------------------------------------------|--------------|
| Architecture                   | High-level application architecture | [`architecture.md`](./docs/architecture.md)                        | Yes |
| Publish                        | Publishing / Native AOT target      | [`publish.md`](./docs/publish.md)                             | Yes |
| WhoHolds.Core project structure | Core project structure             | [`core-project-structure.md`](docs/core-project-structure.md) | Yes |
| Testing | Testing with TUnit    | [`TUnit Skill`](.claude/skills/tunit/SKILL.md)                | No |

## Commands
- Use `-c Release` with `dotnet` commands

### Working on Windows features on Linux host:

- Access the Windows Server VM from Linux: `ssh winvm`
- Push your changes to the Windows Server VM: [`push-win.sh`](./scripts/windows-server-vm/push-win.sh)