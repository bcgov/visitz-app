# Visitz

## Architecture

Hesita is written using .NET MAUI 7 (.NET 7).

## Coding & Repository Conventions

Follow the [official C# coding conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

Then follow:

1. Line length **must** end/break at 120 characters.
    
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
        
4. Use `static readonly` instead of `const` for global constant values.

    * [What is the difference between const and static in C#?](https://stackoverflow.com/a/2512962)

5. App secrets & environment settings *must not* be committed. Use configuration files and/or environment variables instead.

    * See [.NET MAUI appsettings.json](https://montemagno.com/dotnet-maui-appsettings-json-configuration/)

6. When merging Pull Requests it is recommended to use the *Rebase and Merge* option. You *must not* use the *Squash and Merge* option.

    * When you *Rebase* a Pull Request it is not necessary to merge the main branch into your working branch to "update" it. Rebasing inherently handles this for you. If you *do* merge the main branch into your working branch and try to rebase you will get a merge conflict.

    * When you *Squash* a Pull Request, you lose all meaningful information attached to all the commits from that branch. It combines the contents of all commits into one commit and discards the commits' metadata: who wrote it, the timestamp, commit ID, comments the author wrote for context.

7. Before merging a branch into one of the mainlines (dev, test, prod) you *must* remove all instances of commented code.

    * Temporarily disabling code: remove it.
        
        * If you're temporarily disabling code then this isn't something that should be merged into the main branches.

    * Removing code but keeping it in comments for historical purposes: remove it.
        
        * We use Git to track historical changes.
