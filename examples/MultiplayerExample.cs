using System.Collections.Generic;
using UnityEngine;
using Colyseus;
using static Dissonity.Api;

// You need to place the generated C# classes from running "npm run colyseus"
// inside the Unity project.

public class MyScript : MonoBehaviour
{
    ColyseusClient client;
    ColyseusRoom<GameState> room;

    async void Start()
    {
        //\ Get necessary Discord data
        string instanceId = await GetSDKInstanceId();
        string userId = await GetUserId();

        //\ Connect to matchmaking room
        // (This implementation can be improved, but this should do)
        client = new ColyseusClient("wss://<your-app-id>.discordsays.com/.proxy");

        //\ Create or join the activity room
        room = await client.JoinOrCreate<GameState>("game", new Dictionary<string, object>{
            { "instanceId", instanceId }, 
            { "userId", userId }
        });

        // Client is now connected to the room!
    }
}
