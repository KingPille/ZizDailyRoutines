# ZizDailyRoutines

Configurable helpers for everyday FFXIV tasks, customized for zizi45official.

## Install the plugin

Add this single URL under **Settings → Experimental → Custom Plugin Repositories**, enable it,
then search for **ZizDailyRoutines** in the plugin installer:

```text
https://raw.githubusercontent.com/KingPille/ZizPlugins/main/pluginmaster.json
```

Updates are offered through the same plugin repository. The installer downloads the complete package.

**[Download plugin ZIP](https://github.com/KingPille/ZizPlugins/releases/download/DailyRoutines-v1.0.0.0/ZizDailyRoutines-1.0.0.zip)** · **[Download source code](https://github.com/KingPille/ZizPlugins/releases/download/DailyRoutines-v1.0.0.0/ZizDailyRoutines-1.0.0-Source.zip)**

## Source code

The `src` folder contains eight C# projects. You can browse them here, use the download link above,
or choose GitHub's **Code → Download ZIP**. No Git command is needed to download the source.

`ZizDailyRoutines.slnx` groups the projects. `Prepare-Dependencies.ps1` downloads the matching
prebuilt package, verifies its SHA-256 hash and extracts dependencies into `lib`.
Set `DALAMUD_HOME` to your API 15 SDK directory. The default path uses XIVLauncher's development libraries.

A complete clean build of this source snapshot has not been validated. The prebuilt package
and source snapshot are provided separately; a successful source build is not claimed.

## Compatibility

Windows x64, .NET 10 and Dalamud/ZizLoader **API 15**. Other API levels are not compatible.
The package includes its companion DLLs and assets. Keep all files together for a manual installation.
Disable an existing development entry before loading another copy of the same plugin.
Keep your personal plugin configuration and progress files.

Archive integrity, manifest consistency and unchanged DLL payloads have been checked.
This package has not been runtime-tested in FFXIV. Report issues with the plugin version,
loader API level and the exact error message; never include passwords or access tokens.
