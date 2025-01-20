param(
    # Leave blank for prod builds, or use "beta", "demo", "team"
    [string]
    [ValidateSet("prod", "beta", "team", "demo")]
    $Environment = "",

    # Must be larger number than the previous build
    [int] $BuildNumber = -1,

    [string] $BuildColor = "",

    [switch] $SelfContained,

    [switch] $NoOpenOutputDirectory,

    [switch] $AppxPackageSigningEnabled = $true,

    [string] $RuntimeIdentifierOverride = "win-x64", # Can also be "win10-x64"

    [string] 
    [ValidateSet("Process", "User", "Machine")]
    $EnvScope = "User"
)

Set-Variable CertificateThumbprintName -Option Constant -Value "Visitz_WinCertThumbprint"
Set-Variable CertificateSubjectName -Option Constant -Value "Visitz_WinCertSubject"
Set-Variable Oidc_AuthenticationDomainName -Option Constant -Value "Oidc_AuthenticationDomain"
Set-Variable Oidc_ClientIdName -Option Constant -Value "Oidc_ClientId"
Set-Variable Oidc_RedirectUriName -Option Constant -Value "Oidc_RedirectUri"
Set-Variable Api_HttpsApiDomainName -Option Constant -Value "Api_HttpsApiDomain"
Set-Variable ContactInfo_MailToAuthorizeName -Option Constant -Value "ContactInfo_MailToAuthorize"
Set-Variable ContactInfo_AccessRequestFormUrlName -Option Constant -Value "ContactInfo_AccessRequestFormUrl"
Set-Variable ContactInfo_FeedbackSurveyUrlName -Option Constant -Value "ContactInfo_FeedbackSurveyUrl"

function Set-Env {
    param($Name, $Value, $Scope)
    [System.Environment]::SetEnvironmentVariable($Name, $Value, $Scope)
}

function Get-Env {
    param($Name, $Scope)
    [System.Environment]::GetEnvironmentVariable($Name, $Scope)
}

function Ensure-Env {
    param($Name, $Scope)

    $envValue = Get-Env -Name $Name -Scope $Scope

    if (-not $envValue) {
        $val = Read-Host "Provide value for '$Name'"

        if ([string]::IsNullOrEmpty($val)) {
            throw "Value for '$Name' must not be empty"
        }

        Set-Env -Name $Name -Value $val -Scope $Scope
        $envValue = $val
    }

    return $envValue
}

# Certificate thumbprint. May need to open the Safenet client to get this value
[string] $CertificateThumbprint = Ensure-Env -Name $CertificateThumbprintName -Scope $EnvScope

# Commas need to be escaped: use %2C
# e.g. "CN=BCGOV%2C O=SDPR" is equivalent to "CN=BCGOV, O=SDPR"
[string] $CertificateSubject = Ensure-Env -Name $CertificateSubjectName -Scope $EnvScope

[string] $Oidc_AuthenticationDomain = Ensure-Env -Name $Oidc_AuthenticationDomainName -Scope $EnvScope
[string] $Oidc_ClientId = Ensure-Env -Name $Oidc_ClientIdName -Scope $EnvScope
[string] $Oidc_RedirectUri = Ensure-Env -Name $Oidc_RedirectUriName -Scope $EnvScope
[string] $Api_HttpsApiDomain = Ensure-Env -Name $Api_HttpsApiDomainName -Scope $EnvScope
[string] $ContactInfo_MailToAuthorize = Ensure-Env -Name $ContactInfo_MailToAuthorizeName -Scope $EnvScope
[string] $ContactInfo_AccessRequestFormUrl = Ensure-Env -Name $ContactInfo_AccessRequestFormUrlName -Scope $EnvScope
[string] $ContactInfo_FeedbackSurveyUrl = Ensure-Env -Name $ContactInfo_FeedbackSurveyUrlName -Scope $EnvScope

$env = $Environment

[xml]$props = Get-Content "..\visitz\Visitz\Visitz.props"
$version = (Select-Xml -Xml $props -XPath "//PropertyGroup/VisitzVersion").ToString()
$outputName = "MCFD Mobility-Windows-$env-$Version.$BuildNumber"

if ($Environment -eq "prod") {
    # "prod" only used for command line intention and build label, buildscript uses empty string for prod
    $env = ""
}

$artifactsDir = "artifacts"
$outputDir = "output"

if ([string]::IsNullOrEmpty($BuildColor)) {

    if ($env -eq "beta") {
        $BuildColor = "#005724"
    }
    elseif ($env -eq "team") {
        $BuildColor = "#66004b"
    }
    else {
        $BuildColor = "#003366" # prod, demo, etc.
    }
}

# Auto enable/disable debug mode based on the deployment env.
[string] $enableDebug = "false"
if ($env -eq "team") {
    $enableDebug = "true"
}

[string] $selfContainedString = ""
if ($SelfContained) {
    $selfContainedString = "--self-contained"
}

$appxEnabledString = $AppxPackageSigningEnabled.ToString().ToLowerInvariant()

dotnet publish "..\visitz\Visitz\Visitz.csproj" `
    --artifacts-path ".\$artifactsDir" `
    --framework net8.0-windows10.0.19041.0 `
    --configuration Release `
    $selfContainedString `
    -p:ApplicationVersion=$BuildNumber `
    -p:DeploymentEnvironment=$env `
    -p:AppxPackageSigningEnabled=$appxEnabledString `
    -p:PackageCertificateThumbprint=$CertificateThumbprint `
    -p:AppxCertificateSubject=$CertificateSubject `
    -p:BuildTypeColor=$BuildColor `
    -p:RuntimeIdentifierOverride="$RuntimeIdentifierOverride" `
    -p:UseAppSettings=true `
    -p:Oidc_AuthenticationDomain="$Oidc_AuthenticationDomain" `
    -p:Oidc_ClientId="$Oidc_ClientId" `
    -p:Oidc_RedirectUri="$Oidc_RedirectUri" `
    -p:Api_HttpsApiDomain="$Api_HttpsApiDomain" `
    -p:ContactInfo_MailToAuthorize="$ContactInfo_MailToAuthorize" `
    -p:ContactInfo_AccessRequestFormUrl="$ContactInfo_AccessRequestFormUrl" `
    -p:ContactInfo_FeedbackSurveyUrl="$ContactInfo_FeedbackSurveyUrl" `
    -p:Debug_EnableDebugOptions=$enableDebug

if ($?) {
    $msix = Get-ChildItem $artifactsDir -Recurse -Filter "Visitz*$BuildNumber*.msix"
    Write-Host "Found '$msix'"

    if (!(Test-Path $outputDir)) {
        mkdir $outputDir
    }

    $dest = ".\$outputDir\$outputName.msix"
    Copy-Item -Path $msix -Destination $dest
    Write-Host "Copied to '$dest'"

    $openOutputDir = !$NoOpenOutputDirectory

    if ($openOutputDir) {
        explorer $outputDir
    }
}
