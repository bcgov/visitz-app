param([string]$CertificateThumbprint, [switch]$SuppressMismatchError)

$cert = Get-ChildItem Cert:\CurrentUser\My, Cert:\LocalMachine\My `
    | Where-Object { $_.Thumbprint -eq "$CertificateThumbprint" } `
    | Select-Object -First 1

if ($cert) {
    return $cert.Subject.Replace(",", "%2C")
} elseif (!$SuppressMismatchError) {
    throw "No matching certificate for '$CertificateThumbprint'"
}
