<div align="center">

  <h1><code>@dissonity/utils</code></h1>

  <strong>Utility files for hiRPC, implementing part of the Embedded App SDK.</strong>

  <p>
    <img src="https://img.shields.io/badge/version-v0.1.0-red" alt="Version Badge" />
  </p>

  <h3>
    <a href="https://github.com/Furnyr/Dissonity/issues">Issues</a>
    <span> | </span>
    <a href="https://github.com/users/Furnyr/projects/2">Board</a>
  </h3>
</div>

## About

This package serves two functions:

- Passing the configuration from the game engine to the hiRPC
- Calling JS functions maintained by Discord

## Scripts

You may need to install [pnpm](https://pnpm.io).

### 🛠️ Build utility files into `../hirpc/pkg`
```
pnpm build
```

### ⚙️ Add build variables for testing
```
pnpm variables
```

## Production

Build the utility files and don't update the variables, the files that must be included are documented in the hiRPC folder.

## Moving the files to Dissonity (Unity package)

You can move the resources (hiRPC, interface and utils) to the Unity folder once you are done working on them.

```
pnpm bundle
```

## License

Dissonity is licensed under the Apache License, Version 2.0

Discord's Embedded App SDK is licensed under the MIT License