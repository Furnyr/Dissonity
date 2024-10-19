
/*

This is the "RpcBridge". It handles the communication with the Discord RPC.

It can interact directly with Unity, but Unity needs to go through the "InterfaceBridge"
to communicate with it.

It handles the handshake, authorization and authentication while the Unity build loads.

The "user-defined" variables are replaced by post-processing the Unity build.

Function names are PascalCase, C# method conventions.

*/


// Official methods
const { patchUrlMappings, formatPrice } : {
    patchUrlMappings: (mappings: Mapping[], config: PatchUrlMappingsConfig) => void,
    formatPrice: (price: {amount: number; currency: string}, locale: string) => string
} = await import("./official_utils.js" as string);

// Official types
import type { Mapping, PatchUrlMappingsConfig, HandshakePayload } from "./official/official_types";

// Types
import type { RpcBridgeCommands, DissonityBridgeMethods, ParseVariableType, RpcFramePayload, RpcBridgeMessage } from "./types";

//# CONSTANTS - - - - -
// Handshake
const HANDSHAKE_VERSION = 1;
const HANDSHAKE_ENCODING = "json";
const HANDSHAKE_SDK_VERSION_MINIUM_MOBILE_VERSION = 250;
const UNKNOWN_VERSION_NUMBER = -1;
let HANDSHAKE_SDK_VERSION = "[[[ SDK_VERSION ]]]§"; // Embedded App SDK version that Dissonity is simulating. This will be replaced at build time and parsed at runtime.

// Authorize
const RESPONSE_TYPE = "code";
const PROMPT = "none";

const OPCODES = {
    Handshake: 0,
    Frame: 1,
    Close: 2,
    Hello: 3
};
const CLOSE_CODES = {
    Normal: 1000
};
const ALLOWED_ORIGINS = new Set([
    window.location.origin,
    "https://discord.com",
    "https://discordapp.com",
    "https://ptb.discord.com",
    "https://ptb.discordapp.com",
    "https://canary.discord.com",
    "https://canary.discordapp.com",
    "https://staging.discord.co",
    "http://localhost:3333",
    "https://pax.discord.com",
    "null",
]);
const COMMANDS = {
    Dispatch: "DISPATCH",
    Authorize: "AUTHORIZE",
    Authenticate: "AUTHENTICATE"
};
const EVENTS = {
    Error: "ERROR",
    Ready: "READY"
};
const PLATFORMS = {
    Desktop: "desktop",
    Mobile: "mobile"
};
const STATE_CODES = {
    OutsideDiscord: -1,
    Errored: 0,
    Loading: 1,
    Ready: 2,
    Closed: 3
};
const _VARIABLE_SEPARATOR = "§"; //alt 21 win


//# VARIABLES - - - - -
let currentState = STATE_CODES.Loading;
let disableInfoLogs: string | boolean = "[[[ DISABLE_INFO_LOGS ]]]§";
let unityReady = false;

let clientId = "";
let mobileAppVersion: string | null = "";

// Unity data
let readyData = "";
let authorizeData = "";
let serverPayloadData = "";
let authenticateData = "";

// Listeners
let listeners: ((message: MessageEvent) => void)[] = [];

let _unityInstance: {
    SendMessage: (gameObject: string, method: string, value: any) => void;
} | null = null;



//# INITIALIZE - - - - -
function Handshake(): void {

    /*
    
    Process will be:
    - Listen(initialBridge, nonFrameOpcode);
    - (...)
    - RemoveListen(initialBridge); Listen(normalBridge)

    Then the connection is established, authorized and authenticated.
    
    */

    //? Not browser
    if (typeof window == "undefined") {
        currentState = STATE_CODES.OutsideDiscord;
        return;
    }

    //? Loaded in web
    const bridge = document.querySelector('script[data-web]')!;
    const isWeb = bridge.getAttribute('data-web') == "true";

    if (isWeb) {
        window.outsideDiscord = true;
        currentState = STATE_CODES.OutsideDiscord;
        return;
    }

    const query = RequestQuery();

    //? No query params
    if (!query || !query.frame_id || !query.instance_id || !query.platform) {
        window.outsideDiscord = true;
        currentState = STATE_CODES.OutsideDiscord;
        return;
    }

    disableInfoLogs = ParseBuildVariable(disableInfoLogs as string, "boolean") as boolean;

    AddListeners(InitialBridgeListener, NonFrameOpcode);

    const clientIdVariable = "[[[ CLIENT_ID ]]]§";
    
    clientId = ParseBuildVariable(clientIdVariable) as string;
    HANDSHAKE_SDK_VERSION = ParseBuildVariable(HANDSHAKE_SDK_VERSION) as string;

    mobileAppVersion = query.mobile_app_version ?? null;

    const majorMobileVersion = ParseMajorMobileVersion();

    const handshakePayload: HandshakePayload = {
        v: HANDSHAKE_VERSION,
        encoding: HANDSHAKE_ENCODING,
        client_id: clientId,
        frame_id: query.frame_id
    };

    if (query.platform === PLATFORMS.Desktop || majorMobileVersion >= HANDSHAKE_SDK_VERSION_MINIUM_MOBILE_VERSION) {
        handshakePayload["sdk_version"] = HANDSHAKE_SDK_VERSION;
    }

    InternalSend(
        [
            OPCODES.Handshake,
            handshakePayload
        ]
    );
}


