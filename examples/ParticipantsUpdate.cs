using UnityEngine;
using static Dissonity.Api;

public class MyScript : MonoBehaviour
{
    int lastParticipantCount;

    async void Start()
    {
        lastParticipantCount = (await GetInstanceParticipants()).participants.Length;

        DissonityLog($"There are {lastParticipantCount} users");

        // Discord may send an event multiple times, so if you're
        // just trying to detect when a user joins or leaves, don't
        // do anything when (lastParticipantCount == data.participants.Length)
        SubActivityInstanceParticipantsUpdate((data) => {

            //? Someone left
            if (data.participants.Length < lastParticipantCount)
            {
                lastParticipantCount = data.participants.Length;

                DissonityLog("Received a user leave!");
            }

            //? Some joined
            else if (data.participants.Length > lastParticipantCount)
            {
                lastParticipantCount = data.participants.Length;
                
                DissonityLog("Received a new user!");
            }
        });
    }
}
