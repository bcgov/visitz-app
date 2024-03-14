1. App secrets & environment app settings *must not* be committed. Use configuration files and/or environment variables instead.

    * See [.NET MAUI appsettings.json](https://montemagno.com/dotnet-maui-appsettings-json-configuration/)

	*Pull Requests containing info that must not be committed will be* ***rejected***.

    Before making a new PR, you must:

    1. Close the PR (if it isn't already)

    2. Delete your working branch from remote

    3. Modify your local commit history to remove the offending content

        - [git rebase](https://git-scm.com/docs/git-rebase) can be used to achieve this

        - It's not enough to just delete the content and commit or use `git revert`. The content will be removed but only from that commit onwards—the offending content will still exist in the commit history. A rebase is necessary to actually remove it.

    4. Push the modified branch to remote and create a new PR

2. Don't commit environment-specific setup to the repository:

    - User-specific Visual Studio configurations

        - Visual Studio will save settings for manual provisioning profiles for iOS in Visitz.csproj. This isn't shareable, so move the settings that Visual Studio creates into Visitz.csproj.user instead. It will continue to function correctly and will be ignored by Git.

            - If your Visitz.csproj.user file gets corrupted, just re-do the setup in Visitz.csproj and move it over again.

3. Don't merge remote branches into your feature branch in order to keep them up to date. Rebase your feature branch onto remote branches instead.

	- This keeps the commit history cleaner, and pull requests won't be littered with changes unrelated to your new work.

	- Unless there's a good reason, avoid the *Rebase and Merge* option when merging a Pull Request. It's recommended to rebase your feature branch, push, then run a normal merge.

4. Before merging a branch into one of the mainlines (dev, test, prod) you *must* remove all instances of commented code.

    * Temporarily disabling code: remove it.
        
        * If you're temporarily disabling code then this isn't something that should be merged into the main branches.

        * If it's for some kind of A/B testing, then enabling/disabling certain features should be handled by actual code instead of manually building different snapshots with commented code.

    * Removing code but keeping it in comments for historical purposes: remove it.
        
        * We use Git to track historical changes.

## Coding Conventions

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
    
    * Ternary statements that are not nested are fine—but if they're long, consider breaking them up with whitespace:
    
        ```C#
        return someBoolValue
            ? BigStringUtilities.DoSomeLargeAmountsOfProcessing(stringVal)
            : SmallerStringUtilities.TrimSomeWhitespace(stringVal)
        ```
        
4. Use `static readonly` instead of `const` for public global constant values.

    * [What is the difference between const and static in C#?](https://stackoverflow.com/a/2512962)
