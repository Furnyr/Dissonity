This file will document changes made during the alpha phase.

---

# 2025-2-9 - alpha progress 5

This alpha progress version completes the base functionality of the alpha phase. Testing is much needed currently.

## Wiki

The current wiki is still mostly up to date.

Read the `Next` section for information about the documentation.

## hiRPC

- Send hiRPC handshake outside of Discord

- Use closures to store the hashes

- Updated fail load test

## hiRPC Interface

- Add window.dso\_expand\_canvas when using Max resolution

- App hash is now stored in the C# side.

- Added ExpandCanvas function

## Dissonity (internal)

- Splitted MessageBus into DiscordMessageBus and HiRpcMessageBus

- Splitted SubscriptionReference into DiscordSubscription and HiRpcSubscription

- Added implementation to receive hiRPC messages

- Added share frame command

- Added \_hiRpcReady boolean to track when the hiRPC handshake is sent

- Added implementation to serialize unknown enum values to \<Enum\>.Unknown if possible

- Added new resources for the configuration options

- Added namespace Dissonity.Editor.Dialogs

- Added internal subscriptions when resolution is Max to expand the canvas when it's needed (this might fix Max resolution)

## Dissonity (API)

- Added dialogs for initial package installation and package updates.
- - Configuration is automatically generated using these dialogs.
- - Added three configuration options: **Basic**, **Standard** and **Advanced**.

- Added uninstaller dialog

- Added hiRPC methods in Api.HiRpc

- Added new command Api.Commands.ShareLink (and its corresponding mock implementation)

- Added comment for Api.ReferrerId and Api.CustomId

- Added Api.OnReady method

- Resolution in desktop and mobile now defaults to Max

- Fixed missing arguments in SetOrientationLockState command

- Renamed WebResolution to BrowserResolution

## Next

After alpha 5 is tested, the project will be ready to move into the **beta** phase. This would mean that the foundation of the project is stable and we're ready to start polishing.

That doesn't mean the release date is necessarily close, though.

The wiki is unlikely to receive any further updates as public documentation will be created during the beta phase.

[Read more here.](ROADMAP.md)

---

# 2025-1-23 - feat: hirpc v0.4.0

The hiRPC module is pretty much stable now. That doesn't mean that it can't undergo breaking changes until release, but that its design is final.

## hiRPC

Since this is a complete rewrite, everything has changed internally. In the API side, the most important change is that **hiRPC channels** are now managed by the module, instead of just being another payload property.

This way, you can easily filter out messages in other channels.

The `dissonity` channel is reserved for necessary communications.

## hiRPC Interface

- Implemented new hiRPC design

## hiRPC Kit

Added initial version. This package will be used on the JavaScript side to import hiRPC from the Unity build. Unlike with the Embedded App SDK, it depends on the Unity package, not the other way around.

Note: This is the "hiRPC SDK", it was renamed.

## Dissonity (internal)

- Implemented new hiRPC design

- Modified version.json (Unity package version)

- Added Bridge/version.json (hiRPC version)

## Dissonity (API)

- Removed SdkVersion

---

# 2024-12-18 - alpha progress 4

This alpha progress version is centered around migrating from RpcBridge to hiRPC and fixing a few bugs.

- A Dissonity activity can now interact with the JS level
- The "BridgeLib" term is now outdated (read the new wiki!)

## Dissonity (internal)

- Fixed some types (reported by JadenH)

- Implemented hiRPC APIs

- Mobile resolution fix

- Changed "\[Dissonity\]:" to "\[Dissonity\]" in logs

- Api.Initialize works inside Awake

- The DiscordMock is generated if you don't add it manually

- Merged all bridge public methods (that wouldn't be public if they didn't need to be) into \_HiRpcInput

- Deleted classes only meant to be used with the Discord HTTP API. Dissonity will only handle the activity RPC, use another Unity SDK along with Dissonity if you need to use the Discord API.

## Dissonity (WebGL Template)

- Added a mechanism to automatically regenerate the template if the package.json version is higher than the template version (version.json).

- The loader script is now part of the hiRPC interface. Also, I found a way to remove the errors caused by conditional compilation by wrapping the variables around strings.

- Added thumbnail.png

## Dissonity (API)

- Added 1.7.0 to 1.8.0 features from the official SDK

