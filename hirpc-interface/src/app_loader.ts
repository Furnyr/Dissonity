/*

This is the Unity app loader. This script is executed to load a Unity build
using hiRPC for communication with Discord.

*/

import type { HiRpcModule } from "./types";


// File paths - - - - -
let baseUrl = `${window.location.protocol}//${window.location.host}${window.location.pathname}`;
if (!baseUrl.endsWith("/")) baseUrl += "/";

let outsideDiscord = false;
let proxyPrefixAdded = false;
let needsProxyPrefix = false;

let loaderPath = "Build/client.loader.js"; 
const versionCheckPath = baseUrl + ".proxy/version.json";

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

    async function fileExists(url: string) {
        const response = await fetch(url, { method: "HEAD" });
        return response.ok;
    }

    async function updatePaths() {
        
        let { pathname } = window.location;
    
        // Handle URL override
        const pathSegments = pathname.split("/"); // "/.proxy/staging" -> ["", ".proxy", "staging"]
        pathSegments.shift();
    
        proxyPrefixAdded = pathSegments[0] == ".proxy";
        needsProxyPrefix = await fileExists(versionCheckPath);
        outsideDiscord = !proxyPrefixAdded && !needsProxyPrefix;
    
        // Add .proxy
        if (needsProxyPrefix) {
            loaderPath = ".proxy/" + loaderPath;

            window.sessionStorage.setItem("dso_needs_prefix", "true" as NonNullable<SessionStorage["dso_needs_prefix"]>);
        }

        else {
            window.sessionStorage.setItem("dso_needs_prefix", "false" as NonNullable<SessionStorage["dso_needs_prefix"]>);
        }

        // Mark as outside Discord
        if (outsideDiscord) {
            window.sessionStorage.setItem("dso_outside_discord", "true" as NonNullable<SessionStorage["dso_outside_discord"]>);
        }
    
        loaderPath = baseUrl + loaderPath;
    }

    await updatePaths();
}

// Set up the hiRPC module
async function handleHiRpc() {

    //? Module already created
    if (typeof window.dso_hirpc == "object") {

        initialize(window.dso_hirpc as HiRpcModule, true);
        return;
    }

    //\ Create module
    window.dso_hirpc = await new Promise(async (resolve, _) => {

        if (outsideDiscord) {

            await import(`${normalBridgeImport}${hirpcFileName}`);
            await import(`${normalBridgeImport}${buildVariablesFileName}`);
            load();
        }

        else {
            
            await import(`${proxyBridgeImport}${hirpcFileName}`);
            await import(`${proxyBridgeImport}${buildVariablesFileName}`);
            load();
        }

        async function load() {

            const hiRpc = new window.Dissonity.HiRpc.default() as HiRpcModule;

            await initialize(hiRpc, false);

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

    let background = "{{{ BACKGROUND_FILENAME ? 'Build/' + BACKGROUND_FILENAME.replace(/'/g, '%27') + '\') center / cover' : '#000000' }}}";
    if (background != "#000000") {
        background = needsProxyPrefix
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
    const dataUrl = needsProxyPrefix ? ".proxy/Build/{{{ DATA_FILENAME }}}" : "Build/{{{ DATA_FILENAME }}}";
    const frameworkUrl = needsProxyPrefix ? ".proxy/Build/{{{ FRAMEWORK_FILENAME }}}" : "Build/{{{ FRAMEWORK_FILENAME }}}";
    let workerUrl: string | undefined = needsProxyPrefix ? ".proxy/Build/{{{ WORKER_FILENAME }}}" : "Build/{{{ WORKER_FILENAME }}}";
    let codeUrl: string | undefined = needsProxyPrefix ? ".proxy/Build/{{{ CODE_FILENAME }}}" : "Build/{{{ CODE_FILENAME }}}";
    let memoryUrl: string | undefined = needsProxyPrefix ? ".proxy/Build/{{{ MEMORY_FILENAME }}}" : "Build/{{{ MEMORY_FILENAME }}}";
    let symbolsUrl: string | undefined = needsProxyPrefix ? ".proxy/Build/{{{ SYMBOLS_FILENAME }}}" : "Build/{{{ SYMBOLS_FILENAME }}}";

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