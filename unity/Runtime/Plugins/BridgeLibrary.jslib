
var BridgeLibrary = {
    InitializeIFrameBridge: function () {
        window.addEventListener("message", _IFrameBridge);
    },
    IFrameBridge: function ({ data: messageData }) {
        switch (messageData.command) {
            case "DISPATCH": {
                if (!messageData.event)
                    throw new Error("Event not set");
                switch (messageData.event) {
                    case "VOICE_STATE_UPDATE":
                        unityInstance.SendMessage("DiscordBridge", "VoiceStateUpdate", JSON.stringify(messageData.data));
                        break;
                    case "SPEAKING_START":
                        unityInstance.SendMessage("DiscordBridge", "SpeakingStart", JSON.stringify(messageData.data));
                        break;
                    case "SPEAKING_STOP":
                        unityInstance.SendMessage("DiscordBridge", "SpeakingStop", JSON.stringify(messageData.data));
                        break;
                    case "ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE":
                        unityInstance.SendMessage("DiscordBridge", "ActivityInstanceParticipantsUpdate", JSON.stringify(messageData.data));
                        break;
                    case "ACTIVITY_LAYOUT_MODE_UPDATE":
                        unityInstance.SendMessage("DiscordBridge", "ActivityLayoutModeUpdate", JSON.stringify(messageData.data));
                        break;
                    case "ORIENTATION_UPDATE":
                        unityInstance.SendMessage("DiscordBridge", "OrientationUpdate", JSON.stringify(messageData.data));
                        break;
                    case "CURRENT_USER_UPDATE":
                        unityInstance.SendMessage("DiscordBridge", "CurrentUserUpdate", JSON.stringify(messageData.data));
                        break;
                    case "THERMAL_STATE_UPDATE":
                        unityInstance.SendMessage("DiscordBridge", "ThermalStateUpdate", JSON.stringify(messageData.data));
                        break;
                    case "ENTITLEMENT_CREATE":
                        unityInstance.SendMessage("DiscordBridge", "EntitlementCreate", JSON.stringify(messageData.data));
                        break;
                }
                break;
            }
            case "GET_CHANNEL_ID":
            case "GET_GUILD_ID":
            case "GET_USER_ID":
            case "GET_INSTANCE_ID": {
                let methodName = "";
                switch (messageData.command) {
                    case "GET_USER_ID": {
                        methodName = "ReceiveUserId";
                        break;
                    }
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
                unityInstance.SendMessage("DiscordBridge", methodName, messageData.data);
                break;
            }
            case "GET_USER": {
                unityInstance.SendMessage("DiscordBridge", "ReceiveUser", JSON.stringify(messageData.data));
                break;
            }
            case "SET_ACTIVITY": {
                unityInstance.SendMessage("DiscordBridge", "ReceiveSetActivity", JSON.stringify(messageData.data));
                break;
            }
            case "GET_INSTANCE_PARTICIPANTS": {
                unityInstance.SendMessage("DiscordBridge", "ReceiveInstanceParticipants", JSON.stringify(messageData.data));
                break;
            }
            case "HARDWARE_ACCELERATION": {
                unityInstance.SendMessage("DiscordBridge", "ReceiveHardwareAcceleration", JSON.stringify(messageData.data));
                break;
            }
            case "GET_CHANNEL": {
                unityInstance.SendMessage("DiscordBridge", "ReceiveChannel", JSON.stringify(messageData.data));
                break;
            }
            case "GET_CHANNEL_PERMISSIONS": {
                unityInstance.SendMessage("DiscordBridge", "ReceiveChannelPermissions", messageData.data);
                break;
            }
            case "GET_ENTITLEMENTS": {
                unityInstance.SendMessage("DiscordBridge", "ReceiveEntitlements", JSON.stringify(messageData.data));
                break;
            }
            case "GET_PLATFORM_BEHAVIORS": {
                unityInstance.SendMessage("DiscordBridge", "ReceivePlatformBehaviors", JSON.stringify(messageData.data));
                break;
            }
            case "GET_SKUS": {
                unityInstance.SendMessage("DiscordBridge", "ReceiveSkus", JSON.stringify(messageData.data));
                break;
            }
            case "IMAGE_UPLOAD": {
                unityInstance.SendMessage("DiscordBridge", "ReceiveImageUpload", JSON.stringify(messageData.data));
                break;
            }
            case "GET_LOCALE": {
                unityInstance.SendMessage("DiscordBridge", "ReceiveLocale", JSON.stringify(messageData.data));
                break;
            }
            case "SET_CONFIG": {
                unityInstance.SendMessage("DiscordBridge", "ReceiveSetConfig", JSON.stringify(messageData.data));
                break;
            }
            case "LOADED": {
                unityInstance.SendMessage("DiscordBridge", "NpmLoad", messageData.data);
                break;
            }
        }
    },
    Subscribe: function (event) {
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
    Unsubscribe: function (event) {
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
    RequestSetActivity: function (stringifiedActivity) {
        stringifiedActivity = UTF8ToString(stringifiedActivity);
        window.parent.postMessage({
            command: "SET_ACTIVITY",
            args: { activity: JSON.parse(stringifiedActivity) }
        });
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
    RequestUserId: function () {
        window.parent.postMessage({
            command: "GET_USER_ID"
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
    },
    RequestHardwareAcceleration: function () {
        window.parent.postMessage({
            command: "HARDWARE_ACCELERATION"
        });
    },
    RequestChannel: function (channelId) {
        channelId = UTF8ToString(channelId);
        window.parent.postMessage({
            command: "GET_CHANNEL",
            args: {
                channel_id: channelId
            }
        });
    },
    RequestChannelPermissions: function (channelId) {
        channelId = UTF8ToString(channelId);
        window.parent.postMessage({
            command: "GET_CHANNEL_PERMISSIONS",
            args: {
                channel_id: channelId
            }
        });
    },
    RequestEntitlements: function () {
        window.parent.postMessage({
            command: "GET_ENTITLEMENTS"
        });
    },
    RequestPlatformBehaviors: function () {
        window.parent.postMessage({
            command: "GET_PLATFORM_BEHAVIORS"
        });
    },
    RequestSkus: function () {
        window.parent.postMessage({
            command: "GET_SKUS"
        });
    },
    RequestImageUpload: function () {
        window.parent.postMessage({
            command: "IMAGE_UPLOAD"
        });
    },
    RequestOpenExternalLink: function (url) {
        url = UTF8ToString(url);
        window.parent.postMessage({
            command: "EXTERNAL_LINK",
            args: {
                url
            }
        });
    },
    RequestInviteDialog: function () {
        window.parent.postMessage({
            command: "INVITE_DIALOG"
        });
    },
    RequestShareMomentDialog: function (mediaUrl) {
        mediaUrl = UTF8ToString(mediaUrl);
        window.parent.postMessage({
            command: "SHARE_MOMENT_DIALOG",
            args: {
                mediaUrl
            }
        });
    },
    RequestSetOrientationLockState: function (lockState, pictureInPictureLockState, gridLockState) {
        let picture_in_picture_lock_state;
        let grid_lock_state;
        if (pictureInPictureLockState)
            picture_in_picture_lock_state = UTF8ToString(pictureInPictureLockState);
        if (gridLockState)
            grid_lock_state = UTF8ToString(gridLockState);
        window.parent.postMessage({
            command: "SET_ORIENTATION_LOCK_STATE",
            args: {
                lock_state: lockState,
                picture_in_picture_lock_state,
                grid_lock_state
            }
        });
    },
    RequestPurchase: function () {
        window.parent.postMessage({
            command: "START_PURCHASE"
        });
    },
    RequestLocale: function () {
        window.parent.postMessage({
            command: "GET_LOCALE"
        });
    },
    RequestSetConfig: function (useInteractivePip) {
        useInteractivePip = UTF8ToString(useInteractivePip);
        window.parent.postMessage({
            command: "SET_CONFIG",
            args: {
                use_interactive_pip: (useInteractivePip == "True")
            }
        });
    },
    PingLoad: function () {
        window.parent.postMessage({
            command: "PING_LOAD"
        });
    }
};

mergeInto(LibraryManager.library, BridgeLibrary);