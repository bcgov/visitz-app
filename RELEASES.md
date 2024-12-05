# Releasing the app

1. Make sure all working branches that we want to release have been merged into the desired `dev/` branch.

1. Make sure all unit tests pass in the desired `dev/` branch.

1. Make and complete a Pull Request to merge `dev/` into `test/`.

1. Create a regression build from the `test/` branch and ensure the build passes all regression tests.

	- If it does not pass, create working branch from `test/`, resolve issues, and create a new regression build for testing again.

1. If regression testing passes, make and complete a Pull Request to merge `test/` into `prod`. Name the Pull Request "Version Major.Minor.Patch", e.g. "Version 2.4.0"

1. Create desired builds (beta/prod) from the `prod` branch and deliver artifacts to whichever team will handle deployment.

1. Create git tags for each build type on the merge commit that was created when merging the Pull Request into `prod`.

	- In the form: `Major.Minor.Patch`, e.g. `2.4.0`, `3.1.10`, etc.

	- Include `-beta` subscript if releasing a beta build, e.g. `2.4.0-beta`, `3.1.10-beta`, etc.

	- Making a prod and beta build on the same commit means that commit will have two tags on it.

1. Make a GitHub release that links to the tags but do not include any artifacts in it.

	- When making the GitHub release, use GitHub's auto-generate release notes feature.

1. Decide on the next version number and create the next `dev/` branch

	- We use (Semver)[https://semver.org/spec/v2.0.0.html] (Major.Minor.Patch).