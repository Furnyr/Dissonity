
/*

This is the "RpcBridge". It handles the communication with the Discord RPC
but it can't send messages to the Unity build.

Instead, it passes the necessary information to the "IframeBridge".

It handles the handshake, authorization and authentication while the Unity build loads.

The "user-defined" variables are replaced by post-processing the Unity build.

Function names are PascalCase, C# method conventions.

*/

// Official methods
declare const patchUrlMappings: (mappings: Mapping[], config: PatchUrlMappingsConfig) => void;

// Official types
import type { Mapping, PatchUrlMappingsConfig } from "./official/official_types";

// Types
import type { RpcBridgeCommands, DissonityBridgeMethods, ParseVariableType } from "./types";

//# CONSTANTS - - - - -
// Handshake
const HANDSHAKE_VERSION = 1;
const HANDSHAKE_ENCODING = "json";

// Authorize
const RESPONSE_TYPE = "code";
const PROMPT = "none";

// Error
const ERROR = "ERROR";

const OPCODES = {
    Handshake: 0,
    Frame: 1,
    Close: 2,
    Hello: 3
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
    Authorize: "AUTHORIZE",
    Authenticate: "AUTHENTICATE"
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
let childIframe : HTMLIFrameElement | null | undefined = undefined;
let disableInfoLogs: string | boolean = "[[[ DISABLE_INFO_LOGS ]]]§";
let unityReady = false;

let clientId = "";

let readyData = "";
let authorizeData = "";
let serverPayloadData = "";
let authenticateData = "";

let listeners: ((message: MessageEvent) => void)[] = [];


//# FUNCTIONS - - - - -
function SendToIframeBridge(message: { method: DissonityBridgeMethods, payload: string }): void {

    function send() {
        const sourceOrigin = window.location.origin;
        childIframe!.contentWindow?.postMessage(message, sourceOrigin);
    }

    if (childIframe) {
        send();
        return;
    }

    // Child iframe is not cached here
    if (currentState == STATE_CODES.Errored) return;

    childIframe = document.getElementById("dissonity-child") as HTMLIFrameElement | null;
  
    if (childIframe == null) {
        currentState = STATE_CODES.Errored;
        throw new Error("[Dissonity BridgeLib]: Child iframe not found");
    }

    send();
}

function Handshake(): void {

    /*
    
    Process will be:
    - Listen(ready, iframeBridge, nonFrameOpcode);
    - RemoveListen(ready); Listen(authorize);
    - RemoveListen(authorize); Request();
    - Listen(authenticate);
    - RemoveListen(authenticate); Listen(normalBridge)

    Then the connection is established, authorized and authenticated.
    
    */

    disableInfoLogs = ParseUserVariable(disableInfoLogs as string, "boolean") as boolean;

    AddListeners(BridgeReadyListener, IframeBridgeListener, NonFrameOpcode);

    const query = RequestQuery();

    //? No frame_id param
    if (!query || !query.frame_id) {
        currentState = STATE_CODES.OutsideDiscord;
        return;
    }

    const clientIdVariable = "[[[ CLIENT_ID ]]]§";
    
    clientId = ParseUserVariable(clientIdVariable) as string;

    InternalSend(
        [
            OPCODES.Handshake,
            {
                v: HANDSHAKE_VERSION,
                encoding: HANDSHAKE_ENCODING,
                client_id: clientId,
                frame_id: query.frame_id
            }
        ]
    );
}


//# LISTENERS - - - - -
//@iframe-bridge
function IframeBridgeListener(message: MessageEvent): void {

    if (!ALLOWED_ORIGINS.has(message.origin)) return;
    
    //? Send from the IframeBridge
    if (!message.data.iframeBridge) return;

    const { command, payload } = message.data;

    //? Not ready
    if (currentState != STATE_CODES.Ready) {

        switch(command as RpcBridgeCommands) {

            case "RequestQuery": {
                const query = RequestQuery();
                SendToIframeBridge({ method: "ReceiveQuery", payload: JSON.stringify(query) });
                break;
            }
    
            case "RequestState": {
                RequestState();
                break;
            }

            case "RequestPatchUrlMappings": {
                RequestPatchUrlMappings(payload);
                break;
            }

            default: {
                throw new Error("[Dissonity BridgeLib]: Invalid command while the RPC connection isn't established");
            }
        }

        return;
    }
        
    //\ Switch all commands
    switch(command as RpcBridgeCommands) {

        case "Send": {
            Send(payload);
            break;
        }

        case "RequestQuery": {
            const query = RequestQuery();
            SendToIframeBridge({ method: "ReceiveQuery", payload: JSON.stringify(query) });
            break;
        }

        case "RequestState": {
            RequestState();
            break;
        }

        case "RequestPatchUrlMappings": {
            RequestPatchUrlMappings(payload);
            break;
        }

        case "StopListening": {
            StopListening();
            break;
        }
    }
}

//@discord-rpc
function NonFrameOpcode(message: MessageEvent): void {

    if (!ALLOWED_ORIGINS.has(message.origin)) return;

    //? Sent from the IFrameBridge
    if (message.data.iframeBridge) return;

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

            currentState = STATE_CODES.Closed;
    
            StopListening();
    
            //? Forward message to Unity
            if (unityReady) {
                SendToIframeBridge({ method: "HandleMessage", payload: SerializePayload(message.data) })
            }
            break;
        }

        case OPCODES.Handshake:
            break;
    }
}

