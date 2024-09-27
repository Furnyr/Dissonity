using System;
using Dissonity.Commands.Responses;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class SetOrientationLockState : FrameCommand<NoResponse>
    {
        #nullable enable annotations

        internal override string Command => DiscordCommandType.SetOrientationLockState;
    
        [JsonProperty("lock_state")]
        public OrientationLockStateType LockState { get; set; }
        
        [JsonProperty("picture_in_picture_lock_state")]
        public OrientationLockStateType? PictureInPictureLockState { get; set; }
        
        [JsonProperty("grid_lock_state")]
        public OrientationLockStateType? GridLockState { get; set; }

        public SetOrientationLockState(OrientationLockStateType lockState)
        {
            LockState = lockState;
        }
    }
}