//# DISCORD LISTENERS - - - - -
//@discord-rpc
function NonFrameOpcode(message: MessageEvent): void {

    if (!ALLOWED_ORIGINS.has(message.origin)) return;

    const opcode = message.data?.[0];

    //? Frame opcode
    if (opcode == OPCODES.Frame) return;

    switch (opcode) {

        case OPCODES.Hello: {
            // The Hello opcode probably needs to be handled here whenever it's implemented
            break;
        }

        case OPCODES.Close: {
            // The official implementation probably needs to be handled here, but forwarding to Unity if possible for future implementations

            StopListening();
    
            //? Forward message to Unity
            if (unityReady) {
                SendToUnity({ method: "_HandleMessage", payload: SerializePayload(message.data) })
            }
            break;
        }

        case OPCODES.Handshake:
            break;
    }
}

//@discord-rpc
// Handles the Ready event, Authorize response and Authenticate response.
async function InitialBridgeListener(message: MessageEvent): Promise<void> {

    if (!ALLOWED_ORIGINS.has(message.origin)) return;

    const opcode = message.data?.[0];

    //? Non frame opcode
    if (opcode != OPCODES.Frame) return;

    const payload: RpcFramePayload | undefined = message.data?.[1];

    const data = payload?.data;
    const event = payload?.evt;
    const command = payload?.cmd;

    switch (command) {

        case COMMANDS.Dispatch: {

            if (event != EVENTS.Ready) break;

            //\ Save ready data to send once Unity loads
            const data = SerializePayload(message.data);
            readyData = data;

            if (!disableInfoLogs) console.log("[Dissoniy BridgeLib]: Connected to RPC!");

            //# CONSOLE LOG OVERRIDE - - - - -
            const disableLogOverrideVariable = "[[[ DISABLE_CONSOLE_LOG_OVERRIDE ]]]§";
            const disableLogOverride = ParseBuildVariable(disableLogOverrideVariable, "boolean") as boolean | null;

            //\ Read variable
            if (disableLogOverride == null) {
                currentState = STATE_CODES.Errored;
                throw new Error(`[Dissonity BridgeLib]: DISABLE_CONSOLE_LOG_OVERRIDE has an invalid value (${disableLogOverride}). Accepted values are (1/True) and (0/False)`);
            }

            if (!disableLogOverride) {
                OverrideConsoleLogging();
            }

            //# PATCH URL MAPPINGS - - - - -
            const mappingsVariable = "[[[ MAPPINGS ]]]§"; // Mappings have a custom format, no JSON serialized here
            const patchUrlMappingsConfigVariable = "[[[ PATCH_URL_MAPPINGS_CONFIG ]]]§";

            const mappingsMap = ParseBuildVariable(mappingsVariable, "map") as Map<string, string>;
            const patchUrlMappingsConfigMap = ParseBuildVariable(patchUrlMappingsConfigVariable, "map") as Map<string, boolean>;

            const mappings = MapToMappingArray(mappingsMap);
            const patchUrlMappingsConfig = MapToConfig(patchUrlMappingsConfigMap);

            //? Patch url mappings
            if (mappings.length > 0) {

                if (!disableInfoLogs) console.log(`[Dissoniy BridgeLib]: Patching (${mappings.length}) url mappings...`);

                patchUrlMappings(mappings, patchUrlMappingsConfig);
            }

            //# AUTHORIZE - - - - -
            const oauthScopesVariable = "[[[ OAUTH_SCOPES ]]]§";
            const oauthScopes = ParseBuildVariable(oauthScopesVariable, "string[]") as string[];

            InternalSend(
                [
                    OPCODES.Frame,
                    {
                        cmd: COMMANDS.Authorize,
                        nonce: GetNonce(),
                        args: {
                            client_id: clientId,
                            scope: oauthScopes,
                            response_type: RESPONSE_TYPE,
                            prompt: PROMPT,
                            state: ""
                        }
                    }
                ]
            );

            break;
        }

        case COMMANDS.Authorize: {

            //? No authorization
            if (event == EVENTS.Error) {

                //\ Close RpcBridge
                currentState = STATE_CODES.Closed;
                RemoveListeners(InitialBridgeListener);

                InternalSend(
                    [
                        OPCODES.Close,
                        {
                            code: CLOSE_CODES.Normal,
                            message: "User unauthorized scopes",
                            nonce: GetNonce()
                        }
                    ]
                )

                break;
            }

            //\ Save authorize data to send once Unity loads
            const serializedData = SerializePayload(message.data);
            authorizeData = serializedData;

            if (!disableInfoLogs) console.log("[Dissoniy BridgeLib]: Authorized!");

            //# REQUEST TOKEN - - - - -
            const tokenRequestPathVariable = "[[[ TOKEN_REQUEST_PATH ]]]§";
            const tokenRequestPath = ParseBuildVariable(tokenRequestPathVariable, "string") as string;

            //# SERVER REQUEST - - - - -
            const serverRequestVariable = '[[[ SERVER_REQUEST ]]]§'; // Keep these single quotes ('), (") breaks the string when the JSON is loaded.
            const serverRequest = ParseBuildVariable(serverRequestVariable) as Record<string, unknown> | null;

            let body: Record<string, unknown> = {
                code: data.code
            };

            //? Add user server request
            if (serverRequest != null)
            {
                delete serverRequest.code;

                body = {
                    code: data.code,
                    ...serverRequest
                };
            }

            //\ Request
            const response = await fetch(`/.proxy${tokenRequestPath}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(body)
            });

            //\ Parse data
            const json = await response.json();

            //? No token
            if (!json.token) {
                throw new Error("[Dissonity]: The server JSON response didn't include a 'token' field");
            }

            //\ Save response data to send once Unity loads
            serverPayloadData = JSON.stringify(json);

            InternalSend(
                [
                    OPCODES.Frame,
                    {
                        cmd: COMMANDS.Authenticate,
                        nonce: GetNonce(),
                        args: {
                            access_token: json.token
                        }
                    }
                ]
            );

            break;
        }

        case COMMANDS.Authenticate: {

            RemoveListeners(InitialBridgeListener);

            //\ Save authenticate data to send once Unity loads
            const data = SerializePayload(message.data);
            authenticateData = data;

            currentState = STATE_CODES.Ready;

            if (!disableInfoLogs) console.log("[Dissoniy BridgeLib]: Authenticated!");

            //? Unity ready
            if (unityReady) {
                AddListeners(Bridge);
                DispatchMultiEvent();
            }

            // If Unity is not ready, the MultiEvent is dispatched via RequestState

            break;
        }
    }
}

//@discord-rpc
// Receive data from the client RPC
function Bridge(message: MessageEvent): void {

    //? Sent from the IframeBridge
    if (message.data.iframeBridge) return;

    if (!ALLOWED_ORIGINS.has(message.origin)) return;

    const data = SerializePayload(message.data);

    SendToUnity({ method: "_HandleMessage", payload: data })
}


//# UTILS - - - - -
// Used to simplify listening to RPC and IframeBridge
function AddListeners(...args: ((message: MessageEvent) => void)[]): void {

    for (const listener of args) {
        window.addEventListener("message", listener);
        listeners.push(listener);
    }
}

function RemoveListeners(...args: ((message: MessageEvent) => void)[]): void {

    for (const listener of args) {
        window.removeEventListener("message", listener);
        listeners.splice(listeners.indexOf(listener), 1);
    }
}

function SendToUnity(message: { method: DissonityBridgeMethods, payload: string }): void {

    const { method, payload } = message;

    _unityInstance?.SendMessage("_DissonityBridge", method, payload);
}

function ParseBuildVariable(variable: string, type: ParseVariableType = "string"): string | boolean | string[] | Map<string, string | boolean> | Record<string, unknown> | null {

    const raw = variable.split("]]] ")[1].slice(0, -1);

    if (type == "string") return raw;

    if (type == "boolean") {

        if (/1|true/i.test(variable)) return true;
        else if (/0|false/i.test(variable)) return false;
        throw new Error("[Dissonity BridgeLib]: Invalid boolean string");
    }

    if (type == "string[]") {
        const array = raw.split(",");
        if (array.length == 1 && array[0] == "") return [];
        return array;
    }

    if (type == "json") {
        try
        {
            return JSON.parse(raw);
        }
        catch
        {
            return null;
        }
    }

    const map = new Map<string, string | boolean>();

    const array = raw.split(",");

    //? Prevent empty data
    if (array.length < 2) return map;

    let lastValue: string | boolean = "";
    for (let i = 0; i < array.length; i++) {
        
        // Key
        if (i % 2 == 0) {

            lastValue = array[i];
            continue;
        }

        // Value
        else {

            const value = array[1];
            
            if (value == "True") map.set(lastValue, true);
            else if (value == "False") map.set(lastValue, false);
            else map.set(lastValue, value);

            continue;
        }
    }

    return map;
}

function MapToMappingArray(map: Map<string, string>): Mapping[] {

    const array: Mapping[] = [];

    for (const [key, value] of map) {

        array.push({
            prefix: key,
            target: value
        });
    }

    return array;
}

function MapToConfig(map: Map<string, boolean>): PatchUrlMappingsConfig {

    return {
        patchFetch: map.get("patchFetch"),
        patchWebSocket: map.get("patchWebSocket"),
        patchXhr: map.get("patchXhr"),
        patchSrcAttributes: map.get("patchSrcAttributes")
    }
}

// Serialize the RPC payload (then -> IframeBridge -> Unity)
function SerializePayload(messageData: unknown): string {

    const sendMessage = { data: messageData };

    const messageStr = JSON.stringify(sendMessage, (_, value) => {
    
        if (typeof value == "bigint") return value.toString();
        else return value;
    });

    return messageStr;
}

function GetNonce(): string {

    const uuid = new Array(36);

    for (let i = 0; i < 36; i++) {
        uuid[i] = Math.floor(Math.random() * 16);
    }

    uuid[14] = 4;
    uuid[19] = uuid[19] &= ~(1 << 2);
    uuid[19] = uuid[19] |= (1 << 3);
    uuid[8] = uuid[13] = uuid[18] = uuid[23] = "-";
    return uuid.map((x) => x.toString(16)).join("");
}

// Literal implementation of the overrideConsoleLogging from the official SDK
function OverrideConsoleLogging(): void {

    const consoleLevels = ["log", "warn", "debug", "info", "error"] as const;
    type ConsoleLevel = (typeof consoleLevels)[number];

    const captureLog = (level: ConsoleLevel, message: string) => {
        
        InternalSend(
            [OPCODES.Frame, {
                cmd: "CAPTURE_LOG",
                nonce: GetNonce(),
                args: {
                    level,
                    message
                }
            }]
        );
    };

    consoleLevels.forEach((level) => {

        const _consoleMethod = console[level];
        const _console = console;

        if (!_consoleMethod) {
            return;
        }

        console[level] = function () {
            const args = [].slice.call(arguments);
            const message = "" + args.join(" ");
            captureLog(level, message);
            _consoleMethod.apply(_console, args);
        }
    });
}

// Called from the RpcBridge (here) so it doesn't need to parse anything
function InternalSend(message: unknown): void {

    const source: Window = window.parent.opener ?? window.parent;
    const sourceOrigin = !!document.referrer ? document.referrer : "*";

    source.postMessage(message, sourceOrigin);
}

function DispatchMultiEvent(): void {

    const sendMessage = {
        nonce: null,
        payload: {
            raw_multi_event: {
                ready: readyData,
                authorize: authorizeData,
                authenticate: authenticateData,
                response: serverPayloadData
            }
        }
    };

    SendToUnity({ method: "_ReceiveMultiEvent", payload: JSON.stringify(sendMessage) });

    ClearData();
}

function ClearData(): void {
    clientId = "";
    readyData = "";
    authorizeData = "";
    serverPayloadData = "";
    authenticateData = "";
}

function ParseMajorMobileVersion(): number {

    if (mobileAppVersion && mobileAppVersion.includes(".")) {
        try {
            return parseInt(mobileAppVersion.split(".")[0]);
        } catch {
            return UNKNOWN_VERSION_NUMBER;
        }
    }
    return UNKNOWN_VERSION_NUMBER;
}


//# USED BY INDEX - - - - -
//@index
export function SetUnityInstance (instance: any): void {
    if (_unityInstance) return;

    _unityInstance = instance;
}


//# USED BY INTERFACE BRIDGE - - - - -
//@interface-bridge
export function InterfaceBridgeListener(message: RpcBridgeMessage): void {

    const { command, nonce, payload } = message; // payload is a stringified message
        
    switch (command as RpcBridgeCommands) {

        case "Send": {
            Send(payload!);
            break;
        }

        case "RequestQuery": {

            const query = RequestQuery();
            
            const formattedPayload = {
                nonce,
                payload: {
                    str: JSON.stringify(query)
                }
            };

            SendToUnity({ method: "_ReceiveString", payload: JSON.stringify(formattedPayload) });
            break;
        }

        case "RequestState": {
            RequestState(nonce!);
            break;
        }

        case "RequestPatchUrlMappings": {
            const parsedPayload = JSON.parse(payload!);
            RequestPatchUrlMappings(nonce!, parsedPayload.str);
            break;
        }

        case "RequestFormatPrice": {
            const parsedPayload = JSON.parse(payload!);
            RequestFormatPrice(nonce!, parsedPayload.str);
            break;
        }

        case "StopListening": {
            StopListening();
            break;
        }
    }
}

//@indirect:interface-bridge
// Send data to the client RPC
// UTF8 handling occurs in the IframeBridge
function Send(stringifiedMessage: string): void {

    const message = JSON.parse(stringifiedMessage)

    const source: Window = window.parent.opener ?? window.parent;
    const sourceOrigin = !!document.referrer ? document.referrer : "*";

    source.postMessage(message, sourceOrigin);
}

//@indirect:interface-bridge
function StopListening(): void {

    currentState = STATE_CODES.Closed;

    for (const listener of listeners) {
        window.removeEventListener("message", listener);
    }
}

//@indirect:interface-bridge
function RequestQuery(): Record<string, string> {

    function getQueryData() {

        const isParent = window.location.search.includes("?");

        const queryStringKeyValue = (isParent)
            ? window.location.search.replace("?", "").split("&")
            : window.parent.location.search.replace("?", "").split("&");

        const qsJsonObject: Record<string, string> = {};
        if (queryStringKeyValue.length != 0) {
            for (let i = 0; i < queryStringKeyValue.length; i++) {
                qsJsonObject[queryStringKeyValue[i].split("=")[0]] = queryStringKeyValue[i].split("=")[1];
            }
        }
        return qsJsonObject;
    }

    return getQueryData();
}

//@indirect:interface-bridge
function RequestState(nonce: string): void {

    const sendMessage = {
        nonce,
        payload: {
            code: currentState,
        }
    };

    SendToUnity({ method: "_ReceiveState",  payload: JSON.stringify(sendMessage) });

    if (!unityReady) {

        unityReady = true;

        if (currentState == STATE_CODES.Ready) {
            AddListeners(Bridge);
            DispatchMultiEvent();
        }
    }
}

//@indirect:interface-bridge
function RequestPatchUrlMappings(nonce: string, stringifiedMessage: string): void {

    const { mappings, config } = JSON.parse(stringifiedMessage);

    if ((mappings as any[]).length != 0) {
        patchUrlMappings(mappings, config);
    }

    const sendMessage = {
        nonce
    };

    SendToUnity({ method: "_ReceiveEmpty", payload: JSON.stringify(sendMessage) });
}

//@indirect:interface-bridge
function RequestFormatPrice(nonce: string, stringifiedMessage: string): void {

    const { amount, currency, locale } = JSON.parse(stringifiedMessage);

    const result = formatPrice({ amount, currency }, locale);

    const sendMessage = {
        nonce,
        payload: {
            str: result
        }
    };

    SendToUnity({ method: "_ReceiveString", payload: JSON.stringify(sendMessage) });
}

Handshake();