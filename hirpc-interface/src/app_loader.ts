/*

This is the Unity app loader. This script is executed to load a Unity build
using hiRPC for communication with Discord.

*/

import { AuthenticationPromise, BridgeMessage, GamePayload, HiRpcModule, RpcPayload } from "./types";


// Promises - - - - -
let resolveAuthentication = (_?: unknown) => {}
let authenticationPromise = new Promise((resolve) => {
    resolveAuthentication = resolve;
});


// File paths - - - - -
let baseUrl = `${window.location.protocol}//${window.location.host}${window.location.pathname}`;
if (!baseUrl.endsWith("/")) baseUrl += "/";

let outsideDiscord = false;
let proxySuffixAdded = false;
let needsProxySuffix = false;

let loaderPath = "Files/Build/client.loader.js"; 
let versionCheckPath = baseUrl + ".proxy/version.json";
let hiRpcPath = "Files/Bridge/dissonity_hirpc.js";


// Multi Event - - - - -
let readyData = "";
let authorizeData = "";
let serverPayloadData = "";
let authenticateData = "";


// hiRPC
const channel = "dissonity";
let hiRpcHash: Uint8Array | null = null;
let authenticated = false;
let unityReady = false;


// Authentication - - - - -
// Opcodes are handled at the JS level before authentication.
// Then they are handled at the C# level.
const OPCODES = {
    Handshake: 0,
    Frame: 1,
    Close: 2,
    Hello: 3
};

const COMMANDS = {
    Dispatch: "DISPATCH",
    Authorize: "AUTHORIZE",
    Authenticate: "AUTHENTICATE",
    CaptureLog: "CAPTURE_LOG"
};

const EVENTS = {
    Error: "ERROR",
    Ready: "READY"
};

const CLOSE_CODE_NORMAL = 1000;
const RESPONSE_TYPE = "code";
const PROMPT = "none";

// Resolution - - - - -
const MOBILE = "mobile";
let initialWidth = window.innerWidth;
let initialHeight = window.innerHeight;

// Utility logging functions
function styleLog(data: any) {
    console.log(`%c[DissonityHiRpcInterface]%c ${data}`, "color:#8177f6;font-weight: bold;", "color:initial;")
}

function styleError(data: any) {
    console.log(`%c[DissonityHiRpcInterface]%c ${data}`, "color:#8177f6;font-weight: bold;", "color:initial;")
}

// Set up paths before anything
async function initialize() {

    async function fileExists(url: string) {
        const response = await fetch(url, { method: "HEAD" });
        return response.ok;
    }

    async function updatePaths() {
        
        let { pathname } = window.location;
    
        // Handle URL override
        const pathSegments = pathname.split("/"); // "/.proxy/staging" -> ["", ".proxy", "staging"]
        pathSegments.shift();
    
        proxySuffixAdded = pathSegments[0] == ".proxy";
        needsProxySuffix = await fileExists(versionCheckPath);
        outsideDiscord = !proxySuffixAdded && !needsProxySuffix;
    
        // Add .proxy
        if (needsProxySuffix) {
            hiRpcPath = ".proxy/" + hiRpcPath;
            loaderPath = ".proxy/" + loaderPath;
    
            (globalThis as any).dso_needs_suffix = true;
        }

        else {
            (globalThis as any).dso_needs_suffix = false;
        }

        // Mark as outside Discord
        if (outsideDiscord) {
            (globalThis as any).dso_outside_discord = true;
        }
    
        hiRpcPath = baseUrl + hiRpcPath;
        loaderPath = baseUrl + loaderPath;
    }

    await updatePaths();
}

