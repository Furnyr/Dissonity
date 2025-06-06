
# Contributing to Dissonity

Hi! First of all, thank you for showing interest in the project! This document will guide you on how to contribute to Dissonity.

You don't need to write code to contribute — testing or helping with documentation are examples of tasks that are also very appreciated!

## Tools used in the project

- [pnpm](https://pnpm.io) - Package manager
- [Yalc](https://www.npmjs.com/package/yalc) - Testing npm packages locally
- [Doxygen](https://www.doxygen.nl) - Generating documentation for the C# API
- [Act](https://github.com/nektos/act) (and Docker) - Testing workflows locally

## Issues

If you find a bug, open an issue using the "\[v2\] Bug report" template. You may also open an issue for:

- Feature requests
- Change suggestions
- Feedback
- Typos

If you want to change a fundamental part of the project, please open an issue to discuss it before opening a pull request.

## Pull requests

It's recommended to open an issue before submiting a pull request, but you can open one directly if you are sure about your changes.

After opening a pull request, a maintainer will review the code and may request new changes. When working on a PR:

- Only add commits related to the PR
- Ask for help if you feel lost
- Request a new review once the new changes are implemented

We recommend using [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/).

<table>
  <tr>
    <th>Module</th>
    <th>Description</th>
    <th>Status</th>
    <th>Allows contributions?</th>
    <th>When will it allow contributions?</th>
  </tr>
  <tr>
    <td>/unity</td>
    <td>C# Unity package.</td>
    <td>Unlikely to undergo major unexpected changes.</td>
    <td>✅</td>
    <td></td>
  </tr>
  <tr>
    <td>/unity/Editor/Assets/Template</td>
    <td>WebGL Template used to run the hiRPC interface before the game build.</td>
    <td>Unlikely to undergo major unexpected changes.</td>
    <td>✅</td>
    <td></td>
  </tr>
  <tr>
    <td>/hirpc</td>
    <td>Underlying module that interacts with the Discord RPC.</td>
    <td>Unlikely to undergo major unexpected changes.</td>
    <td>✅</td>
    <td></td>
  </tr>
  <tr>
    <td>/hirpc-interface</td>
    <td>hiRPC implementation for Unity.</td>
    <td>Unlikely to undergo major unexpected changes.</td>
    <td>✅</td>
    <td></td>
  </tr>
  <tr>
    <td>/hirpc-kit</td>
    <td>hiRPC utilities for JavaScript.</td>
    <td>Unlikely to undergo major unexpected changes.</td>
    <td>✅</td>
    <td></td>
  </tr>
  <tr>
    <td>/website</td>
    <td>Site that hosts documentation and guides.</td>
    <td>Contributions are accepted.</td>
    <td>✅</td>
    <td></td>
  </tr>
</table>

## Documentation

### Wanted Changes

1. Fixes to incorrect or outdated statements in the documentation
2. Fixing grammatical errors
3. Rewording to clarify complicated explanations

### Unwanted Changes

1. Subjective formatting changes
2. Modifications to the overall structure of the documentation
3. Additions that document private or unreleased functionality

---

## Specific maintenance for each module

### Unity package

Dissonity is a strongly typed implementation of a weakly typed API, meaning Discord can make slight variations of a data structure and call it the same way.

While developing the C# package we don't have as much information as Discord; we simply mirror the official package. Therefore, we need to make careful decisions about the package:

- Mysterious or spontaneous fields without documentation can be excluded.

- Notable structure variations can be handled by creating multiple models (e.g., `GuildMember`, `GuildMemberRpc`, `User`, `Participant`, etc.)

#### RPC Commands

- Add command models to Dissonity.Commands
- Update CommandUtility
- Add command to Api.Commands
- Add mock response to Api.MockSendCommand

#### RPC Events

- Add event models to Dissonity.Events
- Update EventUtility
- Add event to Api.Subscribe
- Add event to mock

#### Other

- Check if it needs initialization / hiRPC ready
- Check if it needs a mock implementation
- Check if new models are needed

After updating the C# API, the generated [Doxygen reference](https://www.doxygen.nl) on the website should be automatically updated via the deployment workflow.

Bump the package.json version as required. Lastly, the update dialog should be updated to reflect the changes.

### hiRPC

- Check if functionality should require access to the hash
- Bump the SDK_VERSION constant if required
- Bump package.json version
- Write tests if needed
- Run pnpm build and pnpm move

### hiRPC Interface

- Build hiRPC
- Bump package.json version
- Run pnpm build and pnpm move

### hiRPC Kit

- Build hiRPC
- Bump package.json version
- Test locally using [Yalc](https://www.npmjs.com/package/yalc)

---

## Who are working on this project?

The [Core Team](https://dissonity.dev/about) consists of two people, but the community is also an important part of the development process.

## Acknowledgements

If you contribute to the project, you could be added to a contributor list as a thank you.
