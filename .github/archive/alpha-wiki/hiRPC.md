# 🔌 hiRPC 🔌

Dissonity needs to start authenticating with Discord before the Unity app loads for faster loading times. Since once of the main goals of version 2 is to be independent of Node.js, we cannot use the official Embedded App SDK for this purpose. We need some kind of portable files that can be served along the game build.

But this creates another problem: we abstract all functionality from the JavaScript level. This means that the only way to receive or send messages to Discord is from Unity.

This is where hiRPC comes in. It is a module that allows interoperability in Dissonity projects. 

## How does it work?

The module is imported when the activity starts and resides in `globalThis.dso_hirpc`.

The idea is that before the Unity index.html loads, JavaScript code served with the activity requests a **hash** from the hiRPC. Then, the Unity index.html is loaded as an iframe and the hash generation is locked.

Functionality is restricted by using SHA256 hashes as keys that must be passed to methods. These hashes aren't impossible to crack and you shouldn't trust data coming from the client as usual, but it does make the activity much harder to tamper.

## How do I use it?

It's currently used internally, the public API documentation will be published once hiRPC is no longer experimental.