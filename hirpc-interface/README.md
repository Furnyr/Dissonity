<div align="center">

  <h1><code>@dissonity/hirpc-interface</code></h1>

  <strong>A hiRPC bridge for Unity WebGL.</strong>

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

> [!WARNING]
This package is a work-in-progress and doesn't work yet

Bridge implementation for interacting with the Dissonity hiRPC within Unity games.

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

The file included in the game build is `build/interface.js`.

## License

Licensed under the Apache License, Version 2.0