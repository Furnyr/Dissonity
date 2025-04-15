
<br>

> [!WARNING]
This wiki documents version 2 for testers, which is currently in a **testing/alpha** phase. Everything here is subject to change and potentially unstable. <br> You should have some web technology knowledge before proceeding.

> [!NOTE]
> Dissonity is intended to be compatible with every version above 2021.3 LTS

# 🎮 Alpha 4 v2 Home 🎮

Dissonity v2 is more than a C# implementation of the [Discord Embedded App SDK](https://github.com/discord/embedded-app-sdk). It is an SDK designed for Unity that provides you with everything you need to easily develop a Discord activity with Unity. Dissonity handles all the communication between the Discord RPC protocol and your application. 

<br>

## 🟦 Version 1 🟦

<img align="right" alt="version 1 schema" width="500" src="https://i.imgur.com/p4Dn5vX.png"/>

Dissonity v1 was a **wrapper** around the Embedded App SDK.

Basically, the Unity package sent messages to its window parent (the build is inside a nested iframe), where the official SDK was, and the so-called bridge handled the data back and forth.

This implementation required the use of a [Node.js](https://nodejs.org/) server, as the the bridge needed to be installed server-side since the official SDK is a Node.js [package](https://www.npmjs.com/package/@discord/embedded-app-sdk).

<br><br><br><br><br><br><br><br><br><br><br><br><br>

## 🟪 Version 2 🟪

<img align="right" alt="version 2 schema" width="500" src="https://i.imgur.com/1mWaICr.png"/>

Dissonity v2 is a standalone, client-side SDK. Instead of sending messages to interact with the official SDK, it acts as the official SDK itself. Version 2 also adds functionality to mock Discord inside Unity for easier testing.

This implementation allows the use of any kind of server.

~~Version 2 also uses a bridge internally, but these simplified schemes help to understand the differences.~~

<br><br><br><br><br><br><br><br><br><br><br><br>

## How does it work?

Activities are just web pages inside iframes in the Discord client. Unity gives the option to build a game for WebGL, which allows it to run in the web. If all we wanted was to embed a Unity game into Discord we wouldn't really need anything else.

---

Now, what if we actually want to have interaction with the Discord client? Like receiving the players' names, displaying rich presence, getting the user's locale, etc. Then we need to:

1. Find a way to send messages to the Discord client
2. Implement a handshake, authorization and authentication through RPC
3. Load everything *before* the Unity build (because it would be inefficient to wait for Unity to connect)

Dissonity's answers are:

1. A system that can interact with both JavaScript and C# (hiRPC)
2. User information is added post-processing the Unity build, then the communication occurs by:
3. A custom WebGL template that allows Dissonity to load JavaScript before the Unity build

## 🧩 Terms 🧩

- RPC Protocol: Discord client protocol that uses JavaScript's [postMessage](https://discord.com/developers/docs/activities/how-activities-work) for communication
- Bridge: Component that allows communication between two or more systems
- Bridge interaction: Communication between C# and JS that doesn't get to the Discord client
- [hiRPC](https://github.com/Furnyr/Dissonity/wiki/hiRPC): Bridge that allows communication with Discord and the JS level
- hiRPC Interface: hiRPC implementation for Unity
- DissonityBridge: C# module that sends or receives data from the IframeBridge
- API or DissonityApi: Public C# interface that enables functionality to users

---

- RPC command: Message sent to the Discord client to execute an action
- RPC event: Message sent from the Discord client
- Subscription: State where Dissonity is listening for certain RPC events, usually user-initiated.
- Message bus: Structure that holds event listeners
- Mock object: Structure that holds data that simulates the Discord client inside Unity
- Mock mode: Mode where Dissonity interacts with the mock object instead of the Discord client

I think that's a good breakdown, but still, it never hurts to know more.
- https://discord.com/developers/docs/activities/overview
- https://discord.com/developers/docs/activities/development-guides#activity-development-guides

> [!TIP]
> When making a build, make sure to change the platform to WebGL and use the Dissonity WebGL template. You can also check "Development Build", this will build the game faster at the cost of slower loading within Discord.

## Testing new changes

If you were testing version 2 before something was added, you should:
- Read ALPHA_CHANGELOG.md
- Delete your Unity build files
- Delete your WebGLTemplates folder
- Delete the mock object
- Check if you need to pull the example server