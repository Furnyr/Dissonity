## Unity for the Discord Embedded App SDK

If you are interested in creating Discord activities with Unity, this repository might help you! Here's everything you need to get started. This project was made for Unity 2022.3.8f1 but it may work for other versions too.
Keep in mind the Embedded App SDK is still in public preview.

(If you would prefer to code a solution yourself, this [guide](https://gist.github.com/Furnyr/e5e968ff7d5a02dd12b370dbeadaf663) may help)

1. Create a Unity project and set WebGL as the target build platform
2. Go to `Project Settings` > `Player` > `Resolution and Presentation`
3. Set the resolution to 1920 x 1080 (this resolution is used in the Node.js project for automation)
4. Set the WebGL Template to Minimal
5. In the Assets folder and scene, set up the C# scripts as explained later
6. Make a build for WebGL and place the files inside `node/src/client/nested`
7. Run `npm run install` to initialize the project
8. Run `npm run start` to start the server. Your activity is now ready!

If you don't know how to test your activity refer to the [Discord documentation](https://discord.com/developers/docs/activities/development-guides#run-your-application-locally)

## Node.js project

The [Node.js](https://nodejs.org/) project is found inside the `node` folder. There you can:
1. Use TypeScript to modify the .jslib plugin for Unity (`node/src/client/_unity_bridge/BridgeLibrary.ts`)
2. Extend logic to implement features from the SDK as needed  (`node/src/index.ts`)
3. Modify the application server (`node/src/server/index.ts`)

**Don't forget to set your env variables to a .env file!**
- If you're using pnpm instead of npm you can take a look at the `node/src/_pnpm` folder.

### package.json scripts
- `npm run unity`: Compile to JavaScript the Unity plugin inside `node/_unity_structures`.
- `npm run build`: Compile to JavaScript the TypeScript files so they can be executed.
- `npm run execute`: Run the server.
- `npm run start`: Build the project, then run the server.

## Unity scripts

The Unity C# code is found inside the `unity` folder.
- `BridgeLibrary.jslib`: Must be placed inside Assets/Plugins. You can extend the code as needed using TypeScript inside the Node.js project.
- `DynamicSDKBridge.cs`: Must be assigned to a GameObject named **exactly** `DynamicSDKBridge` inside the scene to receive data from the .jslib plugin.
- `DiscordClasses.cs`: Used as a base for Discord data types.
- `BridgeClasses.cs`: Contains classes used to parse the data received from the SDK over the .jslib plugin.
- `StaticSDKBridge.cs`: Used to interact with the Discord client inside your project. Example:
```cs
using UnityEngine;
using static StaticSDKBridge;

public class MyGameObject : MonoBehaviour
{

    async void Start ()
    {
        User user = await GetUser();
        Debug.Log($"The user id is {user.id}!");

        // Subscribe to activity instance participants update
        SubActivityInstanceParticipantsUpdate( OnParticipantsUpdate );
    }

    void OnParticipantsUpdate (InstanceParticipantsData data)
    {
        Debug.Log("Someone joined or left the activity!");
    }
}
```

## C# Scripts documentation

You have to `await` all the methods that directly return data, like `GetUser`, `GetInstanceParticipants`, `GetSDKInstanceId`...

## Subscribing to events

Methods prefixed with "Sub" add a subscription to the event and accept two arguments:
1. A listener function that will be called when the event occurs
2. A boolean that indicates whether to add the listener *and* subscribe to the discordSdk instance, or just add the listener. Useful when you want to add another listener function but the event is already subscribed to.

Methods prefixed with "Unsub" remove a subscription from the event and accept two arguments:
1. The listener function to be removed
2. A boolean that indicates whether to remove the listener *and* unsubscribe to the discordSdk instance, or just remove the listener. Useful when you just want to remove a listener but will need to add another soon.

## Implementing more SDK features

In the future I may implement more SDK features to this project, but in the meantime I left comments everywhere if you need to add more functionality now.
This project doesn't include a base for multiplayer, but you should be able to continue from here.
