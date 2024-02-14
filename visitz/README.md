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
- System.IdentityModel.Tokens.Jwt

### Integrations

Visitz uses API endpoints which have been set up in another repository.

### Project Structure

Visitz is built using two C# projects: "Visitz" and "VisitzApi":

- Visitz: Bulk of the business-code
- VisitzApi: API wrapper for interacting with API integrations.

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

    - Visitz is currently only being built for iOS and Windows. Other platforms may or may not work when launching.

