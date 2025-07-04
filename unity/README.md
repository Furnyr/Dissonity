
> [!NOTE]
> Looking for version 1? See the [v1 branch](https://github.com/Furnyr/Dissonity/tree/v1).

<div align="center">
    <a href="https://dissonity.dev" alt="a"><img src="https://i.imgur.com/AmGkPpE.png" width="650"></a>
</div>

<div align="center">
    <img src="https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2FFurnyr%2FDissonity%2Frefs%2Fheads%2Fv2%2Funity%2Fpackage.json&query=version&prefix=v&label=version">
    <a href="https://github.com/Furnyr/Dissonity/actions"><img src="https://img.shields.io/github/actions/workflow/status/Furnyr/Dissonity/unity.yaml"></a>
</div>


## About

[Dissonity](https://github.com/Furnyr/Dissonity/) is a Unity SDK that allows you to easily make Discord activities!

It is a C# implementation of the [Embedded App SDK](https://discord.com/developers/docs/developer-tools/embedded-app-sdk).

## Example

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
        await Commands.SetActivity(new ActivityBuilder
        {
            Type = ActivityType.Playing,
            Details = "Selecting game mode",
            State = "In a Group"
        });

        // Get all the participants in the activity
        Participant[] participants = await Commands.GetInstanceConnectedParticipants();

        foreach (Participant participant in participants)
        {
            Debug.Log($"{participant.DisplayName} is playing!");
        }

        // Subscribe to speaking start
        await Subscribe.SpeakingStart(ChannelId, (data) =>
        {
            Debug.Log($"User with id {data.UserId} is speaking!");
        });
    }
}
```

## Installation

> You can also install Dissonity via [OpenUPM](https://openupm.com/packages/com.furnyr.dissonity/) or with each `.unitypackage` file found in the [Releases](https://github.com/Furnyr/Dissonity/releases) tab.

1. Create a new Unity project (Unity 2021.3 or later, Unity 6 recommended)
2. Open the package manager and install the package from this git URL: `https://github.com/Furnyr/Dissonity.git?path=/unity#v2`
3. Use the pop-up dialog to select a configuration file
4. Set the build platform to Web / WebGL
5. Player settings > Resolution and Presentation > Set the WebGL template to Dissonity

Dissonity is now installed! But you still need to configure a few things:

## Configuration

1. Open the configuration file in Assets/Dissonity/DissonityConfiguration.cs
2. Set your app id in `<SdkConfiguration>.ClientId` (find it or create a Discord app [here](https://discord.com/developers/applications))

Up and running! If you want to test your activity within Unity:

## Testing

1. Right-click the hierarchy > Dissonity > Discord Mock

If you run the game in a scene where there's a `@DiscordMock` object, you will be able to test the activity within Unity, simulating Discord.

If there isn't a mock object in the scene, Dissonity will automatically create one.

## Production

Dissonity helps in the process to make the game, but you will still need to host the backend that will serve the activity to Discord and handle authentication.

If you're not sure how to continue, read the documentation.

> [!TIP]
> If you are also using a framework that provides Discord functionality, you should read [Third-party support](http://dissonity.dev/guides/v2/third-party-support).


## Documentation

- **Activities**: https://discord.com/developers/docs/activities/overview
- **Dissonity**: https://dissonity.dev/

## Contact

- General contact: [`contact@dissonity.dev`](mailto:contact@dissonity.dev)
- Security issues: [`security@dissonity.dev`](mailto:security@dissonity.dev)
- Discord: `nyrrren`

## License

Licensed under the [Apache License, Version 2.0](LICENSE).

This project includes code from the [Discord Embedded App SDK](https://github.com/discord/embedded-app-sdk), licensed under the [MIT License](MIT_LICENSE.md).

## Disclaimer

This project is not affiliated, endorsed, or sponsored by Discord Inc. or Unity Technologies.

The Discord Developer Terms of Service, Discord Developer Policy and Unity Terms of Service apply to you and the applications you develop utilizing this SDK.