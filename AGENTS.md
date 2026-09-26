# WhoHolds Agent Guide

WhoHolds is a CLI application built with .NET for resolving holder processes to a specified file.

---

## Working with this repository

- Check the [Directory.Build.props](Directory.Build.props) for package versions, target frameworks, and the SDK/test runner in [`global.json`](global.json). Preserve the project's versions unless an upgrade is requested or necessary and explained.
- Documents that are not listed in the table under [References](#references) should not be read or changed by AI Agents.
- Release targets Native AOT, because of this: reflection is not used in source-code. Reflection may be used in tests.
- Choose the closest topic from [References](#references) or the targeted website containing the documents and fetch that document before choosing APIs or commands. Start with one page; fetch another only when the task needs it. Skip pages already available in the conversation unless they need refreshing. 
- You may find that a feature requires Windows but the host machine has Linux. In this case a VM with Windows Server is set up with the required tools and is accessible with the `ssh winvm` command or scripts inside [`windows-server-vm`](scripts/windows-server-vm).
- Run focused build or test when making code changes.
- After you finish your changes, check the References table if the related topic to your changes requires its document to be changed - in this case update the document. **Only update documents that are marked with _Yes_ in the 'Keep updated' column.**

---

## References

| Topic                          | Description                         | Document                                                      | Keep updated |
|--------------------------------|-------------------------------------|---------------------------------------------------------------|--------------|
| Architecture                   | High-level application architecture | [`publish.md`](./docs/publish.md)                             | Yes |
| Publish                        | Publishing / Native AOT target      | [`publish.md`](./docs/publish.md)                             | Yes |
| WhoHolds.Cli project structure | Cli project structure               | [`core-project-structure.md`](docs/core-project-structure.md) | Yes |
| Testing | Testing with TUnit    | [`TUnit Skill`](.claude/skills/tunit/SKILL.md)                | No |

## Commands

- Access the Windows Server VM from Linux: `ssh winvm`
- Push your changes to the Windows Server VM: [`push-win.sh`](./scripts/windows-server-vm/push-win.sh)
- Use `-c Release` with `dotnet` commands