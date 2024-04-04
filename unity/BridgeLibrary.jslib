
var BridgeLibrary = {
  // Must be called inside Unity
  InitializeIFrameBridge: function() {
    console.log("BridgeLibrary: Initialized");
    window.addEventListener("message", _IFrameBrige);
  },
  // Must be IMPORTED inside Unity
  IFrameBrige: function({ data: messageData }) {
    switch (messageData.command) {
      case "DISPATCH": {
        if (!messageData.event)
          throw new Error("Event not set");
        switch (messageData.event) {
          case "VOICE_STATE_UPDATE":
            unityInstance.SendMessage("DynamicSDKBridge", "VoiceStateUpdate", JSON.stringify(messageData.data));
            break;
          case "SPEAKING_START":
            unityInstance.SendMessage("DynamicSDKBridge", "SpeakingStart", JSON.stringify(messageData.data));
            break;
          case "SPEAKING_STOP":
            unityInstance.SendMessage("DynamicSDKBridge", "SpeakingStop", JSON.stringify(messageData.data));
            break;
          case "ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE":
            unityInstance.SendMessage("DynamicSDKBridge", "ActivityInstanceParticipantsUpdate", JSON.stringify(messageData.data));
            break;
          case "ACTIVITY_LAYOUT_MODE_UPDATE":
            unityInstance.SendMessage("DynamicSDKBridge", "ActivityLayoutModeUpdate", JSON.stringify(messageData.data));
            break;
          case "ORIENTATION_UPDATE":
            unityInstance.SendMessage("DynamicSDKBridge", "OrientationUpdate", JSON.stringify(messageData.data));
            break;
          case "CURRENT_USER_UPDATE":
            unityInstance.SendMessage("DynamicSDKBridge", "CurrentUserUpdate", JSON.stringify(messageData.data));
            break;
          case "THERMAL_STATE_UPDATE":
            unityInstance.SendMessage("DynamicSDKBridge", "ThermalStateUpdate", JSON.stringify(messageData.data));
            break;
          case "ENTITLEMENT_CREATE":
            unityInstance.SendMessage("DynamicSDKBridge", "EntitlementCreate", JSON.stringify(messageData.data));
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
        unityInstance.SendMessage("DynamicSDKBridge", methodName, messageData.data);
        break;
      }
      case "GET_USER": {
        unityInstance.SendMessage("DynamicSDKBridge", "ReceiveUser", JSON.stringify(messageData.data));
        break;
      }
      case "SET_ACTIVITY": {
        break;
        unityInstance.SendMessage("DynamicSDKBridge", "ReceiveSetActivity", JSON.stringify(messageData.data));
        break;
      }
      case "GET_INSTANCE_PARTICIPANTS": {
        unityInstance.SendMessage("DynamicSDKBridge", "ReceiveInstanceParticipants", JSON.stringify(messageData.data));
        break;
      }
    }
  },
  Subscribe: function(event) {
    event = UTF8ToString(event);
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
      case "ACTIVITY_LAYOUT_MODE_UPDATE": {
        window.parent.postMessage({
          command: "SUBSCRIBE",
          event
        });
        break;
      }
    }
  },
  Unsubscribe: function(event) {
    event = UTF8ToString(event);
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
      case "ACTIVITY_LAYOUT_MODE_UPDATE": {
        window.parent.postMessage({
          command: "UNSUBSCRIBE",
          event
        });
        break;
      }
    }
  },
  RequestInstanceId: function() {
    window.parent.postMessage({
      command: "GET_INSTANCE_ID"
    });
  },
  RequestChannelId: function() {
    window.parent.postMessage({
      command: "GET_CHANNEL_ID"
    });
  },
  RequestGuildId: function() {
    window.parent.postMessage({
      command: "GET_GUILD_ID"
    });
  },
  RequestUser: function() {
    window.parent.postMessage({
      command: "GET_USER"
    });
  },
  RequestInstanceParticipants: function() {
    window.parent.postMessage({
      command: "GET_INSTANCE_PARTICIPANTS"
    });
  }
};

mergeInto(LibraryManager.library, BridgeLibrary);