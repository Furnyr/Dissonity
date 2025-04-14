<div align="center">
	<br />
	<p>
		<a><img src="https://i.imgur.com/5elvldR.png" width="500"/></a>
	</p>
	<br />
</div>

# Who is this package for?
This package is a fork of the original [Dissonity](https://www.npmjs.com/package/dissonity) package. This particular package been created to be used with [Snapser](https://snapser.com).
If you are not building your game on Snapser, please use the original package [link](https://www.npmjs.com/package/dissonity).

# Snapser
Snapser is a modern alternative to Playfab and Firebase. We have built a starter project in Unity C# which you can deploy as a Discord activity within a manner of minutes, while having full access to Snapser backend services. If you are interested to know more, please [register](https://snapser.com/register) for a Snapser account.

# About

Dissonity allows you to easily create Discord activities with Unity. In version 1, the npm and Unity packages share data to provide functionality.

It's designed for a structure similar to the [nested-messages](https://github.com/discord/embedded-app-sdk/tree/main/examples/nested-messages) example from Discord. You may want to familiarize with that project structure first.

# Installation

```
npm install snapser-dissonity
yarn add snapser-dissonity
pnpm add snapser-dissonity
```

# Configuration

When running the activity, your game build will be inside a nested iframe that we will call "child".
The child iframe must have the id "dissonity-child", like:
```html
<iframe id="dissonity-child" src=".proxy/nested/index.html"></iframe>
```
Instead of manually creating an SDK instance, call `setupSdk` inside the parent index.js with your options:

```js
import { setupSdk } from "snapser-dissonity";

window.addEventListener("DOMContentLoaded", () => {

  setupSdk({
    clientId: /*your-app-id*/,
    tokenRoute: "/v1/auth/discord/login",
    method: "PUT",
  });
});
```

Where `tokenRoute` is the route where your server is handling authorization codes and sending back an access token in a POST request:

### Request
```js
{ code: string, create_user: bool, access_token: string }
```

### Expected response
```js
{ access_token: string, user: UserObject }
```

### Example code

```js
app.post("/api/token", async (req, res) => {

  const code = req.body.code;

  (...)

  res.send({ access_token, user });
});
```

That's all the configuration you need inside Node.js! Now, in your Unity project install the [Dissonity Unity package](https://github.com/snapser-community/Dissonity/tree/main/unity#readme).

> As a note, there's an already configured Node.js project in the GitHub repository.

# Links

- [GitHub](https://github.com/snapser-community/Dissonity)
- [NPM Package Source](https://github.com/snapser-community/Dissonity/tree/main/npm)
- [Unity Package](https://github.com/snapser-community/Dissonity/tree/main/unity#readme)
- [Unity Package Documentation](https://github.com/snapser-community/Dissonity/blob/main/unity/Documentation~/Dissonity.md)
- [Examples](https://github.com/snapser-community/Dissonity/tree/main/examples)