<div align="center">

  <h1><code>@dissonity/hirpc-interface</code></h1>

  <strong>A hiRPC implementation for Unity WebGL.</strong>

  <p>
    <img src="https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2FFurnyr%2FDissonity%2Frefs%2Fheads%2Fdev%2Fhirpc-interface%2Fpackage.json&query=version&prefix=v&label=version&color=red" alt="Version Badge" />
  </p>

  <h3>
    <a href="https://github.com/Furnyr/Dissonity/issues">Issues</a>
    <span> | </span>
    <a href="https://github.com/users/Furnyr/projects/2">Board</a>
  </h3>
</div>

## About

Unity plugin and loader for interacting with the Dissonity hiRPC within Unity games.

## Testing

You may need to install [pnpm](https://pnpm.io), [Rust](https://www.rust-lang.org/learn/get-started) or [wasm-pack](https://rustwasm.github.io/wasm-pack/installer/).

### 🦀 Build hiRPC code
```
cd ../hirpc
wasm-pack build --target web
```

### 🛠️ Build to JavaScript
```
cd ../hirpc-interface
pnpm build
```

## Production

- The Unity plugin is `build/plugin.js`
- The loader script is `build/app_loader.js`

## License

Licensed under the Apache License, Version 2.0