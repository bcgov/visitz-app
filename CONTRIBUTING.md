# Contributing

## Pull Requests

Changes to source code must go through a Pull Request.

### Making a PR

1. Commit your work in a working branch

    We don't require a naming convention for working branches as long as they don't conflict with any mainline branch (`dev/*`, `prod`, etc.) and the branch name is relatively descriptive.

1. If your working branch is out of date with `dev/*`, **rebase** it onto the latest dev branch and resolve any merge conflicts. **Do not** pull and merge `dev/*` into your working branch.

1. Create a PR for your branch

 	Set the PR base to the latest `dev/*` branch

1. In the PR description, include a hyperlink to the story/ticket related to this work

	e.g. `[STRY00000](<URL to story/ticket>)`

## App secrets & environment settings

App secrets & environment app settings *must not* be committed. Use configuration files and/or environment variables instead. *Pull Requests containing info that must not be committed will be* ***rejected***.

See [.NET MAUI appsettings.json](https://montemagno.com/dotnet-maui-appsettings-json-configuration/)

## iOS User-specific csproj configurations

Visual Studio will save settings for manual provisioning profiles for iOS in Visitz.csproj. This isn't shareable, so move the settings that Visual Studio creates into Visitz.csproj.user instead. It will continue to function correctly and will be ignored by Git.

If your Visitz.csproj.user file gets corrupted, just re-do the setup in Visitz.csproj and move it over again.

## If your PR is rejected because of secret or environment config

Before making a new PR, you must:

1. Close the current PR (if it isn't already)

2. Delete your working branch from remote

3. Modify your local commit history to remove the offending content

	- [git rebase](https://git-scm.com/docs/git-rebase) can be used to achieve this

	- !! It's not enough to just delete the content and commit or use `git revert`. The content will be removed but only from that commit onwards—the offending content will still exist in the commit history. A rebase is necessary to actually remove it.

4. Push the modified branch to remote
  
You can then create a new PR with the corrected changes.

## Keep working branch up-to-date

To keep your working branch up-to-date, rebase it onto remote branches instead of pulling remote branches and merging into your working branch.

> This keeps the commit history cleaner, and pull requests won't be littered with changes unrelated to your new work.

## Commented code

Before merging a branch into one of the mainlines (dev, test, prod) you *must* remove all instances of commented code.

- If you're temporarily disabling code: either remove it or add a feature flag.
        
	> If you're temporarily disabling code then this isn't something that should be merged into the main branches.

- If it's for some kind of A/B testing, then enabling/disabling certain features should be handled by feature flags in code instead of manually building different snapshots with commented code.

- If you are removing code but keeping it in comments for historical purposes: remove it from the main codebase and add it to the `archive` folder with an explanation of why this code is being kept.

# Coding Conventions

Follow the [official C# coding conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

Then follow:

1. C# Line length **must** end/break at 120 characters.
    
    * If possible, set this in your text editor ([VSCode](https://stackoverflow.com/a/60060509))
        
    * Try to break lines starting from 80 characters if it visually makes sense, but don't force it.
    
2. C# regions **must not** be used for any reason.
    
    * More info: [Why are people so strongly opposed to #region tags in methods?](https://softwareengineering.stackexchange.com/a/118834)
    
    * If a function is becoming unwieldy and large, consider [refactoring it into private functions](http://wiki.c2.com/?HeadlinesTechnique) that complete single tasks with descriptive names.

3. Ternary statements **must not** be nested. 

    * Instead, refactor the surrounding code to work without a nested ternary statement (like an if/else-if/else block or a self-descriptive private function).
    
    * If you're writing a long ternary statement, consider breaking them up with whitespace and newlines:
    
        ```C#
        return someBoolValue
            ? BigStringUtilities.DoSomeLargeAmountsOfProcessing(stringVal)
            : SmallerStringUtilities.TrimSomeWhitespace(stringVal)
        ```
        
4. Use `static readonly` instead of `const` for public global constant values that will be shared between classes.

    * [What is the difference between const and static in C#?](https://stackoverflow.com/a/2512962)
