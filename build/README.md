ℹ️ Read this document fully before following its steps.

# Making releases for Windows

Windows release builds must be run locally (by a human!) due to restrictions on the code-signing certificates we use.

## Requirements

1. A Windows machine with PowerShell installed.

1. You should already be able to make Debug builds of the app on Windows. If you can't, follow the [onboarding documentation](../ONBOARDING.md).

1. You need a valid code-signing certificate installed on your machine. You will probably also need the physical USB token required to authorize the certificate during use.

1. Set up environment variables required by the build script:

    1. `CertificateThumbprint` - The thumbprint of the required code-signing certificate. (dynamic per expiration)

        1. To list thumbprint & subject of all code-signing certificates installed (with private keys available), you can run the following in PowerShell:
            ```powershell
            Get-ChildItem Cert:\CurrentUser\My, Cert:\LocalMachine\My | Where-Object {
                $_.EnhancedKeyUsageList.ObjectId -contains '1.3.6.1.5.5.7.3.3' -and
                $_.HasPrivateKey -and
                $_.NotAfter -gt (Get-Date)
            } | Select-Object Thumbprint, Subject
            ```

    1. `Oidc_AuthenticationDomain` (dynamic per environment, manual)

    1. `Oidc_ClientId` (static)

    1. `Oidc_RedirectUri` (static)

    1. `Api_HttpsApiDomain` (dynamic per environment, manual)
    
    1. `ContactInfo_FeedbackSurveyUrl` (static)

    1. `Debug_EnableDebugOptions` (dynamic per environment, automatic)

## Running the release build

Using PowerShell, release builds are run by executing the [Build-WindowsRelease.ps1](Build-WindowsRelease.ps1) script with a build number and build environment:

```powershell
.\Build-WindowsRelease.ps1 -BuildNumber 1337 -Environment team
```

You can choose whatever positive integer you want for the build number as long as it is larger than the most recently released build (per environment). It's recommended to +1 from previous build number.

> ℹ️ Over time, production build number deltas will be much larger than team build number deltas (prod 2.9.0.133 -> prod 3.0.0.168 vs team 3.0.0.158 -> team 3.0.0.159).

> ℹ️ You can see which environments are available to build for by opening the buildscript and checking the `$Environment` argument.

The buildscript accepts more arguments if you need customization or are debugging the script, but it is usually enough to just provide a number and environment.

After the build has completed, the script will attempt to sign the output MSIX file. This step requires a human to enter a password for the USB token, and is the sole reason Windows builds cannot be automated completely.

After signing, the script will open a Windows Explorer window to the MSIX file's location for your convenience.

## Building for different environments

We currently don't have fancy matrix builds for Windows. We need to manually change environment variables between team and prod builds:

* `Oidc_AuthenticationDomain`

* `Api_HttpsApiDomain`

The buildscript will automatically handle changing the `Debug_EnableDebugOptions` variable.

## Deploying files to users

There are 2 groups of users we deploy to: team members and production users. For both groups, we deploy the MSIX files to BC Gov's internal Artifactory instance.

### Deploying to team members

If the build is not meant for production, then depending on the environment it was built for, it can go in either:

* demo-Windows

* team-Windows

### Deploying to production users

If the build is meant for production, you will need to make 2 more builds of the same commit to cover all environments and place them in:

* demo-deployments

* prod-deployments

* team-deployments

> ℹ️ We make multiple builds of the same commit because:
> 
> 1. It's easier to compare behaviours with pre-built binaries across release versions (e.g. 2.9.0 -> 3.0.0) instead of asking developers to build on-demand
> 1. The demo and team builds are "mirrors" to prod, allowing non-developer team members to help troubleshoot issues with users
> 1. Demo builds are used to make user guides and documentation, so they must be identical to production builds.