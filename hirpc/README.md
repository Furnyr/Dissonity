<div align="center">

  <h1><code>@dissonity/hirpc</code></h1>

  <strong>A Rust and WebAssembly library for the Discord RPC within game engines.</strong>

  <p>
    <img src="https://img.shields.io/badge/version-v0.2.0-red" alt="Version Badge" />
  </p>

  <h3>
    <a href="https://github.com/Furnyr/Dissonity/issues">Issues</a>
    <span> | </span>
    <a href="https://github.com/users/Furnyr/projects/2">Board</a>
  </h3>
</div>

## About

- Creates a connection with the Discord RPC
- Passes messages to the game build
- Exposes APIs to the JavaScript level securely

## Testing

You may need to install [pnpm](https://pnpm.io), [Rust](https://www.rust-lang.org/learn/get-started) or [wasm-pack](https://rustwasm.github.io/wasm-pack/installer/).

### 1. 🛠️ Build the JavaScript utils in `../utils`
```
cd ../utils
pnpm build
```

### 2. 🧩 Add your build variables
```
pnpm variables
```

### 3. 🦀 Build the Rust code
```
cd ../hirpc
wasm-pack build --target web
```

### 4. 🔌 Run the test server in `./www`

###### Test exposed functions in `./www/src/client`

```
cd ./www
pnpm start
```

### 5. 🔬 Use a tunnel to test inside the Discord client

If you're using [cloudflared](https://developers.cloudflare.com/cloudflare-one/connections/connect-networks/get-started/create-local-tunnel/#1-download-and-install-cloudflared):
```
cloudflared tunnel --url http://localhost:3000
```

## Continuous Integration

Test the workflows locally with [Act](https://github.com/nektos/act) and Docker.

```
cd ..
act -j 'hirpc_tests' -P ubuntu-latest=ghcr.io/catthehacker/ubuntu:rust-latest
```

## Production

The files included in the game build are:

- `pkg/dissonity_hirpc.js`
- `pkg/dissonity_hirpc_bg.wasm`
- `pkg/build_variables.js`
- `pkg/snippets`

## License

Licensed under the Apache License, Version 2.0