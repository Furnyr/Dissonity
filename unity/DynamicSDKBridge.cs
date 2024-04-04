using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static StaticSDKBridge;

// Will receive data from the IFrameBridge and read/write the StaticSDKBridge
public class DynamicSDKBridge: MonoBehaviour
{
    // Initialization
    void Awake () {

        //? Not initialize
        if (!initialized) {

            initialized = true;
            InitializeSDKBridge();
        }
    }

    //# BRIDGE METHODS - - - - -
    public void VoiceStateUpdate (string stringData) {
        
        VoiceStateUpdateData data = JsonUtility.FromJson<VoiceStateUpdateData>(stringData);
        
        // Send data to subscriptions
        VoiceStateUpdateHandler(data);
    }

    public void SpeakingStart (string stringData) {

        SpeakingData data = JsonUtility.FromJson<SpeakingData>(stringData);

        // Send data to subscriptions
        SpeakingStartHandler(data);
    }

    public void SpeakingStop (string stringData) {

        SpeakingData data = JsonUtility.FromJson<SpeakingData>(stringData);

        // Send data to subscriptions
        SpeakingStopHandler(data);
    }

    public void ActivityLayoutModeUpdate (string stringData) {
        
        ActivityLayoutModeUpdateData data = JsonUtility.FromJson<ActivityLayoutModeUpdateData>(stringData);

        // Send data to subscriptions
        ActivityLayoutModeUpdateHandler(data);
    }

    public void OrientationUpdate (string stringData) {

        OrientationUpdateData data = JsonUtility.FromJson<OrientationUpdateData>(stringData);

        // Send data to subscriptions
        OrientationUpdateHandler(data);
    }

    public void CurrentUserUpdate (string stringData) {

        CurrentUserUpdateData data = JsonUtility.FromJson<CurrentUserUpdateData>(stringData);

        // Send data to subscriptions
        CurrentUserUpdateHandler(data);
    }

    public void EntitlementCreate (string stringData) {
        
        EntitlementCreateData data = JsonUtility.FromJson<EntitlementCreateData>(stringData);

        // Send data to subscriptions
        EntitlementCreateHandler(data);
    }

    public void ThermalStateUpdate (string stringData) {
        
        ThermalStateUpdateData data = JsonUtility.FromJson<ThermalStateUpdateData>(stringData);

        // Send data to subscriptions
        ThermalStateUpdateHandler(data);
    }

    public void ActivityInstanceParticipantsUpdate (string stringData) {

        InstanceParticipantsData data = JsonUtility.FromJson<InstanceParticipantsData>(stringData);

        // Send data to subscriptions
        ActivityInstanceParticipantsUpdateHandler(data);
    }

    //# BRIDGE GET METHODS - - - - -
    public void ReceiveSDKInstanceId (string id) {

        //? Already cached
        if (instanceId != null) {

            // Send data to subscriptions
            GetInstanceIdHandler(instanceId);

            //¡ Clear delegates
            foreach(Delegate del in GetInstanceIdHandler.GetInvocationList())
            {
                GetInstanceIdHandler -= (GetIdDelegate)del;
            }

            return;
        }

        // Set cache
        instanceId = id;

        // Send data to subscriptions
        GetInstanceIdHandler(id);

        //¡ Clear delegates
        foreach(Delegate del in GetInstanceIdHandler.GetInvocationList())
        {
            GetInstanceIdHandler -= (GetIdDelegate)del;
        }
    }

    public void ReceiveChannelId (string id) {

        //? Already cached
        if (channelId != null) {

            // Send data to subscriptions
            GetChannelIdHandler(channelId);

            //¡ Clear delegates
            foreach(Delegate del in GetChannelIdHandler.GetInvocationList())
            {
                GetChannelIdHandler -= (GetIdDelegate)del;
            }

            return;
        }

        // Set cache
        channelId = id;

        // Send data to subscriptions
        GetChannelIdHandler(id);

        //¡ Clear delegates
        foreach(Delegate del in GetChannelIdHandler.GetInvocationList())
        {
            GetChannelIdHandler -= (GetIdDelegate)del;
        }
    }

    public void ReceiveGuildId (string id) {

        //? Already cached
        if (guildId != null) {

            // Send data to subscriptions
            GetGuildIdHandler(guildId);

            //¡ Clear delegates
            foreach(Delegate del in GetGuildIdHandler.GetInvocationList())
            {
                GetGuildIdHandler -= (GetIdDelegate)del;
            }

            return;
        }

        // Set cache
        guildId = id;

        // Send data to subscriptions
        GetGuildIdHandler(id);

        //¡ Clear delegates
        foreach(Delegate del in GetGuildIdHandler.GetInvocationList())
        {
            GetGuildIdHandler -= (GetIdDelegate)del;
        }
    }

    public void ReceiveUser (string stringData) {

        //? Already cached
        if (user != null) {

            // Send data to subscriptions
            GetUserHandler(user);

            //¡ Clear delegates
            foreach(Delegate del in GetUserHandler.GetInvocationList())
            {
                GetUserHandler -= (GetUserDelegate)del;
            }

            return;
        }

        // Parse string
        User data = JsonUtility.FromJson<User>(stringData);

        // Set cache
        user = data;

        // Send data to subscriptions
        GetUserHandler(data);

        //¡ Clear delegates
        foreach(Delegate del in GetUserHandler.GetInvocationList())
        {
            GetUserHandler -= (GetUserDelegate)del;
        }
    }

    public void ReceiveInstanceParticipants (string stringData) {

        // Parse string
        InstanceParticipantsData data = JsonUtility.FromJson<InstanceParticipantsData>(stringData);

        // Send data to subscriptions
        GetInstanceParticipantsHandler(data);

        //¡ Clear delegates
        foreach(Delegate del in GetInstanceParticipantsHandler.GetInvocationList())
        {
            GetInstanceParticipantsHandler -= (InstanceParticipantsDelegate)del;
        }
    }
}