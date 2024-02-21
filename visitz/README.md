# Visitz

## Architecture

Visitz is built using .NET MAUI.

- .NET 8.0.101
- .NET MAUI 8.0.6

### Notable Libraries (NuGet)

- CommunityToolkit.Maui
- CommunityToolkit.Mvvm
- IdentityModel.OidcClient
- Plugin.Fingerprint
- Realm (MongoDB Realm)
- SecurityCodeScan.VS2019 (Security analysis)
- SharpSource (Linter)
- System.IdentityModel.Tokens.Jwt

### Integrations

Visitz uses API endpoints which have been set up in another repository.

### Project Structure

Visitz is built using several C# projects:

- `Visitz`: The MAUI implementation of the app.
- `VisitzApi`: Wrapper for interacting with API integrations.
- `VisitzModel`: The bulk of the business-logic.
- `Oidc`: OIDC authentication implemenation for MAUI.

Except for `Visitz`, each project has its own xUnit testing project associated with it.

## Environment Setup

1. From Visual Studio Installer

    - Visual Studio

    - .NET Multi-platform App UI development Workload (.NET 8)

2. Clone this repository

3. Inflate appSettings.json

    - Navigate to /visitz/Visitz

    - Make a copy of "appSettings.json.template" as "appSettings.json" right beside it. This new file will be ignored by Git.
    
    - Replace any `{{Templated}}` value with desired values. You can find valid values in this repository's environment variables.

4. Open project in Visual Studio

    - Make sure you launch using the solution file (`visitz/Visitz.sln`)

5. Select build configuration and launch

    - Visitz currently only supports being built for iOS and Windows. Other platforms may or may not work when launching.
