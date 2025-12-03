param(
    [string] $MsixPath,

    # http://timestamp.digicert.com
    # http://timestamp.comodoca.com
    # http://timestamp.globalsign.com
    # http://tsa.starfieldtech.com
    # http://timestamp.entrust.net/TSS/RFC3161sha2TS
    # http://sha256timestamp.ws.symantec.com/sha256/timestamp
    # http://tsa.swisssign.net
    [string] $TimestampUrl = "",

    # https://learn.microsoft.com/en-us/windows/msix/package/sign-app-package-using-signtool#determine-the-hash-algorithm
    [string] $Algorithm = "",

    # !!! Needs Windows SDK to be installed (possibly also "ClickOnce" publishing tools), check Visual Studio installer
    [string] $SignToolPath = "",

    [string] $CertificateThumbprint = "",

    [string] 
    [ValidateSet("Process", "User", "Machine")]
    $EnvScope = "User"
)

Set-Variable CertificateThumbprintName -Option Constant -Value "Visitz_WinCertThumbprint"

function Get-Env {
    param($Name, $Scope)
    [System.Environment]::GetEnvironmentVariable($Name, $Scope)
}

if (-not $TimestampUrl) {
    $TimestampUrl = "http://timestamp.digicert.com"
}

if (-not $SignToolPath) {
    $paths = Get-ChildItem "C:\Program Files (x86)\Windows Kits\*\bin\*\x64" -Recurse -Filter "SignTool.exe"
    $SignToolPath = $paths[-1]
}

if (-not $Algorithm) {
    $Algorithm = "certHash"
}

if (-not $CertificateThumbprint) {
    $CertificateThumbprint = Get-Env $CertificateThumbprintName -Scope $EnvScope
}

Write-Host "Using SignTool from '$SignToolPath'"
Write-Host "Using TimestampUrl '$TimestampUrl'"
Write-Host "Using algorithm '$Algorithm'"
Write-Host "Using thumbprint '$CertificateThumbprint'"

& $SignToolPath sign `
    /t "$TimestampUrl" `
    /fd "$Algorithm" `
    /sha1 "$CertificateThumbprint" `
    "$MsixPath"
