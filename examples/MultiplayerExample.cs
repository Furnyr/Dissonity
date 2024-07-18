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
        var matchmakingRoom = await client.Create<MatchmakingState>("matchmaking", new Dictionary<string, object>{{ "instanceId", instanceId }});

        // Listen for matchmaking room instructions
        matchmakingRoom.OnMessage<Dictionary<string, object>>("matchmake", async data => {

            //\ Leave matchmaking room
            await matchmakingRoom.Leave();

            //? Room already exists
            if ((bool) data["exists"]) {

                //\ Join the existing activity room
                room = await client.JoinById<GameState>(instanceId, new Dictionary<string, object>{{ "userId", userId }});

                // Client is now connected to the room!
            }

            //? Doesn't exist
            else {

                //\ Create the activity room
                room = await client.Create<GameState>("game", new Dictionary<string, object>{{ "instanceId", instanceId }, { "userId", userId }});

                // Client is now connected to the room!
            }
        });
    }
}
