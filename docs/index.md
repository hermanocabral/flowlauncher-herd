---
title: Herd
---

**Herd** is a [Flow Launcher](https://www.flowlauncher.com/) plugin that lets you define named
**groups of applications** and launch every app in a group with a single keystroke.

- [Install](#install)
- [Quick start](#quick-start)
- [Creating and editing groups](#creating-and-editing-groups)
- [App options](#app-options)
- [Launch modes](#launch-modes)
- [Changing the action keyword](#changing-the-action-keyword)
- [How launching works](#how-launching-works)
- [Troubleshooting](#troubleshooting)
- [FAQ](#faq)
- [Uninstall](#uninstall)

## Install

### From the Flow Plugin Store

In Flow Launcher, run `pm install Herd`, or find **Herd** under **Settings → Plugin Store**.
New store listings can take a few days to propagate across Flow's CDNs.

### By URL or local path

Download `Flow.Launcher.Plugin.Herd-<version>.zip` from the
[Releases page](https://github.com/hermanocabral/flowlauncher-herd/releases) and run:

```
pm install https://github.com/hermanocabral/flowlauncher-herd/releases/latest/download/Flow.Launcher.Plugin.Herd-<version>.zip
```

You can also point `pm install` at a downloaded `.zip` on disk.

### From source

Requires the [.NET SDK](https://dotnet.microsoft.com/) (8+) and [go-task](https://taskfile.dev/):

```bash
git clone https://github.com/hermanocabral/flowlauncher-herd
cd flowlauncher-herd
task install:local
task reload
```

## Quick start

1. Open Flow and type the keyword (default `herd`).
2. Start typing a group name to filter the list.
3. Press <kbd>Enter</kbd> to launch every enabled app in the group.

If you have no groups yet, Herd shows a **"No app groups yet"** result — press
<kbd>Enter</kbd> on it to open settings and create your first group.

## Creating and editing groups

Open **Flow Launcher Settings → Plugins → Herd**.

- The **left panel** lists your groups, with **Add**, **Duplicate**, and **Delete** buttons.
- Selecting a group shows its editor on the **right**: name, description, optional custom
  icon, launch mode and delay, and the table of apps.

Changes are saved automatically — structural edits immediately, and field edits when you close
the settings window.

### Group fields

| Field        | Description                                                          |
| ------------ | -------------------------------------------------------------------- |
| Name         | Shown in the launcher and matched against your search text.          |
| Description  | Shown in the result subtitle and the hover tooltip.                  |
| Custom icon  | Optional `.png`/`.ico`/`.jpg` shown instead of the default sheep.    |
| Launch mode  | **Parallel** or **Sequential** (see [Launch modes](#launch-modes)).  |
| Delay (ms)   | Pause between launches in sequential mode.                           |

## App options

Each row in a group's app table:

| Field        | Description                                                                       |
| ------------ | --------------------------------------------------------------------------------- |
| On           | Enable/disable the app without removing it from the group.                        |
| Target       | An executable, file, folder, or URL. Environment variables like `%USERPROFILE%` are expanded. |
| Name         | Friendly label. Defaults to the target's file name.                               |
| Arguments    | Command-line arguments passed to the target.                                      |
| Working dir  | Folder to start in. **Leave empty to default to the app's own folder.**           |
| Admin        | Launch elevated. Windows shows a UAC prompt each time.                            |

Toolbar buttons: **Add file…**, **Add folder…**, **Add URL**, **Set working dir…**,
**Move up**, **Move down**, **Remove**. Order matters in sequential mode.

## Launch modes

- **Parallel** *(default)* — every enabled app starts immediately, in one burst.
- **Sequential** — apps start in table order, pausing **Delay (ms)** between each. Useful when
  one app must be running before another (e.g. a VPN before a remote client, or a server before
  its dashboard).

A disabled app is skipped. If one app fails to launch, the rest still launch and Herd shows a
message naming what failed.

## How launching works

Herd starts each target through the Windows shell, so executables, documents, folders, and URLs
all work the same way:

- **Working directory** — the value you set, or (when blank) the target file's own folder.
  URLs and bare commands have no working directory.
- **Run as admin** — launches with the `runas` verb, producing a UAC prompt.
- **URLs** — anything with a scheme (`https://`, `obsidian://`, …) opens in its default handler.

## Troubleshooting

**An app doesn't launch / "some apps didn't start" message**
- Check the **Target** path is correct and the file exists. For bare commands (e.g. `code`),
  make sure they resolve on your `PATH`.
- For Store apps, point the target at the real executable or a shortcut, or use a URL/protocol
  if the app provides one.

**App opens in the wrong folder**
- Set **Working dir** explicitly. When it's blank, Herd uses the executable's own folder, which
  isn't always what a portable app expects.

**It asks for admin every time**
- That's expected for **Admin**-flagged apps — Windows requires a UAC confirmation per launch.
  Untick **Admin** if the app doesn't actually need elevation.

**Sequential apps start too fast / in the wrong order**
- Increase **Delay (ms)** and use **Move up/down** to set the order.

**My keyword collides with another plugin**
- Change it under **Settings → Plugins → Herd → Action Keywords**.

**Settings didn't save**
- Settings persist when the settings window closes. If Flow was killed abruptly, reopen
  settings and confirm your changes, then close the window normally.

**A custom group icon doesn't show**
- Use an absolute path to a `.png`/`.ico`. If the file moves, the icon falls back to the
  default sheep.

## FAQ

**Does it work on macOS/Linux?**
Flow Launcher is Windows-only, so Herd is too.

**Can I launch websites and folders, not just apps?**
Yes — put a URL or a folder path in **Target**.

**Will launching block Flow?**
No. Launches run in the background and the Flow window closes immediately, even in sequential
mode.

**Where is my configuration stored?**
In your Flow Launcher settings storage for the plugin (managed by Flow). Use the settings UI
rather than editing files by hand.

## Uninstall

In Flow: **Settings → Plugins → Herd → Uninstall**, then restart Flow. If you installed from
source, `task uninstall:local` removes it from the plugins folder.

---

Source code and issues: [github.com/hermanocabral/flowlauncher-herd](https://github.com/hermanocabral/flowlauncher-herd) · [MIT License](https://github.com/hermanocabral/flowlauncher-herd/blob/main/LICENSE)
