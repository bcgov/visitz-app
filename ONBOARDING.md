### Read the document fully before following the steps.

# Dev environment setup

This document will help you set up your development environment to build and run the Visitz app.

## Enable developer mode (Windows only)

Using Visual Studio to compile and run the Visitz app requires Windows' Developer Mode to be turned on.

Go to https://learn.microsoft.com/en-us/windows/advanced-settings/developer-mode for a guide on how to enable it.

## Install .NET

> !! Note: this project pins an exact version of the .NET SDK to be used for development and CI/CD. You will need to install the exact SDK version listed in the [global.json](global.json) file or builds will not run. See [.NET version pinning](#net-version-pinning) for details.

1. From the [.NET distribution site](https://dotnet.microsoft.com/en-us/download/dotnet), download the exact SDK for your platform listed in the global.json file.

1. In CLI/terminal, Navigate to [`visitz/Visitz`](visitz/Visitz).

1. Install workloads

	> Note: You may need to run terminal as Administrator if you are encountering issues or errors.

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

1. Build Visitz.csproj

	For Windows:

	```powershell
	dotnet build --framework net10.0-windows10.0.19041.0
	```

	> !! Note: if the framework identifier `--framework net10.0-windows10.0.19041.0` does not work, you can find the correct one in [`visitz/Visitz/Visitz.csproj`](visitz/Visitz/Visitz.csproj). Look for the `TargetFrameworks` property with `Condition="$([MSBuild]::IsOSPlatform('windows'))"` to find it.

	If there are no build errors, you should be good to proceed.

## .NET version pinning

You can override the default dotnet SDK version used in terminal for a single project using a `global.json` file. This is sometimes necessary when using Visual Studio, as it ships with its own internal version of .NET—which may not match the SDK version currently used by the app.

The [global.json](global.json) file for this project used to be made on-demand by devs or CI/CD, but it is now committed in version history for simplicity.


## Windows setup

Use Visual Studio.

> Note: Visual Studio can be used to run and debug both Windows and iOS builds [(via Pair to Mac)](https://learn.microsoft.com/en-us/dotnet/maui/ios/pair-to-mac?view=net-maui-10.0) of the app.

> Note 2: You can also us VSCode on Windows, but your developer experience will be much better in full Visual Studio.

1. Restart Visual Studio if it was running while you were installing .NET and running the restore/build steps

1. Make sure developer mode is enabled from the previous step

### Troubleshooting

You may encounter weird errors like exceptions with no message, or compiler errors that don't make sense.

If you do, try cleaning the solution and running `dotnet restore` from terminal before running the app in Visual Studio again.

If you've already run the app and gotten crashes or errors, you can try uninstalling the app from the system before running again.

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
