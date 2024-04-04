/**
 * You can code the .jslib for Unity here!
 * When you're done, run (npm run unity) and, inside _unity_structures/BridgeLibrary.js,
 * use the BridgeLibrary object inside your Unity .jslib
 * 
 * Example:
 * 
 * Bridge.jslib - - - - -
 * var BridgeLibrary = {...}
 * 
 * mergeInto(LibraryManager.library, BridgeLibrary);
 */

import type { MessageData, DiscordSDKEvents, MessageChildCommand} from '../utils/types';


//? StateBridge, { key: value } Child(JSlib => C#)
// SDKBrige, { event: data } Child(JSlib => C#)
// IFrameBrige, ... Child(JSlib) => Parent

declare const unityInstance: { SendMessage: (gameObject: string, method: string, value: any) => void };
declare const _IFrameBrige: (data: MessageData) => void;
declare const UTF8ToString: (str: any) => string;

export const BridgeLibrary = {
  // Must be called inside Unity
  InitializeIFrameBridge: function (): void {
    console.log("BridgeLibrary: Initialized");
    window.addEventListener("message", _IFrameBrige);
  },

  // Must be IMPORTED inside Unity
  IFrameBrige: function ({ data: messageData }: MessageEvent<MessageData>) {

    switch (messageData.command as MessageChildCommand) {
      case "DISPATCH": {
        
        //? No event
        if (!messageData.event) throw new Error("Event not set");

        // SDK Bridge
        switch (messageData.event) {
          case "VOICE_STATE_UPDATE":
            unityInstance.SendMessage("DynamicSDKBridge", "VoiceStateUpdate", JSON.stringify(messageData.data))
            break;
  
          case "SPEAKING_START":
            unityInstance.SendMessage("DynamicSDKBridge", "SpeakingStart", JSON.stringify(messageData.data))
            break;
  
          case "SPEAKING_STOP":
            unityInstance.SendMessage("DynamicSDKBridge", "SpeakingStop", JSON.stringify(messageData.data))
            break;
  
          case "ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE":
            unityInstance.SendMessage("DynamicSDKBridge", "ActivityInstanceParticipantsUpdate", JSON.stringify(messageData.data))
            break;
  
          case "ACTIVITY_LAYOUT_MODE_UPDATE":
            unityInstance.SendMessage("DynamicSDKBridge", "ActivityLayoutModeUpdate", JSON.stringify(messageData.data))
            break;
  
          case "ORIENTATION_UPDATE":
            unityInstance.SendMessage("DynamicSDKBridge", "OrientationUpdate", JSON.stringify(messageData.data))
            break;
  
          case "CURRENT_USER_UPDATE":
            unityInstance.SendMessage("DynamicSDKBridge", "CurrentUserUpdate", JSON.stringify(messageData.data))
            break;
  
          case "THERMAL_STATE_UPDATE":
            unityInstance.SendMessage("DynamicSDKBridge", "ThermalStateUpdate", JSON.stringify(messageData.data))
            break;
  
          case "ENTITLEMENT_CREATE":
            unityInstance.SendMessage("DynamicSDKBridge", "EntitlementCreate", JSON.stringify(messageData.data))
            break;
        }
  
        break;
      }

      case "GET_CHANNEL_ID":
      case "GET_GUILD_ID":
      case "GET_INSTANCE_ID": {

        let methodName = "";

        switch (messageData.command) {
          case "GET_INSTANCE_ID": {

            methodName = "ReceiveSDKInstanceId";
            break;
          }

          case "GET_GUILD_ID": {
            methodName = "ReceiveGuildId";
            break;
          }

          case "GET_CHANNEL_ID": {
            methodName = "ReceiveChannelId";
            break;
          }
        }

        // SDK Bridge
        unityInstance.SendMessage("DynamicSDKBridge", methodName, messageData.data);

        break;
      }

      case "GET_USER": {

        // SDK Bridge
        unityInstance.SendMessage("DynamicSDKBridge", "ReceiveUser", JSON.stringify(messageData.data));

        break;
      }

      case "SET_ACTIVITY": {
        
        break;

        // SDK Bridge
        unityInstance.SendMessage("DynamicSDKBridge", "ReceiveSetActivity", JSON.stringify(messageData.data));

        break;
      }

      case "GET_INSTANCE_PARTICIPANTS": {

        // SDK Bridge
        unityInstance.SendMessage("DynamicSDKBridge", "ReceiveInstanceParticipants", JSON.stringify(messageData.data));

        break;
      }
    }
  },

  Subscribe: function (event: DiscordSDKEvents) {

    event = UTF8ToString(event) as DiscordSDKEvents;

    switch (event) {
      case "VOICE_STATE_UPDATE": {

        window.parent.postMessage({
          command: "SUBSCRIBE",
          event,
          args: { channel_id: "channel_id" }
        });
  
        break;
      }

      case "SPEAKING_STOP":
      case "SPEAKING_START": {

        window.parent.postMessage({
          command: "SUBSCRIBE",
          event,
          args: {}
        });

        break;
      }

      case "ENTITLEMENT_CREATE":
      case "THERMAL_STATE_UPDATE":
      case "CURRENT_USER_UPDATE":
      case "ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE":
      case "ORIENTATION_UPDATE":
      case "ACTIVITY_LAYOUT_MODE_UPDATE" : {

        window.parent.postMessage({
          command: "SUBSCRIBE",
          event
        });

        break;
      }
    }
  },

  Unsubscribe: function (event: DiscordSDKEvents) {

    event = UTF8ToString(event) as DiscordSDKEvents;

    switch (event) {
      case "VOICE_STATE_UPDATE": {

        window.parent.postMessage({
          command: "UNSUBSCRIBE",
          event,
          args: { channel_id: "channel_id" }
        });

        break;
      }

      case "SPEAKING_STOP":
      case "SPEAKING_START": {

        window.parent.postMessage({
          command: "UNSUBSCRIBE",
          event,
          args: {}
        });

        break;
      }

      case "ENTITLEMENT_CREATE":
      case "THERMAL_STATE_UPDATE":
      case "CURRENT_USER_UPDATE":
      case "ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE":
      case "ORIENTATION_UPDATE":
      case "ACTIVITY_LAYOUT_MODE_UPDATE" : {

        window.parent.postMessage({
          command: "UNSUBSCRIBE",
          event
        });

        break;
      }
    }
  },

  RequestInstanceId: function () {

    window.parent.postMessage({
      command: "GET_INSTANCE_ID"
    });
  },

  RequestChannelId: function () {

    window.parent.postMessage({
      command: "GET_CHANNEL_ID"
    });
  },

  RequestGuildId: function () {

    window.parent.postMessage({
      command: "GET_GUILD_ID"
    });
  },

  RequestUser: function () {

    window.parent.postMessage({
      command: "GET_USER"
    });
  },

  RequestInstanceParticipants: function () {

    window.parent.postMessage({
      command: "GET_INSTANCE_PARTICIPANTS"
    });
  }
}