- Added Max resolution option

## Dissonity (Node.js project)

- Updated build script

- Removed matchmaking room (contribution by Semanual)

## CI

Use `pnpm bundle` in `./utils` to compile hiRPC, hiRPC interface and hiRPC utils and move them to the Unity folder.

## hiRPC

It still needs to be polished, but it's functional. The current documentation only explains its purpose and the way it works. The hiRPC API will be documented once it's stable.

## hiRPC Interface

Added initial version. The .jslib plugin and the loader script are now in this folder.

## Website

- Fixed resizing issues

## Next

The next thing I'm working on is polishing and adding features to hiRPC, some QoL changes and more documentation.

---

# 2024-11-24 - fix: use url overrides

This commit modifies the `index.html` main script and some "compiled" TypeScript. This is a transitory change, since it fixes code that will soon be replaced by the hiRPC.

And URL overrides don't seem to work on Discord's end either.

## Next

Small update on upcoming changes in progress:

### Confirmed

- (alpha 4) Move the main script to its own file
- (alpha 4) Support calling `Initialize` in `Awake`
- (alpha 4) Remove RpcBridge in favor of hiRPC

### Possible

- (alpha 5?) Add `SyncedParticipants`?
- (alpha 5?) Add `Max` screen resolution?

### Miscellaneous

- Write the hiRPC SDK
- Start writing documentation in the website

---

# 2024-11-21 - feat: website

- Updated README
- Added website (wip) and workflow

---

# 2024-11-17 - feat(unity): support unity 6

## Dissonity (internal)

- Replace deprecated methods

---

# 2024-11-17 - feat(hirpc, api): facade and uppercase url

## hiRPC

- Added facade, a module that holds all the unsafe code
- Added tests
- Handshake sdk version is now here

## Dissonity (API)

- Allow uppercase urls in Api.Proxy methods

---

# 2024-11-12 - fix(api/proxy): editor check and headers

## Dissonity (API)

- Allow requests to full urls inside Unity
- Use headers
- Document valid arguments

---

# 2024-11-12 - feat(api): allow full urls in proxy methods

## Dissonity (API)

