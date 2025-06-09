/*

This is the Unity app loader. This script is executed to load a Unity build
using hiRPC for communication with Discord.

*/

import type { HiRpcModule } from "./types";


// File paths - - - - -
function getPath() {

    const pathname = window.location.pathname;

    const isFile = /\.[^\/]+$/.test(pathname);

    if (isFile) {
        return pathname.substring(0, pathname.lastIndexOf("/") + 1);
    }

    return pathname;
}

function logButRemoveLater(...obj: any[]) {
    console.log(...obj);
}

let baseUrl = `${window.location.protocol}//${window.location.host}${getPath()}`;
if (!baseUrl.endsWith("/")) baseUrl += "/";

let outsideDiscord = false;
let needsProxyPrefix = false;

let loaderPath = "Build/{{{ LOADER_FILENAME }}}"; 

const proxyBridgeImport = "dso_proxy_bridge/";
const normalBridgeImport = "dso_bridge/";
const hirpcFileName = "dissonity_hirpc.js";
const buildVariablesFileName = "dissonity_build_variables.js";

// Resolution - - - - -
const MOBILE = "mobile";
let initialWidth = window.innerWidth;
let initialHeight = window.innerHeight;

// Set up paths before anything
async function initialize() {

    async function updatePaths() {
        
        //? Not outside Discord
        if (window.location.hostname.endsWith(".discordsays.com")) {
            sessionStorage.setItem("dso_outside_discord", "false" as NonNullable<SessionStorage["dso_outside_discord"]>);
        }

        else {
            outsideDiscord = true;
            sessionStorage.setItem("dso_outside_discord", "true" as NonNullable<SessionStorage["dso_outside_discord"]>);
        }

        //? Doesn't need prefix
        if (outsideDiscord || window.location.pathname.startsWith("/.proxy")) {
            sessionStorage.setItem("dso_needs_prefix", "false" as NonNullable<SessionStorage["dso_needs_prefix"]>);
        }

        else {
            needsProxyPrefix = true;
            sessionStorage.setItem("dso_needs_prefix", "true" as NonNullable<SessionStorage["dso_needs_prefix"]>);
        }
    
        // Add .proxy
        if (needsProxyPrefix) {
            loaderPath = ".proxy/" + loaderPath;
        }

        // Mark as outside Discord
        if (outsideDiscord) {
            window.sessionStorage.setItem("dso_outside_discord", "true" as NonNullable<SessionStorage["dso_outside_discord"]>);
        }

        else {
            window.sessionStorage.setItem("dso_outside_discord", "false" as NonNullable<SessionStorage["dso_outside_discord"]>);
        }
    
        loaderPath = baseUrl + loaderPath;
    }

    await updatePaths();
}

// Set up the hiRPC module
async function handleHiRpc() {

    //? Module already created

    //todo: remove, only for testing purposes
    function checkDepth() {

        let depth = 0;
        let data: { canAccess: boolean, error: unknown }[] = [];
        let doContinue = true;
        let lastWindow: Window | null = window;
    
        while (doContinue) {
            try {
                let _ = lastWindow.dso_hirpc;
                
                data.push({
                    canAccess: true,
                    error: null
                });

            } catch (error) {

                data.push({
                    canAccess: false,
                    error
                });
            }
            if (lastWindow == lastWindow.parent) break;
            lastWindow = lastWindow.parent;
            depth++;
        }

        return [depth, data];
    }

    const [depth, data] = checkDepth();
    console.log(`[Dissonity Debug]: App Loader Depth: ${depth}`);
    console.log(`[Dissonity Debug]: Obtained depth data - - - - -`);
    console.log(data);
    console.log(`[Dissonity Debug]: End depth data - - - - -`);

    // Nested
    const isNested = window.parent != window.parent.parent;
    if (isNested && typeof window.parent?.dso_hirpc == "object") {

        //\ Add shallow references to this window to use later
        Object.defineProperty(window, "dso_hirpc", {
            value: window.parent.dso_hirpc,
            writable: false,
            configurable: false
        });

        Object.freeze(window.dso_hirpc);

        window.dso_build_variables = window.parent.dso_build_variables;
        window.Dissonity = window.parent.Dissonity;

        initialize(window.parent.dso_hirpc as HiRpcModule, true);
        return;
    }

    // Not nested
    if (typeof window.dso_hirpc == "object") {

        initialize(window.dso_hirpc as HiRpcModule, true);
        return;
    }

    //\ Create module
    // The instance will be available in window.dso_hirpc after this promise resolution
    await new Promise(async (resolve, _) => {

        if (needsProxyPrefix) {
            await import(`${proxyBridgeImport}${hirpcFileName}`);
            await import(`${proxyBridgeImport}${buildVariablesFileName}`);
            
            load();
        }

        else {
            logButRemoveLater("[Dissonity Debug]: Importing hiRPC module");
            await import(`${normalBridgeImport}${hirpcFileName}`);
            await import(`${normalBridgeImport}${buildVariablesFileName}`);
            
            load();
        }

        async function load() {

            // window.dso_hirpc is defined after this line
            const hiRpc = new window.Dissonity.HiRpc.default() as HiRpcModule;

            await initialize(hiRpc, false || hiRpc.getBuildVariables().LAZY_HIRPC_LOAD);

            logButRemoveLater("[Dissonity Debug]: hiRPC promise resolution");

            resolve(hiRpc);
        }
    });

    async function initialize(hiRpc: HiRpcModule, loaded: boolean) {

        hiRpc.lockHashAccess();

        if (!loaded) {
            await hiRpc.load(0);
        }
    }
}

