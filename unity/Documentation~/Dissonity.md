<div align="center">
	<br />
	<p>
		<a><img src="https://i.imgur.com/TuawbuK.png" width="500"/></a>
	</p>
	<br />
</div>

### Unity package documentation

## Editor

### Right click menu (`Dissonity` > `Discord Bridge`)

Creates a DiscordBridge object, it must be within all the scenes that need to interact with the Embedded App SDK. You shouldn't modify this object manually.

It also has a "Dont Destroy On Load" option. If you use it, you should only place the object in your first scene.

## Overrides

You can set override values through the API class to test them without needing to run the game inside Discord. The overrides will only work while in the Unity editor and not once the game is exported.

All the current override values:

```
OverrideInstanceId
OverrideUserId
OverrideUserGlobal
OverrideUsername 
OverrideUserAvatar
OverrideHardwareAcceleration
OverrideUserLocale
```

## Namespace: `Dissonity`

- Classes representing Discord structures
- Classes representing data sent from the SDK
- Static Api class

Most of the classes from this package are only used to represent types that you ***receive***. The classes that are meant for creation purposes are suffixed `Builder`. Otherwise, you shouldn't create and send manually instantiated classes.

## Api class

You can use the methods from the Dissonity API in 3 ways:

1. Accessing the namespace (`Dissonity.Api.SomeMethod();`)
2. Importing the namespace (`using Dissonity; ... Api.SomeMethod();`)
3. Importing the static API methods (recommended) (`using static Dissonity.Api; ... SomeMethod();`)

> Importing the static API methods won't import the classes, so if you need to access a builder (like ActivityBuilder) you could import the namespace too.

# 🎮 Dissonity API methods 🎮

## `Sub<Event>`

All the methods prefixed with "Sub" subscribe to an event and accept two arguments:

1. [Delegate] : A listener function that will be called when the event occurs
2. [Boolean]\(optional): Defaults to false. A boolean that when false indicates to add the listener and subscribe to the DiscordSDK instance and otherwise just adds the listener. Useful when you want to add another listener function but the event is already subscribed to.