- `Api.Proxy.Https<Method>Request` now allows full urls as an argument:
- - If the path is relative (/cat), the request is sent through the proxy (/.proxy/cat)
- - If the path is absolute (https://...), the request is left as is and the developer is expected to patch the url mappings.

---

# 2024-11-09 - chore(hirpc): initial commit

This progress version changes nothing from the Unity package, but adds some workflows and the work-in-progress hiRPC code.

## What is hiRPC?

It's essentially the Embedded App SDK for game engines. While most of the development has been centered around the C# side of the project, the RpcBridge was the "core" that handled the RPC connection.

But the RpcBridge failed in some aspects:

- It was possible to mess with it from the console
- Some parts of the API weren't exposed at the JavaScript level (so no compatibility with frameworks)

This new core makes it harder to access restricted functionality from the console, while exposing the API to the JavaScript level securely.

## Other differences

Another point that differentiates hiRPC with the RpcBridge is that hiRPC could theoretically be used within other game engines. The only thing that changes between engines is the JS interface used to interact with the hiRPC module.

So instead of:

- RpcBridge
- InterfaceBridge
- Official utils

Now there is:

- @dissonity/hirpc
- @dissonity/hirpc-interface
- @dissonity/utils

And a utility package to allow third parties to interact with the hiRPC:

- @dissonity/hirpc-sdk

Which seems like the right direction for the project, but I'll keep experimenting and see if it's right. The end goal is not depending on Node.js for development inside Unity.

---

# 2024-11-02 - fix: long snowflakes in subscriptions

## Dissonity (API)

- Fixed snowflake type in some subscription methods

---

# 2024-10-30 - allow for null accent color

Authored by JadenH.

## Dissonity (API)

- Fixed User and AuthenticatedUser `AccentColor` type

---

# 2024-10-25 - feat: mirror v1.6.1

Staying up to date with the official SDK.

## Wiki

- Renamed Permission to [PermissionFlags](https://github.com/Furnyr/Dissonity/wiki/Namespaces#dissonitymodels)

## Dissonity (API)

- Updated Api.SdkVersion to 1.6.1

- Added every permission flag

## Note

I made a [project](https://github.com/users/Furnyr/projects/2) to keep track of the remaining tasks.

---

# 2024-10-19 - alpha progress 3

This alpha progress version is centered on QoL changes, internal overhauls, support for app resolution config and other fixes.

## Wiki

- Removed IframeBridge from [Home](https://github.com/Furnyr/Dissonity/wiki) page
- Added [InterfaceBridge](https://github.com/Furnyr/Dissonity/wiki#-terms-) documentation
- Added SyncedUser and SyncedGuildMember to [API properties](https://github.com/Furnyr/Dissonity/wiki/Design#api-properties)
- Added [Structure synchronization](https://github.com/Furnyr/Dissonity/wiki/Design#%EF%B8%8F-configuration-%EF%B8%8F) documentation
- Added [new unsubscribe method](https://github.com/Furnyr/Dissonity/wiki/Design#apisubscribe--apiunsubscribe)
- Updated [Long or string?](https://github.com/Furnyr/Dissonity/wiki/Design#long-or-string) documentation
- Removed references to previously-unrelased features
- Updated [Namespaces](https://github.com/Furnyr/Dissonity/wiki/Namespaces/_compare/2152fa277eee456422bd6d9ae4868ffaa7dfa48d...b6d15e7239168ec019ba0db9ad592ef342b8d3ef)
- Updated [Plan for the future](https://github.com/Furnyr/Dissonity/wiki/Plan-for-the-future-(v2)) to reflect the latest changes
- Updated [Utils](https://github.com/Furnyr/Dissonity/wiki/Utils) to use long for snowflakes

## BridgeLib

> *Goodbye again, IframeBridge!*

- Removed IframeBridge in favor of InterfaceBridge

- Use ES6 modules to not expose the Unity instance directly

- Support web activities

- Improved "outside Discord" check

## Dissonity (internal)

- Fix reset mock clear button

- Added underscores to mock properties

- Added screen resolution config

- Changed snowflakes from strings to longs

- Added internal subscriptions

- Build backgrounds are now completely supported

- Removed MockDictionary and other obsolete classes

## Dissonity (API)

- Added Api.SyncedUser and Api.SyncedGuildMemberRpc

- Added more models

- Added tooltips to API and configuration properties

- Added headers to proxy methods

- Changed the return value to some commands and events (arrays, enums and classes are returned directly)

- Added SynchronizeUser and SynchronizeGuildMemberRpc to the Dissonity configuration

- Added screen orientation configuration

## Testing notes

Main things to test:
- New command return values
- New event return values
- Unsubscribe via listener method
- All snowflakes ids must be longs
- Screen orientation and web support
- Structure synchronization
- User subscriptions shouldn't mess with structure synchronization

Secondary things to test:
- Proxy methods
- Initial server request/response

## Other

Version 2 will be licensed under the Apache License 2.0

## Next

With the latest overhaul, the API is closer to be stable. Still not quite there though.

The next thing I'll be doing is testing, doing research to improve compatibility and maybe setting up unit testing.

---

# 2024-10-09 - alpha progress 2

This alpha progress version is centered on supporting the previously-unreleased features, improvements to the mock object, some conventions and polishing the C# <-> JS bridge.

## Wiki

- Added [Ready](https://github.com/Furnyr/Dissonity/wiki/Design#api-properties) documentation
- Added [ProxyDomain](https://github.com/Furnyr/Dissonity/wiki/Design#api-properties) documentation
- Updated [Testing new changes](https://github.com/Furnyr/Dissonity/wiki#testing-new-changes) documentation
- Updated [OutsideDiscordException](https://github.com/Furnyr/Dissonity/wiki/Exceptions) documentation
- Removed [AuthorizationException](https://github.com/Furnyr/Dissonity/wiki/Exceptions) documentation \*
- Added [Bridge interaction](https://github.com/Furnyr/Dissonity/wiki#-terms-) documentation

\* That implementation required Unity to load before being able to react to it, and there's nothing to do other than closing the activity. RpcBridge now automatically closes the activity (CloseNormal 1000) if the authorization is denied.

Mock images are outdated but updating them is not necessary.

## BridgeLib

- Set OutsideDiscord state when any query param isn't found

- Updated BridgeLib to use nonces and defined payloads

## Dissonity (internal)

- Commands no longer use generics, and utils now use maps

- Remove AuthorizationException and closedState (bool)

- Remove bridge actions in favor of bridge interactions (nonces and no-callback API)

- Removed `Request-` JS functions from API (internal) in favor of bridge interactions

- Added `Request-` JS functions to DissonityBridge

- Array properties should be [] instead of null if there's no data

- Exposed classes that were [previously unreleased](https://github.com/Furnyr/Dissonity/wiki/Namespaces) (Entitlement, EntitlementType, Sku, SkuPrice, SkuType, SkuFlags)

- Partially dismantled Authorize and Authenticate commands \*

\* AuthorizeResponse and AuthenticateResponse are used in the MultiEvent

## Dissonity (API) Updates

- OutsideDiscordException is now thrown if any query parameter is missing

- Exposed ProxyDomain property

- Updated exception text when accessing properties without initialization

- Updated exception text to mention "ready" instead of "initialized"

- Fixed MobileAppVersion getter (thank you)

- MobileAppVersion returns null if the platform is desktop during mock mode

- Potentially fixed PatchUrlMappings with the bridge interactions update

## Dissonity (API) Releases

- Added previously-unreleased commands: `GetEntitlements`, `GetSkus`, `StartPurchase`

- Added previously-unreleased event: `EntitlementCreate`

- Added official util: `FormatPrice`

- Added Models.CurrencyCode

- Added In-App Purchases to the mock object

Mock convention: Blue buttons open menus, dark buttons close menus, red buttons are dangerous.

Convention: Every property, method or object that starts with an underscore (_) is not supposed to be used by developers but must be public. This has been properly documented. To avoid confusion, the mock object has (@) as prefix.

## Testing notes

This progress version has lots of fundamental changes, so it's probably loaded with bugs.

- **Mock object simplification**: Colored buttons and adjusted spaces. Unnecessary properties have been hidden and channels are directly under the channel foldout. GuildMemberRpc has been removed from "Other players".

- **Official utils**: Api.FormatPrice and Api.PatchUrlMappings are pretty untested right now

- **Unreleased (now released) functionality**: The main thing to test is the mock implementation.

**Note 1**: When calling Api.StartPurchase, a mock entitlement is automatically created.

**Note 2**: Sku prices are written in cents

**Note 3**: \<MockChannel\>.VoiceStates is only added if the mock channel has the same id as the query channel id 

## Next

The next things I'm working on are a few QoL updates (Api.User? Api.GuildMember?), research regarding the black screen problem, outside Discord support and *potentially* dismantling the IframeBridge.

---

# 2024-10-01 - alpha progress 1

## Wiki

- Added documentation for the client [POST request](https://github.com/Furnyr/Dissonity/wiki/Design)
- Added [BridgeLib](https://github.com/Furnyr/Dissonity/wiki#-terms-) to terms
- Added [Testing new changes](https://github.com/Furnyr/Dissonity/wiki#testing-new-changes) documentation
- Added [AuthorizationException](https://github.com/Furnyr/Dissonity/wiki/Exceptions#authorizationexception) documentation
- Added [SdkVersion](https://github.com/Furnyr/Dissonity/wiki/Design#api-properties) documentation

## Example Node.js server

- Removed "<>" from example .env
- Added reference Node.js version to package.json
- Updated dependencies

## BridgeLib

- RpcBridge sets closed state when authorization is rejected

- Removed command state limitation

- Merged initialization listeners into InitialBridgeListener

## Dissonity (internal)

- AuthorizationException is now thrown when the RpcBridge is in a closed state

- Mock should log warnings when attempting to run commands with the wrong scopes (or platform)

- Minor fix when consuming a command

- Build post-processor adds sdk version to RpcBridge

## Dissonity (API)

- Added Api.SdkVersion and Api.MobileAppVersion (https://github.com/discord/embedded-app-sdk/pull/262)

- Added AuthorizationException

- Added MobileAppVersion to mock query

- Exposed Ready property

## Testing notes

Api.MobileAppVersion should be null in desktop and a string in the format "number.number" in the mobile client.

The mock logs should be warnings when the command isn't allowed and normal logs otherwise.

## Next

The next thing I'm working on is implementing the features that were recently released.
