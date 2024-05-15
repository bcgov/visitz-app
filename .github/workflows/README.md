# MCFD Mobility GitHub Actions

## Github Environments

We keep a set of main deployment environments:

|	Audience					|	GH Environment	|	Loginproxy	|	Visitz API	|	Debug options	|
|	---							|	---				|	---			|	---			|	---				|
|	Developers					|	developer		|	dev			|	dev			|	**enabled**		|
|	MCFD Mobility project team	|	project-team	|	dev			|	dev			|	**enabled**		|
|	Early adopters				|	beta			|	prod		|	prod		|	*disabled*		|
|	General users				|	prod			|	prod		|	prod		|	*disabled*		|

Notes:

1. The `developer` environment is mainly available for developers to grab environment configs for their local builds. It will rarely be used to make builds via this GH Action.

2. Any existing environment that isn't listed in this table is transitory.

## Tests

On push, if any files in the `visitz/` directory have changed, an Action will be triggered that runs all tests for the Visitz MAUI project.

The production branch requires all tests succeed before allowing a PR merge.

## App release builds

### Android

Nothing yet.

### iOS

#### Update secrets

- ##### IOS_BUILD_CERTIFICATE_NAME

	The name of the distribution certificate associated with the IOS_BUILD_PROVISION_PROFILE_BASE64 provisioning profile.

	It should be wrapped in quotes when saved in secrets, e.g.

	> "iPhone Distribution: Some person's distribution cert name"

- ##### IOS_BUILD_PROVISION_PROFILE_BASE64

	A base64-encoded copy of the distribution provisioning profile.

	To encode on OSX:

	> `base64 -i <provisioning profile name>.mobileprovision | pbcopy`

	`pbcopy` takes output and sets it to the clipboard. Then directly paste into secrets **without** quotes.

- ##### IOS_CODE_SIGN_PROVISION_PROFILE_NAME

	The name of the distribution provisioning profile.

	It should be wrapped in quotes when saved in secrets, e.g.

	> "Provisioning Profile - Name"

#### Build

> **Warning**: as of 2023-11-16 (early Version 2 development) the GH Action for iOS builds hardcodes its .NET and MAUI versions in the action itself instead of relying on variables. This is meant to decrease confusion in the future if anyone makes a build for an older commit.
> 
> Framework versions before this change:
>
> 	- .NET 7.0.302
> 	- .NET MAUI 7.0.86

1. Run "Build iOS release package" workflow manually

2. Choose branch to build from

	- **production** branch for production/beta builds

	- **dev\*** branches for developer or project-team builds

3. Choose a GitHub Environment to build with

4. Run workflow

5. On success, manually distribute GH Action artifacts as required

### Mac

Nothing yet.

### Windows

Nothing yet.