//@discord-rpc
// Receive the READY event from the RPC
function BridgeReadyListener(message: MessageEvent): void {

    if (!ALLOWED_ORIGINS.has(message.origin)) return;

    //? Sent from the IFrameBridge
    if (message.data.iframeBridge) return;

    const opcode = message.data?.[0];

    //? Non frame opcode
    if (opcode != OPCODES.Frame) return;

    RemoveListeners(BridgeReadyListener);

    const payload = message.data?.[1]?.data;

    //? No ready data
    if (!payload || !payload.v) {

        currentState = STATE_CODES.Errored;

        //? Error event
        if (payload.evt == ERROR) {
            
            //? Unity ready
            if (unityReady) {
                SendToIframeBridge({ method: "HandleMessage", payload: SerializePayload(message.data) });
                return;
            }

            throw new Error(`[Dissonity BridgeLib]: Error received with code ${payload.data.code}: ${payload.data.message}`);
        }

        throw new Error("[Dissonity BridgeLib]: Invalid message received");
    }

    //\ Save ready data to send once Unity loads
    const data = SerializePayload(message.data);
    readyData = data;

    if (!disableInfoLogs) console.log("[Dissoniy BridgeLib]: Connected to the RPC!");

    //# CONSOLE LOG OVERRIDE - - - - -
    const disableLogOverrideVariable = "[[[ DISABLE_CONSOLE_LOG_OVERRIDE ]]]§";
    const disableLogOverride = ParseUserVariable(disableLogOverrideVariable, "boolean") as boolean | null;

    //\ Read pre-processor variable
    if (disableLogOverride == null) {
        currentState = STATE_CODES.Errored;
        throw new Error(`[Dissonity BridgeLib]: DISABLE_CONSOLE_LOG_OVERRIDE has an invalid value (${disableLogOverride}). Accepted values are (1/True) and (0/False)`);
    }

    if (!disableLogOverride) {
        OverrideConsoleLogging();
    }

    //# AUTHORIZE - - - - -
    const oauthScopesVariable = "[[[ OAUTH_SCOPES ]]]§";
    const oauthScopes = ParseUserVariable(oauthScopesVariable, "string[]") as string[];

    //\ Switch listener
    AddListeners(BridgeAuthorizeListener);

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
}

