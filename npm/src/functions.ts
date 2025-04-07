
import { DiscordSDK } from "@discord/embedded-app-sdk";
import fetch from "cross-fetch";

import type { ConfigOptions, CompatibleUser, MessageData, MessageParentCommand, DiscordSDKEvents, DataPromise } from "./types";

// Unity compatible version
const PACKAGE_VERSION = "1.5.0";

let initialized = false;


//* Stringifies JSON with BigInts
function stringifyBigInt (obj: Record<string, unknown>): string {

    const str = JSON.stringify(obj, (key, value) => {

        //? Bigint
        if (typeof value == "bigint") {
            return value.toString();
        }

        return value;
    });

    return str;
}

//* Get the child iframe
function getChildIframe(): HTMLIFrameElement {

    const iframe = document.getElementById("dissonity-child") as HTMLIFrameElement | null;

    if (iframe == null) {
        throw new Error("No iframe with id 'dissonity-child' found");
    }

    return iframe;
}

//\ Initialize the SDK
async function initializeSdk(options: ConfigOptions): Promise<{ discordSdk: DiscordSDK, user: CompatibleUser, access_token: string | null }> {

    //\ Initialize
    const discordSdk = new DiscordSDK(options.clientId);
    await discordSdk.ready();

    // Pop open the OAuth permission modal and request for access to scopes listed in scope array below
    const { code } = await discordSdk.commands.authorize({
        client_id: options.clientId,
        response_type: "code",
        state: "",
        prompt: "none",
        scope: options.scope,
    });

    let headers: Record<string, string> = {
        "Content-Type": "application/json",
    }

    if('apiKey' in options) {
        headers["Api-Key"] = `${options.apiKey}`;
    }

    //\ Retrieve access token from the embedded app's server
    const response = await fetch(`/.proxy${options.tokenRoute}`, {
        method: options.method,
        headers: headers,
        body: JSON.stringify({ code }),
    });

    const data = await response.json();

    //? No access token
    if (!data.access_token) {
        throw new Error("No access_token field found in response data");
    }

    //\ Authenticate with Discord client (using the access token)
    const { user } = await discordSdk.commands.authenticate({
        access_token: data.access_token,
    });

    // Better compatibility with the SDKBridge
    (user as CompatibleUser).flags = user.public_flags;
    (user as CompatibleUser).bot = false;

    return { discordSdk, user: (user as CompatibleUser), access_token: data.access_token };
}

