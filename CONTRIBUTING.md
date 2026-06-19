# Contributing to Herd

Thanks for your interest in improving Herd! 🐑

## Getting started

You'll need the [.NET SDK](https://dotnet.microsoft.com/) (8 or newer) and
[go-task](https://taskfile.dev/).

```bash
task            # list tasks
task test       # run the TUnit suite
task test:watch # TDD loop while you work
task dev        # install locally + restart Flow to try it
```

## Ground rules

- **Tests first.** New behaviour comes with TUnit tests. Keep logic in the services/view-models
  (which are unit-tested) and the WPF views thin. Process launching is behind `IProcessLauncher`
  so tests never spawn real processes.
- **Conventional Commits.** Use `feat:`, `fix:`, `test:`, `refactor:`, `docs:`, `chore:`,
  `ci:`, `build:`. Keep commits small and atomic.
- **Formatting.** Run `task lint` (or `task format`) before pushing; CI runs the test suite.
- **Signed commits.** Commits pushed to the repo must be signed.

## Releasing

Bump the version in `src/Flow.Launcher.Plugin.Herd/plugin.json`, then:

```bash
task release    # tags vX.Y.Z and pushes it (signed); CI builds and publishes the zip
```

The release workflow stamps the version, runs tests, publishes, zips, and creates a GitHub
Release. The Flow Plugin Store auto-syncs new releases after the one-time store listing PR.
