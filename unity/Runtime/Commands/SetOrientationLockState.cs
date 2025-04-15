using System;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class SetOrientationLockState : FrameCommand
    {
        #nullable enable annotations

        internal override string Command => DiscordCommandType.SetOrientationLockState;
    
        [JsonProperty("lock_state")]
        public OrientationLockStateType LockState { get; set; }
        
        [JsonProperty("picture_in_picture_lock_state", NullValueHandling = NullValueHandling.Ignore)]
        public OrientationLockStateType? PictureInPictureLockState { get; set; }
        
        [JsonProperty("grid_lock_state", NullValueHandling = NullValueHandling.Ignore)]
        public OrientationLockStateType? GridLockState { get; set; }

        public SetOrientationLockState(OrientationLockStateType lockState, OrientationLockStateType? pipLockState, OrientationLockStateType? gridLockState)
        {
            LockState = lockState;
            PictureInPictureLockState = pipLockState;
            GridLockState = gridLockState;
        }
    }
}