//\ Handle received message
async function receiveMessage(discordSdk: DiscordSDK, user: CompatibleUser | null, access_token: string, messageData: MessageData) {

    const { nonce, event, command } = messageData;
    let args = messageData.args ?? {};

    // Sends a message to child iframe
    function handleSubscribeEvent(eventData: Record<string, unknown>) {

        getChildIframe().contentWindow?.postMessage(
            {
                event,
                command: "DISPATCH",
                data: eventData,
            },
            "*"
        );
    }

    // Handle the command
    switch (command as MessageParentCommand) {

        case "SUBSCRIBE": {

            if (event == null) {
                throw new Error("SUBSCRIBE event is undefined");
            }

            //? Use channel id
            if (args.channel_id) args = { channel_id: discordSdk!.channelId };

            try {
                discordSdk!.subscribe(event as DiscordSDKEvents, handleSubscribeEvent, args as any);
            } catch (_err) {
                console.error(`Dissonity NPM: Error attempting to subscribe to: ${event}. You may need some scope?`);
            }
            break;
        }

        case "UNSUBSCRIBE": {

            if (event == null) {
                throw new Error("UNSUBSCRIBE event is undefined");
            }

            //? Use channel id
            if (args.channel_id) args = { channel_id: discordSdk!.channelId };

            discordSdk!.unsubscribe(event as DiscordSDKEvents, handleSubscribeEvent);
            break;
        }

        case "SET_ACTIVITY": {

            //? No activity
            if (!args.activity) {
                throw new Error("No activity provided for SET_ACTIVITY");
            }

            //\ Clean unnecessary fields
            if (args.activity.assets?.large_image == "") delete args.activity.assets;
            if (args.activity.party?.id == "") delete args.activity.party;
            if (args.activity.emoji?.id == "") delete args.activity.emoji;
            if (args.activity.secrets?.match == "") delete args.activity.secrets;

            try {
                const data = await discordSdk!.commands.setActivity(args);
                getChildIframe().contentWindow?.postMessage({ nonce, event, command, data }, "*");

            } catch (_err) {
                console.error("Dissonity NPM: Error attempting to set the activity. You may need the 'rpc.activities.write' scope.")
            }
            break;
        }

        case "GET_ACCESS_TOKEN": {
            if (access_token == null) throw new Error("You need to be authenticated to get the current access_token");
            getChildIframe().contentWindow?.postMessage({ nonce, command, data: access_token, args }, "*");
            break;
        }

        case "GET_APPLICATION_ID": {

            const { clientId } = discordSdk!;
            getChildIframe().contentWindow?.postMessage({ nonce, command, data: clientId, args }, "*");
            break;
        }

        case "GET_INSTANCE_ID": {

            const { instanceId } = discordSdk!;
            getChildIframe().contentWindow?.postMessage({ nonce, command, data: instanceId, args }, "*");
            break;
        }

        case "GET_CHANNEL_ID": {

            const { channelId } = discordSdk!;
            getChildIframe().contentWindow?.postMessage({ nonce, command, data: channelId, args }, "*");
            break;
        }

        case "GET_GUILD_ID": {

            const { guildId } = discordSdk!;
            getChildIframe().contentWindow?.postMessage({ nonce, command, data: guildId, args }, "*");
            break;
        }

        case "GET_USER_ID": {

            if (user == null) throw new Error("You need to be authenticated to get the current user id");
            getChildIframe().contentWindow?.postMessage({ nonce, command, data: user.id, args }, "*");
            break;
        }

        case "GET_USER": {

            if (user == null) throw new Error("You need to be authenticated to get the current user");
            getChildIframe().contentWindow?.postMessage({ nonce, command, data: user, args }, "*");
            break;
        }

        case "GET_INSTANCE_PARTICIPANTS": {

            const data = await discordSdk!.commands.getInstanceConnectedParticipants();
            getChildIframe().contentWindow?.postMessage({ nonce, command, data, args }, "*");
            break;
        }

        case "HARDWARE_ACCELERATION": {

            const data = await discordSdk!.commands.encourageHardwareAcceleration();
            getChildIframe().contentWindow?.postMessage({ nonce, command, data, args }, "*");
            break;
        }

        case "GET_CHANNEL": {

            //? No channel id
            if (!args.channel_id) {
                throw new Error("No channel id provided for GET_CHANNEL");
            }

            try {
                const data = await discordSdk!.commands.getChannel(args);
                getChildIframe().contentWindow?.postMessage({ nonce, command, data, args }, "*");

            } catch (_err) {
                console.error("Dissonity NPM: Error attempting to get the channel. You may need the 'guilds' scope.")
            }

            break;
        }

        case "GET_CHANNEL_PERMISSIONS": {

            //? No channel id
            if (!args.channel_id) {
                throw new Error("No channel id provided for GET_CHANNEL_PERMISSIONS");
            }

            try {
                const data = await discordSdk!.commands.getChannelPermissions(args);
                getChildIframe().contentWindow?.postMessage({ nonce, command, data: stringifyBigInt(data), args }, "*");

            } catch (_err) {
                console.error("Dissonity NPM: Error attempting to get the channel permissions. You may need the 'guilds.members.read' scope.")
            }

            break;
        }

        case "GET_ENTITLEMENTS": {

            throw new Error("Entitlements are not supported in Dissonity v1");

            const data = await discordSdk!.commands.getEntitlements();
            getChildIframe().contentWindow?.postMessage({ nonce, command, data, args }, "*");
            break;
        }

        case "GET_PLATFORM_BEHAVIORS": {

            const data = await discordSdk!.commands.getPlatformBehaviors();
            getChildIframe().contentWindow?.postMessage({ nonce, command, data, args }, "*");
            break;
        }

        case "GET_SKUS": {

            throw new Error("Skus are not supported in Dissonity v1");

            const data = await discordSdk!.commands.getSkus();
            getChildIframe().contentWindow?.postMessage({ nonce, command, data, args }, "*");
            break;
        }

        case "IMAGE_UPLOAD": {

            try {
                const data = await discordSdk!.commands.initiateImageUpload();
                getChildIframe().contentWindow?.postMessage({ nonce, command, data: { image_url: data.image_url, canceled: false }, args }, "*");

            } catch (_err) {
                getChildIframe().contentWindow?.postMessage({ nonce, command, data: { image_url: "", canceled: true }, args }, "*");
            }

            break;
        }

        case "EXTERNAL_LINK": {

            //? No url
            if (!args.url) {
                throw new Error("No url provided for EXTERNAL_LINK");
            }

            discordSdk!.commands.openExternalLink(args);
            break;
        }

        case "INVITE_DIALOG": {

            discordSdk!.commands.openInviteDialog();
            break;
        }

        case "SHARE_MOMENT_DIALOG": {

            //? No media url
            if (!args.mediaUrl) {
                throw new Error("No media url provided for SHARE_MOMENT_DIALOG");
            }

            discordSdk!.commands.openShareMomentDialog(args);
            break;
        }

        case "SET_ORIENTATION_LOCK_STATE": {

            //? No lock state
            if (!args.lock_state) {
                throw new Error("No lock state provided for SET_ORIENTATION_LOCK_STATE");
            }

            try {
                discordSdk!.commands.setOrientationLockState(args);
            } catch (_err) {
                console.error("Dissonity NPM: Error attempting to set the orientation lock state. You may need the 'guilds.members.read' scope.")
            }

            break;
        }

        case "START_PURCHASE": {

            throw new Error("Purchases are not supported in Dissonity v1");

            discordSdk!.commands.startPurchase(args);
            break;
        }

        case "GET_LOCALE": {

            try {
                const data = await discordSdk!.commands.userSettingsGetLocale();
                getChildIframe().contentWindow?.postMessage({ nonce, command, data, args }, "*");

            } catch (_err) {
                console.error("Dissonity NPM: Error attempting to get the user locale. You may need the 'identify' scope.")
            }

            break;
        }

        case "SET_CONFIG": {

            //? No field
            if (!args.use_interactive_pip) {
                throw new Error("No 'use interactive pip' provided for SET_CONFIG");
            }

            const data = await discordSdk!.commands.setConfig(args);
            getChildIframe().contentWindow?.postMessage({ nonce, command, data, args }, "*");
            break;
        }

        case "PING_LOAD": {

            getChildIframe().contentWindow?.postMessage({ command: "LOADED", data: PACKAGE_VERSION }, "*");

            break;
        }
    }
}

