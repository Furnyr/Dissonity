<div align="center">

  <h1><code>@dissonity/hirpc-interface</code></h1>

  <strong>A hiRPC implementation for Unity WebGL.</strong>

  <p>
    <img src="https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2FFurnyr%2FDissonity%2Frefs%2Fheads%2Fdev%2Fhirpc-interface%2Fpackage.json&query=version&prefix=v&label=version&color=yellow" alt="Version Badge" />
  </p>

  <h3>
    <a href="https://github.com/Furnyr/Dissonity/issues">Issues</a>
    <span> | </span>
    <a href="https://github.com/users/Furnyr/projects/2">Board</a>
  </h3>
</div>

## About

Unity plugin and loader for interacting with the Dissonity hiRPC within Unity games.

## Developing

You may need to install [pnpm](https://pnpm.io).

### 📘 Build hiRPC code
```
cd ../hirpc
pnpm build
```

### 🛠️ Build hiRPC interface
```
cd ../hirpc-interface
pnpm build
```

## Production

- The Unity plugin is `dist/plugin.js`
- The loader script is `dist/app_loader.js`

## License

Dissonity is licensed under the Apache License, Version 2.0

This project includes code from the [Discord Embedded App SDK](https://github.com/discord/embedded-app-sdk), licensed under the [MIT License](MIT_LICENSE.md).