- Required scopes: [Depends on the event](https://discord.com/developers/docs/developer-tools/embedded-app-sdk#sdk-events)
- Returns: void

All the currently supported subscription methods:
```
SubVoiceStateUpdate
SubSpeakingStart
SubSpeakingStop
SubActivityLayoutModeUpdate
SubOrientationUpdate
SubCurrentUserUpdate
SubThermalStateUpdate
SubActivityInstanceParticipantsUpdate
```

##

## `Unsub<Event>`

All the methods prefixed with "Unsub" remove a subscription to an event and accept two arguments:

1. [Delegate] : The listener function to be removed
2. [Boolean]\(optional): Defaults to false. A boolean that when false indicates to remove the listener and unsubscribe to the DiscordSDK instance and otherwise just removes the listener. Useful when you want to remove a listener but will need to add another soon.

(No required scopes)

- Returns: void

All the currently supported unsubscription methods:
```
UnsubVoiceStateUpdate
UnsubSpeakingStart
UnsubSpeakingStop
UnsubActivityLayoutModeUpdate
UnsubOrientationUpdate
UnsubCurrentUserUpdate
UnsubThermalStateUpdate
UnsubActivityInstanceParticipantsUpdate
```

##

## `GetSDKInstanceId`

Used to obtain the activity instance id.

(No arguments)

(No required scopes)

- Returns: Task\<string\>

##

## `GetChannelId`

Used to obtain the channel id.

(No arguments)

(No required scopes)

- Returns: Task\<string\>

##

## `GetGuildId`

Used to obtain the guild id.

(No arguments)

(No required scopes)

- Returns: Task\<string\>

##

## `GetUserId`

Used to obtain the user id.

(No arguments)

(No required scopes)

- Returns: Task\<string\>

##

## `GetUser`

Used to obtain the entire user object. If you just need the id use `GetUserId` instead.

(No arguments)

(No required scopes)

- Returns: Task\<User\>

##

## `GetChannel`

Used to obtain a channel object. If you just need the id of the current channel use `GetChannelId` instead.

- Arguments:

- - [String] : The channel id

- Required scopes: `guilds`

- Returns: Task\<Channel\>

##

## `GetInstanceParticipants`

Used to obtain an array of the users connected to the voice channel. `<InstanceParticipantsData>.participants` is of type `Participant[]`.

(No arguments)

(No required scopes)

- Returns: Task\<InstanceParticipantsData\>

##

## `EncourageHardwareAcceleration`

Attempts to enable [hardware acceleration](https://discord.com/developers/docs/developer-tools/embedded-app-sdk#encouragehardwareacceleration).

(No arguments)

(No required scopes)

- Returns: Task

##

## `GetChannelPermissions`

Used to obtain a channel's permissions. `<ChannelPermissionsData>.permissions` is a string. You may parse it to process it.

- Arguments:

- - [String] : The channel id

- Required scopes: `guilds.members.read`

- Returns: Task\<ChannelPermissionsData\>

##

## `GetPlatformBehaviors`

Used to obtain information about supported platform behaviors.

(No arguments)

(No required scopes)

- Returns: Task\<PlatformBehaviorsData\>

##

## `InitiateImageUpload`

Presents the file upload flow in the Discord client. `<ImageUploadData>.imageUrl` is a string that contains the image url, unless the user canceled the upload. `<ImageUploadData>.canceled` is a bool.

(No arguments)

(No required scopes)

- Returns: Task\<ImageUploadData\>

##

## `OpenExternalLink`

Allows for opening an external link from within the Discord client.

- Arguments:

- - [String] : The url

(No required scopes)

- Returns: void

##

## `OpenInviteDialog`

Presents an invitation modal dialog.

(No arguments)

(No required scopes)

- Returns: void

##

## `OpenShareMomentDialog`

Presents a modal dialog to share media to a channel or direct message.

- Arguments:

- - [String] : The media url

(No required scopes)

- Returns: void

##

## `SetActivity`

Modifies how the activity's rich presence is displayed in the Discord client.

- Arguments:

- - ActivityBuilder : A builder class with the activity data

- Required scopes: `rpc.activities.write`

- Returns: Task\<Activity\>

##

## `SetOrientationLockState`

[Discord docs](https://discord.com/developers/docs/developer-tools/embedded-app-sdk#setorientationlockstate)

Locks the application to specific orientations in each of the supported layout modes.

- Arguments:

- - [String] : Lock state
- - [String] : Picture in picture lock state
- - [String] : Grid lock state

- Required scopes: `guilds.members.read`

- Returns: void

##

## `GetUserLocale`

Used to obtain the current user's locale. `<LocaleData>.locale` is a string.

(No arguments)

- Required scopes: `identify`

- Returns: Task\<LocaleData\>

##

## `SetConfig`

Set whether or not the PIP (picture-in-picture) is interactive. `<ConfigData>.use_interactive_pip` is a bool.

- Arguments:

- - [Boolean] : Use interactive PIP

(No required scopes)

- Returns: Task\<ConfigData\>

##

## `WaitForLoad`

Used to wait until the npm package is ready. You don't really need this since the npm package will process the sent requests once it loads. You could use this method to prevent the player from seeing the scene before you load assets that use SDK data, for example.

(No arguments)

- Returns: Task

## `DissonityLog`

You will need to debug your activity looking at the browser dev console. To (hopefully) save you some time, you can use DissonityLog instead of Debug.Log. The only difference is that when DissonityLog is called outside of the Unity Editor, it adds `[Dissonity]:` in front of the log. This way, you can easily filter the logs that contain "dissonity".

- Arguments:
- - [Object] : Acts as the first argument of Debug.Log

- Returns: void

[Delegate]: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/
[Boolean]: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool
[String]: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types#the-string-type
[Object]: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types