// Prepare the Unity build
async function handleUnityBuild() {

    // The hiRPC module should be ready when this function is called
    const hiRpc = window.dso_hirpc as HiRpcModule;
    const query = hiRpc.getQueryObject();

    const canvas: HTMLCanvasElement = document.querySelector("#unity-canvas")!;

    logButRemoveLater("[Dissonity Debug]: Unity canvas is:");
    logButRemoveLater(canvas);

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
        let browserResolution: string | number = "[[[ BROWSER_RESOLUTION ]]]§";

        let resolution;

        // Parse
        viewportWidth = Number( parseVariable(viewportWidth) );
        viewportHeight = Number( parseVariable(viewportHeight) );
        desktopResolution = Number( parseVariable(desktopResolution) );
        mobileResolution = Number( parseVariable(mobileResolution) );
        browserResolution = Number( parseVariable(browserResolution) );

        // Browser
        if (outsideDiscord) resolution = browserResolution;

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

        else if (!resolution) {
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

                window.dso_expand_canvas = () => {
                    width = window.innerWidth;
                    height = window.innerHeight;
                    setRatio();
                }
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

    let background = "{{{ BACKGROUND_FILENAME ? 'Build/' + BACKGROUND_FILENAME.replace(/'/g, '%27') + '\') center / cover' : '#000000' }}}";
    if (background != "#000000") {
        background = needsProxyPrefix
        ? "url('.proxy/" + background
        : "url('" + background;
    }
    canvas.style.background = background;

    // Load Unity and handle resolution - - - - -
    logButRemoveLater("[Dissonity Debug]: Trying to laod Unity build now...");
    await loadUnityBuild();
    logButRemoveLater("[Dissonity Debug]: Unity build is loaded, but not instantiated yet.");

    handleResolution();

    const USE_THREADS = /true|1/i.test("{{{ USE_THREADS }}}");
    const USE_WASM = /true|1/i.test("{{{ USE_WASM }}}");
    const MEMORY_FILENAME = /true|1/i.test("{{{ MEMORY_FILENAME }}}");
    const SYMBOLS_FILENAME = /true|1/i.test("{{{ SYMBOLS_FILENAME }}}");

    // Configuration - - - - -
    const dataUrl = needsProxyPrefix ? `${baseUrl}.proxy/Build/{{{ DATA_FILENAME }}}` : `${baseUrl}Build/{{{ DATA_FILENAME }}}`;
    const frameworkUrl = needsProxyPrefix ? `${baseUrl}.proxy/Build/{{{ FRAMEWORK_FILENAME }}}` : `${baseUrl}Build/{{{ FRAMEWORK_FILENAME }}}`;
    let workerUrl: string | undefined = needsProxyPrefix ? `${baseUrl}.proxy/Build/{{{ WORKER_FILENAME }}}` : `${baseUrl}Build/{{{ WORKER_FILENAME }}}`;
    let codeUrl: string | undefined = needsProxyPrefix ? `${baseUrl}.proxy/Build/{{{ CODE_FILENAME }}}` : `${baseUrl}Build/{{{ CODE_FILENAME }}}`;
    let memoryUrl: string | undefined = needsProxyPrefix ? `${baseUrl}.proxy/Build/{{{ MEMORY_FILENAME }}}` : `${baseUrl}Build/{{{ MEMORY_FILENAME }}}`;
    let symbolsUrl: string | undefined = needsProxyPrefix ? `${baseUrl}.proxy/Build/{{{ SYMBOLS_FILENAME }}}` : `${baseUrl}Build/{{{ SYMBOLS_FILENAME }}}`;

    if (!USE_THREADS) workerUrl = undefined;
    if (!USE_WASM) codeUrl = undefined;
    if (!MEMORY_FILENAME) memoryUrl = undefined;
    if (!SYMBOLS_FILENAME) symbolsUrl = undefined;

    const companyName = '{{{ JSON.stringify(COMPANY_NAME) }}}'.replace('"', "");
    const productName = '{{{ JSON.stringify(PRODUCT_NAME) }}}'.replace('"', "");
    const productVersion = '{{{ JSON.stringify(PRODUCT_VERSION) }}}'.replace('"', "");

    logButRemoveLater("[Dissonity Debug]: Creating Unity instance...");

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
    }).then(() => {
        logButRemoveLater("[Dissonity Debug]: Unity instance created.");
    });
}

(async () => {

    logButRemoveLater("[Dissonity Debug]: First initialization");

    await initialize();

    logButRemoveLater("[Dissonity Debug]: hiRPC initialization");

    await handleHiRpc();

    logButRemoveLater("[Dissonity Debug]: Unity initialization");

    await handleUnityBuild();
})();