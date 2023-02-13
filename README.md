# mcfd-mobility

This is the main repository for the MCFD Mobility mobile app solution.

Internally, the app is referred to as **Hestia**. You can find the app code in the */hestia* directory.

## Architecture

Hesita is written using .NET MAUI 7 (.NET 7).

## Coding Conventions

Follow the [official C# coding conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

Then follow:

1. Line length **must** end/break at 120 characters.
    
    * If possible, set this in your text editor ([VSCode](https://stackoverflow.com/a/60060509))
        
    * Try to break lines at 80-100 characters if it visually makes sense, but don't force it.
    
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

## Licence

Copyright 2019 Province of British Columbia

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
