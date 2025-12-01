# Dev environment setup

## Windows

Use Visual Studio.

> Visual Studio can be used to run and debug both Windows and iOS builds [(via Pair to Mac)](https://learn.microsoft.com/en-us/dotnet/maui/ios/pair-to-mac?view=net-maui-10.0) of the app.

From Visual Studio Installer, select components/workloads:

- Visual Studio
- .NET Multi-platform App UI development workload

You can also us VSCode on Windows, but DevEx will be much better in full Visual Studio.

## Mac/OSX

Use VSCode.

> VSCode on Mac can only be used to run and debug iOS/Maccatalyst builds of the app.

Use this guide when setting up VSCode for the first time or updating framework versions.
 
### XCode and its tooling
 
1. Install the version of XCode that the app's current [MAUI version](https://github.com/dotnet/maui/wiki/Release-Versions) supports.

	You can find the current MAUI version used by checking the Visitz.csproj for `Microsoft.Maui.Controls*` versions—e.g., 
    [Visitz.csproj:83-84](/visitz/Visitz/Visitz.csproj#L83). Example PackageReferences:

    ```xml
	<PackageReference Include="Microsoft.Maui.Controls" Version="x.y.z" />
	<PackageReference Include="Microsoft.Maui.Controls.Compatibility" Version="x.y.z" />
    ```
 
2. Make sure XCode command line tools are installed (XCode > Settings > Locations > Command Line Tools | or do `xcode-select --install` in terminal)
 
3. Uninstall all existing iOS simulators that don't match the version of your currently installed XCode version
 
4. Install the matching iOS simulator version for your current installed XCode version (even if you won't use it)

### .NET
 
1. Install matching major version of .NET that matches the current MAUI requirements (MAUI 8 = .NET 8, MAUI 9 = .NET 9, etc.)
 
	- Make sure to install the latest minor/patch version

1. In CLI/terminal, Navigate to `visitz/Visitz`
 
1. Install workloads

	```bash
	sudo dotnet workload restore
	```

1. If previous step fails, directly install required workloads

	```bash
	sudo dotnet workload install maui ios maccatalyst maui-ios maui-maccatalyst
	```
 
### Required extensions
 
Install and set up extensions:
 
1. [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
 
1. [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
 
	Sign into C# Dev Kit extension with licensed account
 
1. [.NET MAUI](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-maui)

	***!!!*** There have been issues with other versions' debuggers, so if you have issues, try using version **1.3.29**.

1. [EditorConfig](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig)

## Set up Apple certificates and provisioning profiles

Assuming your Apple account is already authorized and can access certs and profiles for the project:

1. Create a dummy project in XCode

1. Give it the same bundle ID as the Visitz app (check [Visitz.csproj](visitz/Visitz/Visitz.csproj) for app ID)

1. Enable "Automatically manage signing" and let XCode set up certs, CA's and provisioning profiles for you

1. Try to build and run the dummy project, it will probably fail

1. XCode should now have set up everything you need in the chain of trust to run development builds of the app

	***!!!*** If your development certificate is still not trusted by the machine, you'll need to [manually download](https://www.apple.com/certificateauthority/AppleWWDRCAG3.cer) and install Apple's CA for dev certs

---

# App launch setup

## Inflate appSettings.json

1. Navigate to `visitz/Visitz`

1. Make a copy of "appSettings.json.template" as "appSettings.json" right beside it. This new file will be ignored by Git.
    
1. Replace any `{{Templated}}` value with desired values. You can find valid values in this repository's environment variables.

## Open project

- **Visual Studio**: launch using the solution file (`visitz/Visitz.sln`)
- **VSCode**: File > Open Folder > select root level repository folder

## Select build configuration and launch

Visitz currently only supports being built for iOS and Windows. Other platforms may or may not work when launching.

- **Visual Studio**: choose configuration (Windows Machine or iOS options) and run.
- **VSCode**: select simulator or physical device and run.
