# MCFD Mobility GitHub Actions

## App release builds

### Android

Nothing yet.

### iOS

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

3. Choose GitHub environment to use

|	Audience					|	GH Environment	|	Loginproxy	|	Visitz API	|	Debug options	|
|	---							|	---				|	---			|	---			|	---				|
|	Developers					|	developer		|	dev			|	dev			|	**enabled**		|
|	MCFD Mobility project team	|	project-team	|	test		|	test		|	**enabled**		|
|	Early adopters				|	beta			|	prod		|	prod		|	*disabled*		|
|	General users				|	prod			|	prod		|	prod		|	*disabled*		|

4. Run workflow

5. On success, manually distribute GH Action artifacts as required

> The `developer` environment is mainly available for developers to grab environment configs for their local builds. It will rarely be used to make builds via this GH Action.

### Mac

Nothing yet.

### Windows

Nothing yet.