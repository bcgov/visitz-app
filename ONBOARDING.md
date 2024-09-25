# IDE Setup

## Windows

Use Visual Studio.

## Mac/OSX

Use VSCode.

Use this guide when setting up VSCode for the first time or updating framework versions.
 
### XCode and its tooling
 
1. Install the version of XCode that the app's current [MAUI version](https://github.com/dotnet/maui/wiki/Release-Versions) supports.

    [Visitz.csproj:81-82](https://github.com/BC-Gov-Social-Sector/mcfd-mobility/blob/794773121a6c77e074efba9e1a51acda13094ca2/visitz/Visitz/Visitz.csproj#L81). Example PackageReferences:

    ```xml
	<PackageReference Include="Microsoft.Maui.Controls" Version="x.y.z" />
	<PackageReference Include="Microsoft.Maui.Controls.Compatibility" Version="x.y.z" />
    ```
 
2. Make sure XCode command line tools are installed (XCode > Settings > Locations > Command Line Tools | or do `xcode-select --install` in terminal)
 
3. Uninstall all existing iOS simulators that don't match the version of your currently installed XCode version
 
4. Install the matching iOS simulator version for your current installed XCode version (even if you won't use it)

### .NET
 
1. Install matching major version of .NET that matches the current MAUI requirements (MAUI 8 = .NET 8)
 
	- Make sure to install the latest minor/patch version
 
2. Install workloads
 
	`sudo dotnet workload install maui ios maui-ios`
 
### Required extensions
 
Install and set up extensions:
 
### [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
 
### [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
 
Sign into C# Dev Kit extension with licensed account
 
### [.NET MAUI](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-maui) == version 1.3.29

***!!!*** There are issues with other versions' debuggers, so make sure you use this specific version.

### [EditorConfig](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig)
