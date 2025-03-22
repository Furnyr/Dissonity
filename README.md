
> [!WARNING]
> Dissonity Version 2 is still in beta and may cause issues with your code between updates. Use it at your own discretion!

<div align="center">
    <img src="https://i.imgur.com/60Sv0ak.png" width="650">
</div>

<div align="center">
<img src="https://img.shields.io/badge/version-beta%202%20v2.0.0-yellow">
</div>


# About

[Dissonity](https://dissonity.dev) is a Unity SDK that allows you to easily make Discord activities!

# Examples

Update the current activity:

```cs
using UnityEngine;
using Dissonity.Models;
using Dissonity.Models.Builders;
using static Dissonity.Api;

public class MyScript : MonoBehaviour
{
    async void Start()
    {
        // Initialize Dissonity
        await Initialize();

        // Update activity
        await Commands.SetActivity(new ActivityBuilder {
            Type = ActivityType.Playing,
            Details = "In the lobby",
            State = "Level 10"
        });
    }
}
```

Get all participants:

```cs
using UnityEngine;
using Dissonity.Models;
using static Dissonity.Api;

public class MyScript : MonoBehaviour
{
    async void Start()
    {
        // Initialize Dissonity
        await Initialize();

        // Get all the participants in the activity
        Participant[] participants = await Commands.GetInstanceConnectedParticipants();

        foreach (Participant participant in participants)
        {
            Debug.Log($"{participant.DisplayName} is playing!");
        }
    }
}
```

Listen to `SpeakingStart`:

```cs
using UnityEngine;
using static Dissonity.Api;

public class MyScript : MonoBehaviour
{
    async void Start()
    {
        // Initialize Dissonity
        await Initialize();

        // Subscribe to speaking start
        await Subscribe.SpeakingStart(ChannelId, (data) =>
        {
            Debug.Log($"User with id {data.UserId} is talking!");
        });
    }
}
```

## Installation

1. Create a Unity project (Unity 2021.3 or later, Unity 6 recommended)
2. Open the package manager and install the package from https://github.com/Furnyr/Dissonity.git?path=/unity#dev
3. Set the build platform to Web / WebGL
4. Player settings > Resolution and Presentation > Set the WebGL template to Dissonity

Dissonity is now installed! But you still need to configure a few components:

## Configuration

1. Right click your project assets > Create > Dissonity > Configuration
2. Set your app id in `<SdkConfiguration>.ClientId` (find it [here](https://discord.com/developers/applications))

Up and running! If you want to test your activity within Unity:

## Testing

1. Right click the hierarchy > Dissonity > Discord Mock

If you run the game in a scene where there's a `@DiscordMock` object, it will act as a Discord simulator within Unity.

## Production

Dissonity handles the game-making process. You will still need to host the backend that will serve the activity to Discord and handle the authentication process ([read the documentation](#documentation)).


# Documentation

https://github.com/Furnyr/Dissonity/wiki

https://dissonity.dev (not done yet)

# Contact

DM me on Discord: `nyrrren`

# License

Licensed under the [Apache License, Version 2.0](LICENSE)

# Disclaimer

This project is not affiliated, endorsed, or sponsored by Discord Inc. or Unity Technologies.

The Discord Developer Terms of Service, Discord Developer Policy and Unity Terms of Service apply to you and the applications you develop utilizing this SDK.