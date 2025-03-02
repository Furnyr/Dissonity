
# Contributing to Dissonity

Hi! First of all, thank you for showing interest in the project! This document will guide you on how to contribute to Dissonity.

You don't need to write code to contribute — testing is also very appreciated! (See the Issues tab).

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

---

It's important to keep in mind that version 2 is currently in beta, so the codebase is still prone to changes.

We try to keep the [Wiki documentation](https://github.com/Furnyr/Dissonity/wiki) up to date for contributors, although the more experimental features are not so well documented:

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
    <td>/examples</td>
    <td>Code samples and basic backend.</td>
    <td>Contributions are accepted. Open an issue to discuss new examples.</td>
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
  <tr>
</table>

---

## Who are working on this project?

It's mainly [Nyrrren](https://github.com/Furnyr) (me) and Kistar, the main tester. We are still experimenting with what we think is right for the project. You can submit feedback any time through the Issues tab.

## What is being worked on currently?

The [v2 board](https://github.com/users/Furnyr/projects/2) is updated frequently with what we are working on.

In the long term, it's planned to collaborate with [Robo.js](https://github.com/Wave-Play/robo.js) to offer a simple hosting process, but we'll see over time.

## What will happen after the alpha?

When version 2 is stable, it will enter the **beta** phase, which could last for a few months. During that time, users of version 1 should reinstall the Unity package from the v1 branch.

Then, the dev branch will be merged into main and version 1 will be deprecated in 3 months.

## Acknowledgements

If you contribute to the project, you could be added to a contributor list as a thank you.
