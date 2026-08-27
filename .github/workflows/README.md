# Github Environments

|	Intended usage				|	GH Environment	|	Loginproxy	|	Visitz API	|	Debug options	|
|	---							|	---				|	---			|	---			|	---				|
|	Team members				|	project-team	|	test		|	test		|	*enabled*		|
|	Demos						|	demo			|	test		|	test		|	**disabled**	|
|	General users				|	prod			|	prod		|	prod		|	**disabled**	|

Any environment that isn't listed in this table is temporary.

# Tests

On push, if any files in the `visitz/` directory have changed, an Action will be triggered that runs all tests for the Visitz MAUI project.

The production branch requires all tests succeed before allowing a PR merge.

# iOS release builds

"Auto builds" workflow runs automatically every pull request `synchronize` event.

You can also run "Auto builds" workflow manually. Choose branch to build from:

- **production** branch for production and demo builds

- **dev\*** or working branches for project-team builds

On success, artifacts for all environments are uploaded to BC Gov's Artifactory instance

## Update iOS secrets

### IOS_BUILD_PROVISION_PROFILE_BASE64

A base64-encoded copy of the distribution provisioning profile. You can base64 encode the file using bash:

```bash
base64 -i <provisioning profile name>.mobileprovision | pbcopy
```

`pbcopy` takes output and sets it to the clipboard. You can then directly paste into GH secrets **without** quotes. Remove the newline after pasting.

The workflow will automatically read certificate and provisioning profile names. 

<details>

<summary>If this fails, you will need to modify the workflow and provide the names as secret values (tap to expand)</summary>

### IOS_BUILD_CERTIFICATE_NAME

The name of the distribution certificate associated with the IOS_BUILD_PROVISION_PROFILE_BASE64 provisioning profile.

Its format is `<Certificate Type>: <Certificate Name>`. It should be wrapped in quotes when saved in secrets, e.g. `"iPhone Distribution: Some person's distribution cert name"`

***IMPORTANT***: The Apple Developer site may show "iOS Distribution" instead of "iPhone Distribution" for the `Certificate Type`. This is just a front-end label change—use "iPhone Distribution" instead. If you're absolutely not sure what prefix to use, import the certificate into an OSX keychain, `Get Info` on it, and use its full `Common Name`.

### IOS_CODE_SIGN_PROVISION_PROFILE_NAME

The name of the distribution provisioning profile.

It should be wrapped in quotes when saved in secrets, e.g. `"Provisioning Profile - Name"`
</details>

# Windows release builds (UNUSED: only local builds are currently supported)

Until we are allowed to use CI/CD pipelines to build as per Certificate Authority policies, Windows builds must be made manually by developers.

<details>

<summary>Windows CI/CD build guide</summary>

## Build

1. Run "Build Windows release package" workflow manually

2. Choose branch to build from

	- **production** branch for production and demo builds

	- **dev\*** or working branches for project-team builds

3. Choose a GitHub Environment to build with

4. Run workflow

5. On success, manually distribute GH Action artifacts as required

## Update secrets

- ### WINDOWS_RELEASE_CERT_THUMBPRINT

	The thumbprint of the code signing certificate stored in WINDOWS_RELEASE_CERT_BASE64.

	It does not need to be wrapped in quotes when saved in secrets.

- ### WINDOWS_RELEASE_CERT_BASE64

	A base64-encoded copy of the code signing certificate.

	To encode via PowerShell:

	> `[System.Convert]::ToBase64String([IO.File]::ReadAllBytes("<filename>")) | Set-Clipboard`

	`Set-Clipboard` takes output and sets it to the clipboard. Then directly paste into secrets **without** quotes. A trailing newline is ok.

- ### WINDOWS_RELEASE_CERT_PASSWORD

	The password for the code signing certificate—used when importing.

	It does not need to be wrapped in quotes when saved in secrets.

## Testing builds with a self-signed certificate

> If you sign a build with a self-signed certificate, users of the app must explicitly trust and install your self-signed cert before they can use the app, which could become a security issue. Proceed with caution.

1. [Create a signing certificate](https://learn.microsoft.com/en-us/dotnet/maui/windows/deployment/publish-cli?view=net-maui-8.0#create-a-signing-certificate)

	```powershell
	New-SelfSignedCertificate `
		-Type Custom `
		-Subject "CN=MCFD" `
		-KeyUsage DigitalSignature `
		-FriendlyName "MCFD Mobility self-signed testing cert" `
		-CertStoreLocation "Cert:\CurrentUser\My" `
		-TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")
	```

	The `-Subject` value must match the Package.Identity.@Publisher value in Package.appxmanifest.

2. Run a local dotnet publish build using the new certificate you created. Refer to make-windows-build.yaml on how to run the publish build.

3. [Install the app](https://learn.microsoft.com/en-us/dotnet/maui/windows/deployment/publish-cli?view=net-maui-8.0#installing-the-app)

	Instructions from guide:

	1. Right-click on the .msix file and choose Properties.

	2. Select the Digital Signatures tab.
	
	3. Choose the certificate then press Details.
	
	4. Select View Certificate.
	
	5. Select Install Certificate....
	
	6. Choose Local Machine then select Next.
	
		> Important! You must choose `Local Machine` or the certificate won't be discoverable by the installer.
	
	7. If you're prompted by User Account Control to Do you want to allow this app to make changes to your device?, select Yes.
	
	8. In the Certificate Import Wizard window, select Place all certificates in the following store.
	
	9. Select Browse... and then choose the Trusted People store. Select OK to close the dialog.
	
	10. Select Next and then Finish. You should see a dialog that says: The import was successful.
	
	11. Select OK on any window opened as part of this process, to close them all.
	
	12. Run the MSIX file and `Install` it.
</details>
