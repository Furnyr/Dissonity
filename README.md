> [!IMPORTANT]
> **Version 2** is now in beta! Read the release plan [here](https://github.com/snapser-community/Dissonity/blob/dev/ROADMAP.md).
>
> TL;DR: If you installed v1 before January 2025, please reinstall the package from `https://github.com/snapser-community/Dissonity.git?path=/unity#v1`

---

<div align="center">
	<br />
	<p>
		<a><img src="https://i.imgur.com/TuawbuK.png" width="500"/></a>
	</p>
	<br />
</div>

### Dissonity allows you to build Discord activities using Unity.

Previously named Unity-Embedded-App-SDK. This project has been developed for Unity 2022 or later but it should work with all the LTS versions.

Dissonity is designed for a structure similar to the [nested-messages](https://github.com/discord/embedded-app-sdk/tree/main/examples/nested-messages) example from Discord. You may want to familiarize with that project structure first.

### Example Unity script
```cs
using UnityEngine;
using static Dissonity.Api;

public class MyScript : MonoBehaviour
{
    async void Start()
    {
        string userId = await GetUserId();
        Debug.Log($"The user's id is {userId}");

        SubActivityInstanceParticipantsUpdate((data) => {
            Debug.Log("Received a participants update!");
        });
    }
}
```

> [!NOTE]
> You can implement these packages yourself, or start with the Node.js example project inside the examples folder.

## Design

This project consists of two main packages:
- Dissonity NPM
- Dissonity Unity

To use Dissonity you need a Node.js project that uses the Dissonity NPM package and a Unity project that uses the Unity package.

In the Unity project you will have access to methods that allow you to interact with the Embedded App SDK. Behind the scenes, the SDK is still in the Node.js part. Both packages share data to provide functionality.

> When running the activity, your game build will be inside a nested iframe inside the Node.js client that we will call "child" (more on this later).
# Installation

First of all, you need to have installed:
- Node.js
- Unity 2022 or later
- Git (recommended)

## Node.js

1. Install the npm package with `npm install dissonity`
2. Inside the client's index.js, call `setupSdk` with your config ([sample](https://github.com/snapser-community/Dissonity/blob/main/npm/README.md#configuration))
3. Make sure the iframe where your build will be has the id `dissonity-child` ([sample](https://github.com/snapser-community/Dissonity/blob/main/npm/README.md#configuration))
4. Configure your server to send the proper content headers ([sample](https://github.com/snapser-community/Dissonity/blob/main/examples/projectConfiguration.md#other-server-configuration))

That's everything you need in the Node.js client.

> Notice you don't need to manually create a DiscordSDK instance.

## Unity
1. Go to `Window` > `Package Manager` > `Add package from git URL`
2. Install the package from `https://github.com/snapser-community/Dissonity.git?path=/unity#v1`
3. Right click in the hierarchy, `Dissonity` > `Discord Bridge`
4. You can now build your game for WebGL and put it in your nested iframe. If you're not using the example Node.js project you will need to follow the "Project configuration" guide below.

If you don't know how to test your activity refer to the [Discord documentation](https://discord.com/developers/docs/activities/development-guides#run-your-application-locally).

> [!IMPORTANT]
> If you don't add the Discord Bridge object to your scene you won't be able to receive data from the SDK!

# Links

- [GitHub](https://github.com/snapser-community/Dissonity)
- [NPM package](https://www.npmjs.com/package/dissonity)
- [Dissonity NPM Documentation](https://github.com/snapser-community/Dissonity/blob/main/npm/README.md)
- [Dissonity Unity Documentation](https://github.com/snapser-community/Dissonity/blob/main/unity/Documentation~/Dissonity.md)
- [Project configuration](https://github.com/snapser-community/Dissonity/blob/main/examples/projectConfiguration.md)

# Examples

There are code samples and an example Node.js project (using [Colyseus](https://github.com/colyseus/colyseus) for multiplayer support!) inside the examples folder.

If you find a bug, don't hesitate to open an issue. Now go and build something cool! :)
