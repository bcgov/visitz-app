# Dev environment setup

This document will help you set up your development environment to build and run the Visitz app.

Read the document fully before following the steps.

## Enable developer mode (Windows only)

Using Visual Studio to compile and run the Visitz app requires Windows' Developer Mode to be turned on.

Go to https://learn.microsoft.com/en-us/windows/advanced-settings/developer-mode for a guide on how to enable it.

## Install .NET

> !! Note: if you have multiple versions of .NET installed or are using Visual Studio, you may need to pin the version of the runtime you want to use. See [.NET version pinning](#net-version-pinning) for details.

1. Navigate to [`visitz/Visitz/Visitz.csproj`](visitz/Visitz/Visitz.csproj) and look for the `TargetFrameworks` property to see which major .NET version to install.

	As an example, if you find:

	```xml
	<TargetFrameworks>net9.0-maccatalyst;net9.0-ios</TargetFrameworks>
	```

	then you must install .NET 9.

1. From the [.NET distribution site](https://dotnet.microsoft.com/en-us/download/dotnet), download the latest SDK for the matching major version for your platform.

1. In CLI/terminal, Navigate to [`visitz/Visitz`](visitz/Visitz).

1. Install workloads

	```powershell
	dotnet workload install maui ios maccatalyst maui-ios maui-maccatalyst
	dotnet workload restore
	```

1. Restore projects

	```powershell
	dotnet restore
	```

	> Note: `dotnet build` is supposed to run its own `restore`, but sometimes it doesn't. So we want to explicitly restore to make sure it happens.

	> Note 2: you may need to run a restore in between workload installations.

1. Build

	```powershell
	dotnet build
	```

	If there are no build errors, you should be good to proceed.

## .NET version pinning

You can override the default dotnet SDK version used in terminal for a single project using a `global.json` file. This is sometimes necessary when using Visual Studio, as it ships with its own internal version of .NET—which may not match the SDK version currently used by the app.

In the root directory of the repository, make a copy of [`global.json.template`](global.json.template), rename it to `global.json` in the same directory, and set the SDK version you wish to use in `"version": "DOTNET_SDK_VERSION"`.

An example could look like:

```json
{
  "sdk": {
    "version": "9.0.309", // Example only. This may not be the latest version
    "allowPrerelease": false,
    "rollForward": "disable"
  }
}
```

If you don't know the SDK version you want (it is different from the main version), you can find it on the [.NET distribution site](https://dotnet.microsoft.com/en-us/download/dotnet) or run a terminal command to see locally installed versions:

```powershell
dotnet --list-sdks
```

## Windows setup

Use Visual Studio.

> Visual Studio can be used to run and debug both Windows and iOS builds [(via Pair to Mac)](https://learn.microsoft.com/en-us/dotnet/maui/ios/pair-to-mac?view=net-maui-10.0) of the app.

You can also us VSCode on Windows, but your developer experience will be much better in full Visual Studio.

## Mac/OSX setup

Follow Mac-specific steps in [ONBOARDING-MAC.md](ONBOARDING-MAC.md) before proceeding with this guide.

## App settings

Visitz requires some settings in a configuration file for full functionality:

1. Navigate to [`visitz/Visitz`](visitz/Visitz) in the repository

1. Make a copy of [`visitz/Visitz/appSettings.json.template`](visitz/Visitz/appSettings.json.template) and rename to `appSettings.json` in the same directory. This new file will be ignored by Git.
    
1. Replace any `{{Templated}}` value with desired values. You can find valid values in this repository's environment variables or from another developer.

## Open project

- **Visual Studio**: launch using the solution file [`visitz/Visitz.sln`](visitz/Visitz.sln)
- **VSCode**: File > Open Folder > select root level repository folder

## Select build configuration and launch

Visitz currently only supports being built for iOS and Windows. Other platforms may or may not work when launching.

- **Visual Studio**: choose configuration (Windows Machine or iOS options) and run.
- **VSCode**: select simulator or physical device and run.
