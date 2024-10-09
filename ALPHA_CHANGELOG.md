This file will document changes made during the alpha phase.

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

- Array properties should be [] instead of null is there's no data

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