// Set up the hiRPC module
async function handleHiRpc() {

    async function loadHiRpc() {

        //? Module already created
        if (typeof window.dso_hirpc == "object") {

            initialize(window.dso_hirpc as HiRpcModule);
            return;
        }

        //\ Create module
        (globalThis as any).dso_hirpc = await new Promise(async (resolve, _) => {

            if ((globalThis as any).dso_needs_suffix) {
                const module = await import("dso_proxy_hirpc" as string);
                load(module);
            }

            else {
                const module = await import("dso_hirpc" as string);
                load(module);
            }

            async function load(module: any) {
                await module.default();

                const hiRpc = module as HiRpcModule;

                initialize(hiRpc);

                resolve(hiRpc);
            }
        });

        function initialize(hiRpc: HiRpcModule) {

            // Hash
            hiRpcHash = hiRpc.requestHash()!;
            hiRpc.lockHashes(hiRpcHash);

            // Add RPC listener
            hiRpc.addRpcListener(hiRpcHash, rpcListener);

            // Add app listener
            hiRpc.addAppListener(hiRpcHash, appListener);

            // Connect
            hiRpc.connect(hiRpcHash);
        }
    }

    // Only used for authentication - - - - -

    // Handles data sent from Unity
    function appListener(_: BridgeMessage) {

        // Unity just loaded, authentication already finished
        if (!unityReady && authenticated) {
            dispatchMultiEvent();
        }

        if (!unityReady) {
            unityReady = true;
            return;
        }
    }

    // Handles data sent from the Discord RPC
    async function rpcListener(data: [number, RpcPayload]) {

        const opcode = data?.[0];

        // This allows future opcode implementations before auth
        switch (opcode) {

            case OPCODES.Frame: {
                handleFrame(data);
            }
        }
    }

    async function handleFrame(data: [number, RpcPayload]) {

        const hiRpc = (globalThis as unknown as { dso_hirpc: HiRpcModule }).dso_hirpc;

        const payload = data?.[1];

        const rpcData = payload?.data;
        const event = payload?.evt;
        const command = payload?.cmd;
        
        switch (command) {

            case COMMANDS.Dispatch: {

                if (event != EVENTS.Ready) break;

                readyData = serializePayload(rpcData);

                if (!hiRpc.getDisableInfoLogs()) styleLog("Connected to RPC!");

                // Console log override - - - - -
                if (!hiRpc.getDisableConsoleLogOverride()) {
                    overrideConsoleLogging();
                }

                // Patch url mappings
                try {
                    const mappings = JSON.parse(hiRpc.getMappings()!);
                    const patchUrlMappingsConfig = JSON.parse(hiRpc.getPatchUrlMappingsConfig()!);

                    if (mappings.length > 0) {

                        if (!hiRpc.getDisableInfoLogs()) styleLog(`Patching url mappings... (${mappings.length})`);
                        hiRpc.patchUrlMappings(hiRpcHash!, mappings, patchUrlMappingsConfig);
                    }

                } catch (err) {
                    styleError(`Something went wrong patching the URL mappings: ${err}`);
                }

                // Authorize - - - - -
                hiRpc.sendToRpc(hiRpcHash!, OPCODES.Frame, {
                    cmd: COMMANDS.Authorize,
                    nonce: getNonce(),
                    args: {
                        client_id: hiRpc.getClientId()!,
                        scope: hiRpc.getOauthScopes()!,
                        response_type: RESPONSE_TYPE,
                        prompt: PROMPT,
                        state: ""
                    }
                });

                break;
            }

            case COMMANDS.Authorize: {

                //? No authorization
                if (event == EVENTS.Error) {

                    hiRpc.sendToRpc(hiRpcHash!, OPCODES.Close, {
                        code: CLOSE_CODE_NORMAL,
                        message: "User unauthorized scopes",
                        nonce: getNonce()
                    });

                    break;
                }

                authorizeData = serializePayload(rpcData);

                if (!hiRpc.getDisableInfoLogs()) styleLog("Authorized!");

                // Server request - - - - -
                let token;

                try {
                    const serverRequest = JSON.parse(hiRpc.getServerRequest()!);
                    const tokenRequestPath = hiRpc.getTokenRequestPath()!;

                    let body: Record<string, unknown> = {
                        code: rpcData.code
                    };

                    //? Add user server request
                    if (serverRequest != null)
                    {
                        delete serverRequest.code;
        
                        body = {
                            code: rpcData.code,
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
                    const jsonResponse = await response.json();

                    //? No token
                    if (!jsonResponse.token) {
                        throw new Error("The server JSON response didn't include a 'token' field");
                    }

                    serverPayloadData = JSON.stringify(jsonResponse);

                    token = jsonResponse.token;

                } catch (err) {

                    styleError(`Something went wrong in the server request: ${err}`);

                    hiRpc.clearRpcListeners(hiRpcHash!);

                    return;
                }

                // Authenticate - - - - -
                hiRpc.sendToRpc(hiRpcHash!, OPCODES.Frame, {
                    cmd: COMMANDS.Authenticate,
                    nonce: getNonce(),
                    args: {
                        access_token: token
                    }
                });

                break;
            }

            case COMMANDS.Authenticate: {
                
                authenticated = true;
                authenticateData = serializePayload(rpcData);

                if (!hiRpc.getDisableInfoLogs()) styleLog("Authenticated!");

                if (unityReady) {
                    dispatchMultiEvent();
                }
            }
        }
    }

    // Literal implementation of the overrideConsoleLogging from the official SDK
    function overrideConsoleLogging(): void {

        const consoleLevels = ["log", "warn", "debug", "info", "error"] as const;
        type ConsoleLevel = (typeof consoleLevels)[number];

        const captureLog = (level: ConsoleLevel, message: string) => {

            const hiRpc = (globalThis as unknown as { dso_hirpc: HiRpcModule }).dso_hirpc;

            hiRpc.sendToRpc(hiRpcHash!, OPCODES.Frame, {
                cmd: COMMANDS.CaptureLog,
                nonce: getNonce(),
                args: {
                    level,
                    message
                }
            });
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

    function getNonce(): string {

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

    function serializePayload(payload: unknown) {

        return JSON.stringify(payload, (_, value) => {
            if (typeof value == "bigint") return value.toString();
            else return value;
        });
    }

    // Complete the authentication process and open the upward communication (app -> JS)
    async function dispatchMultiEvent() {

        const hiRpc = (globalThis as unknown as { dso_hirpc: HiRpcModule }).dso_hirpc;

        hiRpc.sendToApp(hiRpcHash!, {
            channel,
            raw_multi_event: {
                ready: readyData,
                authorize: authorizeData,
                authenticate: authenticateData,
                response: serverPayloadData
            }
        });

        readyData = "";
        authorizeData = "";
        authenticateData = "";
        serverPayloadData = "";

        resolveAuthentication();
    }

    await loadHiRpc();
}

// Prepare the Unity build
async function handleUnityBuild() {

    // The hiRPC module should be ready when this function is called
    const hiRpc = (globalThis as unknown as { dso_hirpc: HiRpcModule }).dso_hirpc;
    const query = JSON.parse(hiRpc.getStringifiedQuery());

    const canvas: HTMLCanvasElement = document.querySelector("#unity-canvas")!;

    // Resolution
    const defaultWidth = Number('{{{ WIDTH }}}');
    const defaultHeight = Number('{{{ HEIGHT }}}');
    let width: number;
    let height: number;
    let autoSize = false;

    async function loadUnityBuild() {

        const loader = document.createElement("script");
        loader.setAttribute("src", loaderPath);
        loader.setAttribute("type", "text/javascript");

        const loaderPromise = new Promise(resolve => {
            loader.addEventListener("load", resolve);
        });

        document.head.appendChild(loader);
        
        await loaderPromise;
    }

    function handleResolution() {

        // Constants
        const RESOLUTION_TYPE = {
            Default: 1,
            Viewport: 2,
            Dynamic: 3,
            Max: 4
        };

        // Resolution
        let viewportWidth: string | number = "[[[ WIDTH ]]]§";
        let viewportHeight: string | number = "[[[ HEIGHT ]]]§";

        let desktopResolution: string | number = "[[[ DESKTOP_RESOLUTION ]]]§";
        let mobileResolution: string | number = "[[[ MOBILE_RESOLUTION ]]]§";
        let webResolution: string | number = "[[[ WEB_RESOLUTION ]]]§";

        let resolution;

        // Parse
        viewportWidth = Number( parseVariable(viewportWidth) );
        viewportHeight = Number( parseVariable(viewportHeight) );
        desktopResolution = Number( parseVariable(desktopResolution) );
        mobileResolution = Number( parseVariable(mobileResolution) );
        webResolution = Number( parseVariable(webResolution) );

        // Web
        if (outsideDiscord) resolution = webResolution;

        // Mobile
        if (query.platform == MOBILE || /iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {

            if (!resolution) resolution = mobileResolution;

            // Mobile device style: fill the whole browser client area with the game canvas:
            var meta = document.createElement('meta');
            meta.name = 'viewport';
            meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
            document.getElementsByTagName('head')[0].appendChild(meta);

            canvas.style.width = "100%";
            canvas.style.height = "100%";
            canvas.style.position = "fixed";

            document.body.style.textAlign = "left";
        }

        else if (!resolution){
            resolution = desktopResolution;
        }

        switch (resolution) {
            case RESOLUTION_TYPE.Default: {
                width = defaultWidth;
                height = defaultHeight;
                break;
            }
            case RESOLUTION_TYPE.Viewport: {
                width = viewportWidth;
                height = viewportHeight;
                break;
            }
            case RESOLUTION_TYPE.Dynamic: {
                autoSize = true;
                break;
            }
            case RESOLUTION_TYPE.Max: {
                width = initialWidth;
                height = initialHeight;
                setRatio();
            }
        }

        if (!autoSize) {

            setRatio();

            window.addEventListener("resize", () => {
                setRatio();
            });
        }
    }

    function parseVariable(variable: string) {
        const raw = variable.split("]]] ")[1].slice(0, -1);
        return raw;
    }

    function setRatio()
    {
        canvas.width = width;
        canvas.height = height;
        const aspectRatio = canvas.width / canvas.height;

        let newWidth = window.innerWidth;
        let newHeight = window.innerHeight;

        if (newWidth / newHeight > aspectRatio) {
            newWidth = newHeight * aspectRatio;
        } else {
            newHeight = newWidth / aspectRatio;
        }

        canvas.style.width = `${newWidth}px`;
        canvas.style.height = `${newHeight}px`;
    }

    let background = "{{{ BACKGROUND_FILENAME ? 'Files/Build/' + BACKGROUND_FILENAME.replace(/'/g, '%27') + '\') center / cover' : '#000000' }}}";
    if (background != "#000000") {
        background = needsProxySuffix
        ? "url('.proxy/" + background
        : "url('" + background;
    }
    canvas.style.background = background;

    // Load Unity and handle resolution - - - - -
    await loadUnityBuild();

    handleResolution();

    const USE_THREADS = /true|1/i.test("{{{ USE_THREADS }}}");
    const USE_WASM = /true|1/i.test("{{{ USE_WASM }}}");
    const MEMORY_FILENAME = /true|1/i.test("{{{ MEMORY_FILENAME }}}");
    const SYMBOLS_FILENAME = /true|1/i.test("{{{ SYMBOLS_FILENAME }}}");

    // Configuration - - - - -
    const dataUrl = needsProxySuffix ? ".proxy/Files/Build/{{{ DATA_FILENAME }}}" : "Files/Build/{{{ DATA_FILENAME }}}";
    const frameworkUrl = needsProxySuffix ? ".proxy/Files/Build/{{{ FRAMEWORK_FILENAME }}}" : "Files/Build/{{{ FRAMEWORK_FILENAME }}}";
    let workerUrl: string | undefined = needsProxySuffix ? ".proxy/Files/Build/{{{ WORKER_FILENAME }}}" : "Files/Build/{{{ WORKER_FILENAME }}}";
    let codeUrl: string | undefined = needsProxySuffix ? ".proxy/Files/Build/{{{ CODE_FILENAME }}}" : "Files/Build/{{{ CODE_FILENAME }}}";
    let memoryUrl: string | undefined = needsProxySuffix ? ".proxy/Files/Build/{{{ MEMORY_FILENAME }}}" : "Files/Build/{{{ MEMORY_FILENAME }}}";
    let symbolsUrl: string | undefined = needsProxySuffix ? ".proxy/Files/Build/{{{ SYMBOLS_FILENAME }}}" : "Files/Build/{{{ SYMBOLS_FILENAME }}}";

    if (!USE_THREADS) workerUrl = undefined;
    if (!USE_WASM) codeUrl = undefined;
    if (!MEMORY_FILENAME) memoryUrl = undefined;
    if (!SYMBOLS_FILENAME) symbolsUrl = undefined;

    const companyName = '{{{ JSON.stringify(COMPANY_NAME) }}}'.replace('"', "");
    const productName = '{{{ JSON.stringify(PRODUCT_NAME) }}}'.replace('"', "");
    const productVersion = '{{{ JSON.stringify(PRODUCT_VERSION) }}}'.replace('"', "");

    createUnityInstance(canvas, {
        dataUrl,
        frameworkUrl,
        streamingAssetsUrl: "StreamingAssets",
        workerUrl,
        codeUrl,
        memoryUrl,
        symbolsUrl,
        companyName,
        productName,
        productVersion,
        matchWebGLToCanvasSize: autoSize,
        // devicePixelRatio: 1, // Uncomment this to override low DPI rendering on high DPI displays.
    });
}

(async () => {

    await initialize();

    await handleHiRpc();

    await handleUnityBuild();
})();

export default authenticationPromise;