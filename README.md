<p align="center">
  <img src="src/Flow.Launcher.Plugin.Herd/Images/icon.png" width="120" alt="Herd logo" />
</p>

<h1 align="center">Herd</h1>

<p align="center">
  <em>Herd your apps into groups and launch the whole flock at once.</em>
</p>

<p align="center">
  <a href="https://github.com/hermanocabral/flowlauncher-herd/actions/workflows/ci.yml"><img src="https://github.com/hermanocabral/flowlauncher-herd/actions/workflows/ci.yml/badge.svg" alt="CI" /></a>
  <a href="https://github.com/hermanocabral/flowlauncher-herd/releases"><img src="https://img.shields.io/github/v/release/hermanocabral/flowlauncher-herd?include_prereleases&sort=semver" alt="Release" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-yellow.svg" alt="License: MIT" /></a>
  <img src="https://img.shields.io/badge/Flow%20Launcher-plugin-blue" alt="Flow Launcher plugin" />
</p>

---

**Herd** is a [Flow Launcher](https://www.flowlauncher.com/) plugin that lets you define named
**groups of applications** and launch every app in a group with a single keystroke. Open your
whole "work" setup — editor, terminal, browser, chat — or your "gaming" setup, in one go.

Type your keyword, pick a group, press <kbd>Enter</kbd>. The flock takes off. 🐑

## Why

Opening the same five apps every morning is five trips to the launcher. Herd turns that into
one. Each app remembers its own working directory, arguments, and whether it needs to run as
admin — and a group can fire everything at once or stagger launches when one app needs a head
start.

## Features

- 🐑 **Groups of apps** — name a set of apps and launch them together with one <kbd>Enter</kbd>.
- 🗂️ **Per-app options** — target (exe / file / folder / URL), display name, arguments,
  working directory, run-as-admin, and an enable/disable toggle.
- ⚡ **Launch modes** — *parallel* (all at once) or *sequential* (one by one, with a delay)
  for apps that must start in order.
- 🎨 **Custom icons & descriptions** — give each group its own icon and a description that
  shows in the result panel.
- 🛠️ **Friendly settings UI** — manage everything from a panel in Flow's settings; no JSON
  editing required.
- 🔁 **Configurable keyword** — the default keyword is `herd`, changeable in Flow's settings
  if it clashes with another plugin.
- 🛡️ **Resilient** — one app failing to start never stops the rest; you get a clear message.

## Install

### From the Flow Plugin Store (recommended)

In Flow Launcher, run:

```
pm install Herd
```

(Once listed in the store you can also find it under Settings → Plugin Store.)

### Manual / specific version

Grab the latest `Flow.Launcher.Plugin.Herd-x.y.z.zip` from
[Releases](https://github.com/hermanocabral/flowlauncher-herd/releases) and install it by URL or
local path:

```
pm install https://github.com/hermanocabral/flowlauncher-herd/releases/latest/download/Flow.Launcher.Plugin.Herd-<version>.zip
```

### From source

Requires the [.NET SDK](https://dotnet.microsoft.com/) (8 or newer) and
[go-task](https://taskfile.dev/).

```bash
task install:local   # build, publish, and copy into %APPDATA%\FlowLauncher\Plugins
task reload           # restart Flow Launcher to pick it up
```

## Usage

1. Open Flow and type your keyword (default `herd`).
2. Start typing a group name to filter; the panel shows the group's description and apps.
3. Press <kbd>Enter</kbd> to launch every enabled app in the group.

The first time you use it, Herd shows a *"No app groups yet"* hint — press <kbd>Enter</kbd> on
it to jump straight to settings and create your first group.

## Configuring groups

Open **Flow Launcher Settings → Plugins → Herd**. On the left is your list of groups
(**＋ New group / Duplicate / Delete**). Select a group to edit it on the right:

| Group field   | What it does                                                              |
| ------------- | ------------------------------------------------------------------------- |
| Name          | Shown in the launcher and matched against your search.                    |
| Description   | Shown in the result subtitle and tooltip.                                 |
| Custom icon   | Optional image shown instead of the default sheep.                        |
| Launch        | **All at once**, or **one by one** with a delay between each.             |

Each app is a card — click it to expand its options:

| App field         | What it does                                                                    |
| ----------------- | ------------------------------------------------------------------------------- |
| Enabled           | Tick/untick to include or skip the app without removing it.                     |
| Target            | Path to an executable/file/folder, or a URL.                                    |
| Name              | Friendly label (defaults to the target's file name).                            |
| Arguments         | Command-line arguments passed on launch.                                        |
| Working dir       | Folder to start in. **Leave empty to use the app's own folder.**                |
| Admin             | Launch elevated (triggers a UAC prompt).                                        |

Click **＋ Add app** (File / Folder / Web link) to add an app. Click an app to expand it and edit
its name, target, arguments, working directory and run-as-admin — and to move or remove it. Order
matters when the group launches "one by one".

### Changing the keyword

The action keyword defaults to `herd`. To change it (e.g. to avoid clashing with another
plugin), go to **Settings → Plugins → Herd** and edit the **Action Keywords** field — this is
built into Flow Launcher.

## Documentation

Full guide, tips, and troubleshooting:
**[hermanocabral.github.io/flowlauncher-herd](https://hermanocabral.github.io/flowlauncher-herd/)**

## Development

```bash
task            # list all tasks
task test       # run the TUnit test suite
task test:watch # TDD loop
task build      # Release build
task package    # produce the release zip in dist/
task dev        # install locally + restart Flow
task lint       # verify formatting
```

The plugin targets `net8.0-windows` (LTS; loads on current Flow Launcher's .NET 8/9 runtime). Tests use
[TUnit](https://tunit.dev/) on the Microsoft.Testing.Platform. Core logic — path resolution,
launch orchestration, query building, and the settings view-model — is covered by unit tests,
with process launching abstracted behind an interface so nothing is spawned during tests.

## Contributing

Issues and PRs are welcome. Please keep changes covered by tests and use
[Conventional Commits](https://www.conventionalcommits.org/). See
[`CONTRIBUTING.md`](CONTRIBUTING.md).

## License

[MIT](LICENSE) © Hermano Cabral
