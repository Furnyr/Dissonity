/*
    Script that receives the Discord data from the JS plugin.
    If you have any problem open an issue at https://github.com/Furnyr/Dissonity
*/

using System;
using UnityEditor;
using UnityEngine;
using static Dissonity.Api;

namespace Dissonity
{
    //! You don't need call anything from this script. All the methods are called by the JavaScript plugin.
    public class DiscordBridge : MonoBehaviour
    {
        #if UNITY_EDITOR
            [NoModify]
            [TextArea(4, 5)]
            public string information = "There must be a DiscordBridge object (with that exact name) within each scene that needs to interact with Discord.\n\n(Tip: You can place it in your first scene and check 'dontDestroyOnLoad')";
        #endif

        public bool dontDestroyOnLoad = false;

        // Initialization
        void Awake () {

            //? Initialize bridge
            if (!bridgeInitialized) {

                InitializeSDKBridge();
                WaitForLoad(); // to check for version unmatches

                bridgeInitialized = true;
            }

            //? Don't destroy
            if (dontDestroyOnLoad) {
                DontDestroyOnLoad(gameObject);
            }
        }

        //^ Npm package loaded
        public void NpmLoad (string version) {

            var unityVersion = Config.version.Split(".");
            var npmVersion = version.Split(".");

            //? Major version doesn't match
            if (unityVersion[0] != npmVersion[0]) {
                throw new Exception($"[Dissonity]: Detected major version unmatch. NPM package is v{version} while Unity package is v{Config.version}. Please make sure both packages have the same major version.");
            }

            //? Minor version unmatch
            if (unityVersion[1] != npmVersion[1]) {
                DissonityWarn($"Detected minor version unmatch. NPM package is v{version} while Unity package is v{Config.version}. Please make sure both packages have the same minor version. Otherwise, some functionality may not work properly.");
            }

            if (_LoadEvent != null) _LoadEvent();
            _LoadEvent = null;
        }


        //# BRIDGE SUBSCRIBE METHODS - - - - -
        public void VoiceStateUpdate (string stringData) {
            
            VoiceStateUpdateData data = JsonUtility.FromJson<VoiceStateUpdateData>(stringData);
            
            // Send data to subscriptions
            _VoiceStateUpdateEvent(data);
        }

        public void SpeakingStart (string stringData) {

            SpeakingData data = JsonUtility.FromJson<SpeakingData>(stringData);

            // Send data to subscriptions
            _SpeakingStartEvent(data);
        }

        public void SpeakingStop (string stringData) {

            SpeakingData data = JsonUtility.FromJson<SpeakingData>(stringData);

            // Send data to subscriptions
            _SpeakingStopEvent(data);
        }

        public void ActivityLayoutModeUpdate (string stringData) {
            
            ActivityLayoutModeUpdateData data = JsonUtility.FromJson<ActivityLayoutModeUpdateData>(stringData);

            // Send data to subscriptions
            _ActivityLayoutModeUpdateEvent(data);
        }

        public void OrientationUpdate (string stringData) {

            OrientationUpdateData data = JsonUtility.FromJson<OrientationUpdateData>(stringData);

            // Send data to subscriptions
            _OrientationUpdateEvent(data);
        }

        public void CurrentUserUpdate (string stringData) {

            CurrentUserUpdateData data = JsonUtility.FromJson<CurrentUserUpdateData>(stringData);

            // Send data to subscriptions
            _CurrentUserUpdateEvent(data);
        }

        public void EntitlementCreate (string stringData) {
            
            EntitlementCreateData data = JsonUtility.FromJson<EntitlementCreateData>(stringData);

            // Send data to subscriptions
            _EntitlementCreateEvent(data);
        }

        public void ThermalStateUpdate (string stringData) {
            
            ThermalStateUpdateData data = JsonUtility.FromJson<ThermalStateUpdateData>(stringData);

            // Send data to subscriptions
            _ThermalStateUpdateEvent(data);
        }

        public void ActivityInstanceParticipantsUpdate (string stringData) {

            InstanceParticipantsData data = JsonUtility.FromJson<InstanceParticipantsData>(stringData);

            // Send data to subscriptions
            _ActivityInstanceParticipantsUpdateEvent(data);
        }

        //# BRIDGE NON-SUBSCRIBE METHODS - - - - -
        public void ReceiveSDKInstanceId (string id) {
            
            //? Already cached
            if (cachedInstanceId != null) {

                // Send data to subscriptions
                if (_GetInstanceIdEvent != null) {
                    _GetInstanceIdEvent(cachedInstanceId);
                }

                //¡ Clear delegates
                _GetInstanceIdEvent = null;

                return;
            }

            // Set cache
            cachedInstanceId = id;

            // Send data to subscriptions
            if (_GetInstanceIdEvent != null) {
                _GetInstanceIdEvent(id);
            }

            //¡ Clear delegates
            _GetInstanceIdEvent = null;
        }

