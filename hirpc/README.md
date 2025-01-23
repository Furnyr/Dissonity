<div align="center">

  <h1><code>@dissonity/hirpc</code></h1>

  <strong>Discord RPC library for activities made in game engines.</strong>

  <p>
    <img src="https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2FFurnyr%2FDissonity%2Frefs%2Fheads%2Fdev%2Fhirpc%2Fpackage.json&query=version&prefix=v&label=version&color=yellow" alt="Version Badge" />
    <img src="https://img.shields.io/github/actions/workflow/status/Furnyr/Dissonity/hirpc.yaml">
  </p>

  <h3>
    <a href="https://github.com/Furnyr/Dissonity/issues">Issues</a>
    <span> | </span>
    <a href="https://github.com/users/Furnyr/projects/2">Board</a>
  </h3>
</div>

## About

This library can be included in the game build files. Allows communication between Discord, the JavaScript layer and your embedded application.

- Creates a connection with the Discord RPC
- Passes messages to the game build
- Exposes APIs to the JavaScript layer securely

## Developing

You may need to install [pnpm](https://pnpm.io).

### 1. 🛠️ Build the library
```
pnpm build
```

### 2. 🧩 Add your build variables
```
pnpm variables
```

### 3. 🔌 Run the test server in `./www`

###### Test exposed functions in `./www/src/client`

```
cd ./www
pnpm start
```

### 4. 🔬 Use a tunnel to test inside the Discord client

If you're using [cloudflared](https://developers.cloudflare.com/cloudflare-one/connections/connect-networks/get-started/create-local-tunnel/#1-download-and-install-cloudflared):
```
cloudflared tunnel --url http://localhost:3000
```

## Continuous Integration

Test the workflows locally with [Act](https://github.com/nektos/act) and Docker.

```
cd ..
act -j 'hirpc_tests'
```

## Production

The files included in the game build are:

- `dist/dissonity_hirpc.js`
- `dist/dissonity_build_variables.js`
- `dist/version.json`

## License

Dissonity is licensed under the Apache License, Version 2.0

This project includes code from the [Discord Embedded App SDK](https://github.com/discord/embedded-app-sdk), licensed under the [MIT License](MIT_LICENSE.md).