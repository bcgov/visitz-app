# visitz-app

Repository for the mobile app solution designed for workers in CFD.

Internally, the app is referred to as **Visitz**. You can find the app code  in the */visitz* directory.

## Project Structure

Visitz is built using several C# projects:

- `Visitz`: The MAUI implementation of the app.
- `VisitzApi`: Wrapper for interacting with API integrations.
- `VisitzModel`: The bulk of the business-logic.
- `Oidc`: OIDC authentication implemenation for MAUI.

Except for `Visitz`, each project has its own xUnit testing project associated with it.

## Making release builds

GitHub Actions are used to make release builds for desired platform targets. Refer to the [workflows README](.github/workflows/README.md) for information and instructions on how to run them.

## API

Implementation of supporting APIs the app connects to can be found at [visitz-api](https://github.com/bcgov/visitz-api) and [mcfd-mobility-webmethods-passthru](https://github.com/bcgov/mcfd-mobility-webmethods-passthru).

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
