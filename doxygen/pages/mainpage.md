@mainpage %Dissonity

%Dissonity's Unity SDK focuses on implementing Unity APIs with Discord's Embedded App SDK. The SDK is a C# library ready to be installed as a Unity package.
Internally, it uses a variant of the Embedded App SDK — hiRPC — that is specifically designed for interaction with game-engine-produced builds.

The SDK provides APIs to interact with the Discord client from within an embedded app, but also an integrated environment to test the application outside of Discord.

<h2>Static API</h2>

The entrypoint isn't an instance, but rather a static class. By accessing Dissonity.Api, developers will be able to use SDK functionality.

Dissonity.Api.Initialize must be called once before accessing functionality from the API class, unless specified otherwise. It is possible to wait for initialization through multiple scripts using Dissonity.Api.OnReady.

<h2>Functionality</h2>

%Dissonity provides all the functionality included in the Embedded App SDK and:

- A mock environment within Unity
- Integrated JavaScript/C# interoperation APIs (Dissonity.Api.HiRpc)
- Support for non-Node.js servers
- Unity utilities (e.g., Dissonity.Api.Proxy, Dissonity.Utils)

The SDK also offers Quality of Life features for Unity developers, like simple configuration or ease of supporting other platforms (e.g., Dissonity.SdkConfiguration, Dissonity.OutsideDiscordException).