//@discord-rpc
// Receive the AUTHORIZE response from the RPC
async function BridgeAuthorizeListener(message: MessageEvent): Promise<void> {

    if (!ALLOWED_ORIGINS.has(message.origin)) return;

    //? Sent from the IFrameBridge
    if (message.data.iframeBridge) return;

    const opcode = message.data?.[0];

    //? Non frame opcode
    if (opcode != OPCODES.Frame) return;

    RemoveListeners(BridgeAuthorizeListener);

    const payload = message.data?.[1]?.data;

    //? No authorize data
    if (!payload || !payload.code) {

        currentState = STATE_CODES.Errored;

        //? Error event
        if (payload.evt == ERROR) {
            
            //? Unity ready
            if (unityReady) {
                SendToIframeBridge({ method: "HandleMessage", payload: SerializePayload(message.data) });
                return;
            }

            throw new Error(`[Dissonity BridgeLib]: Error received with code ${payload.data.code}: ${payload.data.message}`);
        }

        throw new Error("[Dissonity BridgeLib]: Invalid message received");
    }

    //\ Save authorize data to send once Unity loads
    const data = SerializePayload(message.data);
    authorizeData = data;

    if (!disableInfoLogs) console.log("[Dissoniy BridgeLib]: Authorized!");

    //\ Read user variables (patch url mappings)
    const mappingsVariable = "[[[ MAPPINGS ]]]§"; // Mappings have a custom format, no JSON serialized here
    const patchUrlMappingsConfigVariable = "[[[ PATCH_URL_MAPPINGS_CONFIG ]]]§";

    const mappingsMap = ParseUserVariable(mappingsVariable, "map") as Map<string, string>;
    const patchUrlMappingsConfigMap = ParseUserVariable(patchUrlMappingsConfigVariable, "map") as Map<string, boolean>;

    const mappings = MapToMappingArray(mappingsMap);
    const patchUrlMappingsConfig = MapToConfig(patchUrlMappingsConfigMap);

    //? Patch url mappings
    if (mappings.length > 0) {

        if (!disableInfoLogs) console.log(`[Dissoniy BridgeLib]: Patching (${mappings.length}) url mappings...`);

        patchUrlMappings(mappings, patchUrlMappingsConfig);
    }

    //# REQUEST TOKEN - - - - -
    const tokenRequestPathVariable = "[[[ TOKEN_REQUEST_PATH ]]]§";
    const tokenRequestPath = ParseUserVariable(tokenRequestPathVariable, "string") as string;

    //# SERVER REQUEST - - - - -
    const serverRequestVariable = '[[[ SERVER_REQUEST ]]]§'; // Keep these single quotes ('), (") breaks the string when the JSON is loaded.
    const serverRequest = ParseUserVariable(serverRequestVariable) as Record<string, unknown> | null;

    let body: Record<string, unknown> = {
        code: payload.code
    };

    //? Add user server request
    if (serverRequest != null)
    {
        delete serverRequest.code;

        body = {
            code: payload.code,
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

    //# AUTHENTICATE - - - - -

    //\ Switch listener
    AddListeners(BridgeAuthenticateListener);

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
}

//@discord-rpc
// Receive the AUTHENTICATE response from the RPC
function BridgeAuthenticateListener(message: MessageEvent): void {

    if (!ALLOWED_ORIGINS.has(message.origin)) return;

    //? Sent from the IFrameBridge
    if (message.data.iframeBridge) return;

    const opcode = message.data?.[0];

    //? Non frame opcode
    if (opcode != OPCODES.Frame) return;

    RemoveListeners(BridgeAuthenticateListener);

    const payload = message.data?.[1]?.data;

    //? No authenticate data
    if (!payload || !payload.access_token) {

        currentState = STATE_CODES.Errored;

        //? Error event
        if (payload.evt == ERROR) {
            
            //? Unity ready
            if (unityReady) {
                SendToIframeBridge({ method: "HandleMessage", payload: SerializePayload(message.data) });
                return;
            }

            throw new Error(`[Dissonity BridgeLib]: Error received with code ${payload.data.code}: ${payload.data.message}`);
        }

        throw new Error("[Dissonity BridgeLib]: Invalid message received");
    }

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
}

//@discord-rpc
// Receive data from the client RPC
function Bridge(message: MessageEvent): void {

    //? Sent from the IframeBridge
    if (message.data.iframeBridge) return;

    if (!ALLOWED_ORIGINS.has(message.origin)) return;

    const data = SerializePayload(message.data);

    SendToIframeBridge({ method: "HandleMessage", payload: data })
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

function ParseUserVariable(variable: string, type: ParseVariableType = "string"): string | boolean | string[] | Map<string, string | boolean> | Record<string, unknown> | null {

    const raw = variable.split("]]] ")[1].slice(0, -1);

    if (type == "string") return raw;

    if (type == "boolean") {

        if (/1|true/i.test(variable)) return true;
        else if (/0|false/i.test(variable)) return false;
        throw new Error("[Dissonity BridgeLib]: Invalid boolean string");
    }

    if (type == "string[]") {
        return raw.split(",");
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

    const multiEventPayload = {
        ready: readyData,
        authorize: authorizeData,
        authenticate: authenticateData,
        response: serverPayloadData
    };

    SendToIframeBridge({ method: "ReceiveMultiEvent", payload: JSON.stringify(multiEventPayload) });

    ClearData();
}

function ClearData(): void {
    clientId = "";
    readyData = "";
    authorizeData = "";
    serverPayloadData = "";
    authenticateData = "";
}


//# USED BY IFRAME BRIDGE - - - - -
//@indirect:iframe-bridge
// Send data to the client RPC
// UTF8 handling occurs in the IframeBridge
function Send(stringifiedMessage: string): void {

    const message = JSON.parse(stringifiedMessage)

    const source: Window = window.parent.opener ?? window.parent;
    const sourceOrigin = !!document.referrer ? document.referrer : "*";

    source.postMessage(message, sourceOrigin);
}

//@indirect:iframe-bridge
function StopListening(): void {
    for (const listener of listeners) {
        window.removeEventListener("message", listener);
    }
}

//@indirect:iframe-bridge
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

//@indirect:iframe-bridge
function RequestState(): void {

    const sendMessage = {
        code: currentState,
    };

    SendToIframeBridge({ method: "ReceiveState", payload: JSON.stringify(sendMessage) });

    if (!unityReady) {

        unityReady = true;

        if (currentState == STATE_CODES.Ready) {
            AddListeners(Bridge);
            DispatchMultiEvent();
        }
    }
}

//@indirect:iframe-bridge
function RequestPatchUrlMappings(stringifiedMessage: string): void {

    const { mappings, config } = JSON.parse(stringifiedMessage);

    if ((mappings as any[]).length != 0) {
        patchUrlMappings(mappings, config);
    }

    SendToIframeBridge({ method: "ReceivePatchUrlMappings", payload: "" });
}


document.addEventListener("DOMContentLoaded", Handshake);