# 💿 Namespaces 💿

Some classes or namespaces are marked with emojis:
- `🔒` Fully internal.

- `🔑` Necessarily public but developers don't need to interact with it.

- `❓` Terminology not used by Discord

- `❌` Unreleased

<br>

## Dissonity

Main SDK namespace.
- `Api`
- `DiscordMock`
- `DissonityConfigAttribute`
- `CommandException`
- `OutsideDiscordException`
- `Utils`
- `ServerTokenRequest`
- `ServerTokenResponse`
- `ISdkConfiguration`
- `_UserData🔑`
- `DissonityBridge🔑`

<br>

## Dissonity.Models

Classes that represent Discord structures, or structures used to communicate with Discord. There's a bunch of unused types in the official SDK. Dissonity only implements the ones that are used.

Developers will never need to instantiate any of these models, they're exclusively received from the Discord client. The only classes meant for instantiation are [Builders](https://github.com/Furnyr/Dissonity/wiki/Namespaces#dissonitymodelsbuilders).

- `Activity`
- `ActivityAssets`
- `ActivityParty`
- `ActivityType`
- `ActivitySecrets`
- `Application`
- `AuthenticatedApplication ❓`
- `ApiGuildMember`
- `ApiUser`
- `Attachment`
- `AuthenticatedUser❓`
- `AuthorizeResponseType`
- `AvatarDecoration`
- `BaseUser❓`
- `CurrencyCode`
- `ChannelRpc❓`
- `ChannelMention`
- `ChannelType`
- `ConsoleLevel`
- `Embed`
- `EmbedAuthor`
- `EmbedField`
- `EmbedFooter`
- `EmbedProvider`
- `Emoji`
- `Entitlement`
- `EntitlementType`
- `GuildMember`
- `GuildMemberFlags`
- `GuildMemberRpc`
- `Image`
- `LayoutModeType`
- `Locale`
- `Message`
- `MessageActivity`
- `MessageActivityType`
- `MessageApplication`
- `MessageReference`
- `MultiEvent❓`
- `OauthScope`
- `OrientationLockStateType`
- `OrientationType`
- `Participant`
- `PermissionFlags`
- `Platform`
- `PremiumType`
- `Reaction`
- `ReadyConfig`
- `SpeakingData`
- `Sku`
- `SkuPrice`
- `SkuType`
- `SkuFlags`
- `ScreenResolution❓`
- `ThermalStateType`
- `Timeframe`
- `User`
- `UserFlags`
- `UserVoiceState`
- `Video`
- `VoiceState`

<br>

## Dissonity.Models.Mock 🔑

Holds classes with default values only used in the mock menu.

<br>

## Dissonity.Models.Interop

Structures used to interact with the Discord client at an internal level (RPC messages, initialization...)

The only public classes are:
- `QueryData`
- `RpcCloseCode`
- `RpcErrorCode`

<br>

## Dissonity.Models.Builders

Classes used to create objects, usually as arguments for other methods or features.

- `ActivityBuilder`
- `MappingBuilder`
- `PatchUrlMappingsConfigBuilder`

<br>

## Dissonity.Events

Event objects and their corresponding data. Only the `-Data` classes are public.

There's also:

- `SubscriptionReference`
- `DiscordEventType`

<br>

## Dissonity.Commands 🔒

Command objects sent to Discord. Developers use API methods instead.

<br>

## Dissonity.Commands.Response

Command responses. Only the `-Data` classes are public.

<br>

## Dissonity.Bus 🔒

Classes used to keep track of event listeners.

<br>

## Dissonity.Editor 🔒

Everything that only runs in the Unity editor is here (build post-processor, menus...)