        public void ReceiveChannelId (string id) {

            //? Already cached
            if (cachedChannelId != null) {

                // Send data to subscriptions
                if (_GetChannelIdEvent != null) {
                    _GetChannelIdEvent(cachedChannelId);
                }
                
                //¡ Clear delegates
                _GetChannelIdEvent = null;

                return;
            }

            // Set cache
            cachedChannelId = id;

            // Send data to subscriptions
            if (_GetChannelIdEvent != null) {
                _GetChannelIdEvent(cachedChannelId);
            }

            //¡ Clear delegates
            _GetChannelIdEvent = null;
        }

        public void ReceiveGuildId (string id) {

            //? Already cached
            if (cachedGuildId != null) {

                // Send data to subscriptions
                if (_GetGuildIdEvent != null) {
                    _GetGuildIdEvent(cachedGuildId);
                }

                //¡ Clear delegates
                _GetGuildIdEvent = null;

                return;
            }

            // Set cache
            cachedGuildId = id;

            // Send data to subscriptions
            if (_GetGuildIdEvent != null) {
                _GetGuildIdEvent(id);
            }

            //¡ Clear delegates
            _GetGuildIdEvent = null;
        }

        public void ReceiveUserId (string id) {

            //? Already cached
            if (cachedUserId != null) {

                // Send data to subscriptions
                if (_GetUserIdEvent != null) {
                    _GetUserIdEvent(cachedUserId);
                }

                //¡ Clear delegates
                _GetUserIdEvent = null;

                return;
            }

            // Set cache
            cachedUserId = id;

            // Send data to subscriptions
            if (_GetUserIdEvent != null) {
                _GetUserIdEvent(id);
            }

            //¡ Clear delegates
            _GetGuildIdEvent = null;
        }

        public void ReceiveUser (string stringData) {

            // Parse string
            User data = JsonUtility.FromJson<User>(stringData);

            // Send data to subscriptions
            if (_GetUserEvent != null) {
                _GetUserEvent(data);
            }

            //¡ Clear delegates
            _GetUserEvent = null;
        }

        public void ReceiveSetActivity (string stringData) {

            // Parse string
            Activity data = JsonUtility.FromJson<Activity>(stringData);

            // Send data to subscriptions
            if (_SetActivityEvent != null) {
                _SetActivityEvent(data);
            }

            //¡ Clear delegates
            _SetActivityEvent = null;
        }

        public void ReceiveInstanceParticipants (string stringData) {

            // Parse string
            InstanceParticipantsData data = JsonUtility.FromJson<InstanceParticipantsData>(stringData);

            // Send data to subscriptions
            if (_GetInstanceParticipantsEvent != null) {
                _GetInstanceParticipantsEvent(data);
            }

            //¡ Clear delegates
            _GetInstanceParticipantsEvent = null;
        }

        public void ReceiveHardwareAcceleration (string stringData) {

            // Parse string
            HardwareAccelerationData data = JsonUtility.FromJson<HardwareAccelerationData>(stringData);

            // Send data to subscriptions
            if (_HardwareAccelerationEvent != null) {
                _HardwareAccelerationEvent(data);
            }

            //¡ Clear delegates
            _HardwareAccelerationEvent = null;
        }

        public void ReceiveChannel (string stringData) {

            // Parse string
            Channel data = JsonUtility.FromJson<Channel>(stringData);

            // Send data to subscriptions
            if (_GetChannelEvent != null) {
                _GetChannelEvent(data);
            }

            //¡ Clear delegates
            _GetChannelEvent = null;
        }

        public void ReceiveChannelPermissions (string stringData) {

            // Parse string
            ChannelPermissionsData data = JsonUtility.FromJson<ChannelPermissionsData>(stringData);

            // Send data to subscriptions
            if (_GetChannelPermissionsEvent != null) {
                _GetChannelPermissionsEvent(data);
            }

            //¡ Clear delegates
            _GetChannelPermissionsEvent = null;
        }

        public void ReceivePlatformBehaviors (string stringData) {

            // Parse string
            PlatformBehaviorsData data = JsonUtility.FromJson<PlatformBehaviorsData>(stringData);

            // Send data to subscriptions
            if (_GetPlatformBehaviorsEvent != null) {
                _GetPlatformBehaviorsEvent(data);
            }

            //¡ Clear delegates
            _GetPlatformBehaviorsEvent = null;
        }

        public void ReceiveImageUpload (string stringData) {

            // Parse string
            ImageUploadData data = JsonUtility.FromJson<ImageUploadData>(stringData);

            // Send data to subscriptions
            if (_ImageUploadEvent != null) {
                _ImageUploadEvent(data);
            }

            //¡ Clear delegates
            _ImageUploadEvent = null;
        }

        public void ReceiveLocale (string stringData) {

            // Parse string
            LocaleData data = JsonUtility.FromJson<LocaleData>(stringData);

            // Send data to subscriptions
            if (_GetLocaleEvent != null) {
                _GetLocaleEvent(data);
            }

            //¡ Clear delegates
            _GetLocaleEvent = null;
        }

        public void ReceiveSetConfig (string stringData) {

            // Parse string
            ConfigData data = JsonUtility.FromJson<ConfigData>(stringData);

            // Send data to subscriptions
            if (_SetConfigEvent != null) {
                _SetConfigEvent(data);
            }

            //¡ Clear delegates
            _SetConfigEvent = null;
        }

    }
}