//\ Initialize the SDK and setup the bridge
export async function setupSdk(options: ConfigOptions) {

    if (initialized) throw new Error("Already initialized");

    initialized = true;

    const dataPromise = initializeSdk(options);
    let discordSdk: DiscordSDK | null = null;
    let user: CompatibleUser | null = null;
    let access_token: string | null = null;
    async function handleMessage({ data: messageData }: MessageEvent<MessageData>) {

        // Discard empty objects
        if (typeof messageData !== "object" || Array.isArray(messageData) || messageData === null) {
            return;
        }

        //? Not yet resolved
        if (!discordSdk! || !user) {
            await dataPromise;
        }

        receiveMessage(discordSdk!, user!, access_token, messageData);
    }

    //\ Setup message event handler
    window.addEventListener("message", handleMessage);

    // Wait for promise resolution
    const resolvedData = await dataPromise;
    discordSdk = resolvedData.discordSdk;
    user = resolvedData.user;
    access_token = resolvedData.access_token;
    getChildIframe().contentWindow?.postMessage({ command: "LOADED", data: PACKAGE_VERSION }, "*");
}

//\ Use external data and setup bridge
export async function useSdk(dataPromise: DataPromise) {

    if (initialized) throw new Error("Already initialized");

    initialized = true;

    let discordSdk: DiscordSDK | null = null;
    let user: CompatibleUser | null = null;

    async function handleMessage({ data: messageData }: MessageEvent<MessageData>) {

        // Discard empty objects
        if (typeof messageData !== "object" || Array.isArray(messageData) || messageData === null) {
            return;
        }

        //? Not yet resolved
        if (!discordSdk! || !user) {
            await dataPromise;
        }

        receiveMessage(discordSdk!, user, messageData);
    }

    //\ Setup message event handler
    window.addEventListener("message", handleMessage);

    // Wait for promise resolution
    const resolvedData = await dataPromise;
    discordSdk = resolvedData.discordSdk;

    if (resolvedData.user != null) {

        // Better compatibility with the SDKBridge
        user = {...resolvedData.user} as CompatibleUser;
        user.flags = user.public_flags;
        user.bot = false;
    }

    getChildIframe().contentWindow?.postMessage({ command: "LOADED", data: PACKAGE_VERSION }, "*");
}

export { DiscordSDK };