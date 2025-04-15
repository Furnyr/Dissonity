# 🎭 Mock 🎭

Mocking is a feature that allows developers to test their Discord activity within Unity. It consists of:

- DiscordMock: MonoBehaviour script that holds data to simulate the Discord client, as well as a custom menu with utils like dispatching events or adding mock players
- API Mock mode: Mode where the Dissonity API interacts with the **DiscordMock** inside Unity.

---

When trying to use the API normally within Unity, Dissonity will detect that it's running inside the Unity editor and will prompt the developer to use the mock.

<img alt="no mock editor warning" src="https://i.imgur.com/pdNaGg5.png">

<br>

<br>

You can create the mock object right clicking the hierarchy and selecting `Dissonity` > `Discord Mock`.

> [!NOTE]
> Both the **DiscordMock** and [DissonityBridge](https://github.com/Furnyr/Dissonity/wiki#-terms-) are not destroyed on load. If there's another mock in the next scene, it will be destroyed.

<br>

## The DiscordMock object

The mock object is structured in multiple foldouts, starting with **Query**, **Activity** and **General Events**. The "query" foldout only includes the query parameters and the "general events" foldout has buttons to dispatch events that are not related to a single player. However, the "activity" foldout is more complex:

<img alt="activity foldout expanded" src="https://i.imgur.com/nnxZvMi.png">

<br>

<br>

- Locale: Enum of all the locales supported by Discord
- CurrentPlayer: Object that represents the player that is running the activity
- OtherPlayers: List of other mock players
- Channels: List of mock channels

Dissonity will also assign unique snowflakes as ids to mock players and channels.

> [!NOTE]
> `Player terminology` <br> The term "player" is only used by Dissonity and is not an official Discord term. As you can see in the image, mocking a Discord "user" can also means mocking its guild member or other structures assigned to it, hence, Dissonity refers to a "player" when it's the information related to a single individual.

> [!WARNING]
> The mock object menu is completely rendered by a [CustomEditor](https://docs.unity3d.com/Manual/editor-CustomEditors.html). This means that the menu can break in very weird ways, between different screen sizes and Unity versions. This will require as much testing as possible.

<br>

## API Mock mode

When API Mock mode is enabled, Dissonity will return mock data or print logs instead of attempting to send messages to the [IframeBridge](https://github.com/Furnyr/Dissonity/wiki#-terms-).

<br>

## Building with a mock

There's no problem with making a build with a mock object in the scene, since it will be auto-destroyed when running outside of the Unity editor. That being said, you can delete your mocks before making a final release, of course.

<br>

## Considering

> [!IMPORTANT]
> This section refers to possible additions to the final release, feedback is appreciated.

### Limitations

The only current limitation of the mock object is that it can't return mock channel messages. Messages are complex structures and I don't think they are necessary for activity development, but I'm open to hearing another opinion.