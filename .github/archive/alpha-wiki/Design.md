
# 🛠️ Design 🛠️

As Dissonity is now a completely client-side SDK, it's expected that the developer implements the [code exchange](https://discord.com/developers/docs/activities/building-an-activity#step-5-authorizing-authenticating-users) on their backend. Dissonity will make a POST request to the specified path (read about configuration below) with a JSON payload and a `code` property. The server is expected to return a JSON payload with a `token` property.

## Static SDK

Unlike other implementations of the Embedded App SDK, Dissonity isn't used via a class instance; the API is static. This way, the information is easily accessible through scene changes and you don't need to use something like a singleton to keep access to the instance.

## API properties

API properties expose data without the need of any RPC command.

- [Query parameters](https://discord.com/developers/docs/activities/how-activities-work#activity-lifecycle) \*
- ProxyDomain
- Current user id
- [Configuration](https://github.com/Furnyr/Dissonity/wiki/Design#%EF%B8%8F-configuration-%EF%B8%8F)
- Initialized? boolean \*\*
- Ready? boolean \*\*
- IsMock? boolean
- SyncedUser \*\*\*
- SyncedGuildMemberRpc \*\*\*

\* When [mock mode](https://github.com/Furnyr/Dissonity/wiki/Mock) is enabled, query parameters are getters for mock data.

\*\* **Initialized** is true after `Api.Initialize` is called. **Ready** is true if the initialization was successful.

\*\*\* Read more [here](https://github.com/Furnyr/Dissonity/wiki/Design#structure-synchronization)

## Multi Event

If you read the [Home](https://github.com/Furnyr/Dissonity/wiki) page, you know that the initialization with the Discord client occurs before Unity loads. This arises questions:

1. What if initialization isn't complete when the Unity build loads?
2. Then, the initial data sent from the Discord client isn't available in Unity?

And the answers are

1. A message is sent to Unity through hiRPC once it's ready
2. The initial data is saved until Unity is ready, then it's sent

While using the official Embedded App SDK, you have a ready event, an authorize response and an authenticate response. In Dissonity, there's only a **MultiEvent** that holds all that data and it's received when the RPC connection is established.

```cs
using Dissonity.Commands.Responses;
using Dissonity.Events;

namespace Dissonity.Models
{
    /// <summary>
    /// This is not a normal RPC event, this "event" is sent through hiRPC
    /// once the authentication process has finished successfully and Unity has loaded.
    /// </summary>
    public class MultiEvent
    {
        public ReadyEventData ReadyData { get; set; }

        public AuthorizeData AuthorizeData { get; set; }

        public AuthenticateData AuthenticateData { get; set; }

        public object ServerResponse { get; set; }
    }
}
```

## ⚙️ Configuration ⚙️

Since the initial connection occurs before Unity loads, there must be a way for users to set their OAuth2 scopes, client id, etc.

That's where the **DissonityConfig** attribute comes in. You can create a default configuration file with (Right click > Create > Dissonity > Configuration). On build post-processing, this class will be accessed to update the hiRPC files to include its data.

```cs
using Dissonity;
using Dissonity.Models;
using Dissonity.Models.Builders;

[DissonityConfig]
public class DissonityUserConfiguration : SdkConfiguration<MyServerRequest, MyServerResponse>
{
    // Hover over properties for more information!

    // Initialization
    public override long ClientId => 123456789123456;

    public override string[] OauthScopes => new string[] { OauthScope.Identify, /*, your-oauth-scopes*/ };

    public override string TokenRequestPath => "/api/token";

    // Logging
    public override bool DisableDissonityInfoLogs => false;
    
    // If you want to call PatchUrlMappings before initialization, use this setting
    /*public override MappingBuilder[] Mappings => new MappingBuilder[] {
        new()
        {
            Prefix = "/foo",
            Target = "foo.com"
        }
    };*/

    // Utils
    public override bool SynchronizeUser => false;
    public override bool SynchronizeGuildMemberRpc => false;

    // App resolution
    public override ScreenResolution DesktopResolution => ScreenResolution.Viewport;
    public override ScreenResolution MobileResolution => ScreenResolution.Dynamic;
    public override ScreenResolution WebResolution => ScreenResolution.Dynamic;
}

// If you want to get or send data to your server while authenticating, define it here.
// You can access the response object in the Api.Initialize return value.
public class MyServerRequest : ServerTokenRequest {}
public class MyServerResponse : ServerTokenResponse {}
```

### Structure synchronization

The API properties `SyncedUser` and `SyncedGuildMemberRpc` are only available if `SynchronizeUser` or `SynchronizeGuildMemberRpc` are enabled respectively.

When this setting is enabled, Dissonity will automatically subscribe to the RPC to keep those properties up to date with the current state.

## 📲 Sending and receiving 📲

The home page covers the basic flow between Unity and Discord, but what's going on inside Unity?

## Api.Initialize

Before using the Dissonity API, we need to make sure that the hiRPC has successfully established a connection. Awaiting the Initialize method does just that and returns the MultiEvent instance:

```cs
using UnityEngine;
using Dissonity;

public class NewBehaviourScript : MonoBehaviour
{
    async void Start()
    {
        var multiEvent = await Api.Initialize();
    }
}
```

~~As easy as that~~

## Api.Commands

Api.Commands is another static class. It holds methods used to, you guessed it, send RPC commands.

When a method from Api.Commands is called, the command object is sent to hiRPC, that will send it to the Discord Client. Then the response is the same process inverted, with hiRPC sending the response back to the DissonityBridge and completing the task.

```cs
using UnityEngine;
using Dissonity;

public class NewBehaviourScript : MonoBehaviour
{
    async void Start()
    {
        await Api.Initialize();

        var response = await Api.Commands.UserSettingsGetLocale();
        Debug.Log($"User locale is {response.Locale}");
    }
}
```

## Api.Subscribe / Api.Unsubscribe

Api.Subscribe is a static class that holds methods used to listen for RPC events. The official SDK's subscribe methods always send an RPC command to the Discord client to subscribe to an event. Dissonity's subscription system is slightly different:

Each time you call any subscribe method you are adding a **listener** for that event, but only sending a subscribe RPC command the first time.

Something like this:

```cs
using UnityEngine;
using Dissonity;

public class NewBehaviourScript : MonoBehaviour
{
    async void Start()
    {
        await Api.Initialize();

        // RPC subscribe command only sent here
        await Api.Subscribe.SpeakingStart(Api.ChannelId, (data) =>
        {
            Debug.Log("Listener one!");
        });

        // Already subscribed to event, just adds the listener
        await Api.Subscribe.SpeakingStart(Api.ChannelId, (data) =>
        {
            Debug.Log("Listener two!");
        });
    }
}
```

<br>

Would need to be implemented like this in the official SDK (if it was C#):

<br>

```cs
using UnityEngine;
using Dissonity;
using System;

public class NewBehaviourScript : MonoBehaviour
{
    Action listeners;

    async void Start()
    {
        await Api.Initialize();

        listeners += ListenerOne;
        listeners += ListenerTwo;

        await Api.Subscribe.SpeakingStart(Api.ChannelId, (data) =>
        {
            listeners();
        });
    }

    void ListenerOne()
    {
        Debug.Log("Listener one!");
    }

    void ListenerTwo()
    {
        Debug.Log("Listener two!");
    }
}
```

<br>

Every call to a subscription method returns a **SubscriptionReference**. You can use this instance to remove a listener.

<br>

```cs
var listener1 = await Api.Subscribe.SpeakingStart(Api.ChannelId, (data) =>
{
    Debug.Log("Listener one!");
});

var listener2 = await Api.Subscribe.SpeakingStart(Api.ChannelId, (data) =>
{
    Debug.Log("Listener two!");
});

// Only remove first listener
Api.Unsubscribe(listener1);
```

<br>

You can also call Api.Unsubscribe passing the listener method and the corresponding type parameter:

<br>

```cs
await Api.Subscribe.SpeakingStart(Api.ChannelId, Listener);

void Listener(SpeakingData data)
{
    Debug.Log("Listener called!");
    Api.Unsubscribe<SpeakingData>(Listener);
}
```

## Other unsubscribe methods

If you don't have access to the subscription reference, you can still remove all the listeners from a specific event or clear every listener from every event:

```cs
// Remove every listener from this event
Api.UnsubscribeFromEvent(DiscordEventType.SpeakingStart);

// Remove every listener
Api.ClearAllSubscriptions();
```

> [!NOTE]
> Just like the RPC subscribe command is sent on the first subscribe call, the RPC unsubscribe command is sent when every listener from an event is removed.

> [!TIP]
> Feedback on this subscription API is appreciated.

## Other SDK methods

All the functionality that can't be classified into a subclass (like commands or subscriptions) is directly in the **Api** class. This includes the **Close** method and utils like **PatchUrlMappings**.

<br>

## Long or string?

Discord ids (snowflakes), permissions and flags are longs. Other non-snowflake ids (like the activity instance id) are strings.

<br>

## How much of the official SDK does Dissonity cover?

Dissonity will cover all features from the official SDK or offer something to achieve the same.