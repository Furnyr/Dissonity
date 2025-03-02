import { log, logError } from "./logger";
import { Opcode, Platform, RpcCommands, StateCode } from "./enums";
import PackageJson from "../package.json";
import { AUTHORIZE_PROMPT, AUTHORIZE_RESPONSE_TYPE, DISSONITY_CHANNEL, HANDSHAKE_ENCODING, HANDSHAKE_SDK_MINIMUM_MOBILE_VERSION,
    HANDSHAKE_VERSION, SDK_VERSION } from "./constants";

import { HashGenerator } from "./modules/hash_generator";
import { State } from "./modules/state";
import { OfficialUtils } from "./modules/official_utils";
import { Rpc } from "./modules/rpc";

import type { HandshakePayload, Mapping, PatchUrlMappingsConfig } from "./official_types";
import type { BuildVariables, DissonityChannelHandshake, InteropMessage, RpcPayload } from "./types";

/**
 * Main hiRPC class. An instance should be located in window.dso_hirpc.
 * 
 * Imports that must be defined:
 * - dso_bridge/
 * - dso_proxy_bridge/
 */ 
export default class HiRpc0_5 {

    #state: State;
    #hashes: HashGenerator;
    #utils: OfficialUtils;
    #rpc: Rpc;

    constructor() {

        //\ Load build variables by accessing them
        const buildVariables = this.getBuildVariables();
    
        if (!buildVariables.DISABLE_INFO_LOGS) {
            this.#greet();
        }

        this.#state = new State();
        this.#hashes = new HashGenerator(this.#state);
        this.#utils = new OfficialUtils();
        this.#rpc = new Rpc(this.#state, this.#utils);

        this.#state.readyPromise = new Promise(resolve => {
            this.#state.dispatchReady = resolve;
        });

        this.#state.authPromise = new Promise(resolve => {
            this.#state.dispatchAuth = resolve;
        });

        this.#state.appSenderPromise = new Promise(resolve => {
            this.#state.dispatchAppSender = resolve;
        });

        if (typeof window != "undefined") {

            //\ Add main RPC listener
            window.addEventListener("message", this.#rpc.receive);
        }
    }

    /**
     * Load the hiRPC module once per runtime.
     * 
     * Depends on the external RPC state tracked via `sessionStorage`.
     * 
     * ```js
     * // Trigger a parallel load:
     * sessionStorage.setItem("dso_connected", true);
     * 
     * // Trigger a soft load:
     * sessionStorage.setItem("dso_connected", true);
     * sessionStorage.setItem("dso_authenticated", true);
     * ```
     */
    async load(maxAccessCount = 1): Promise<void> {

        if (this.#state.loaded) {
            throw new Error("hiRPC Module already loaded");
        }

        //? Web environment
        if (typeof window == "undefined") {

            this.#state.stateCode = StateCode.Unfunctional;

            throw new Error("Cannot load hiRPC Module outside of a web environment");
        }

        //\ Ready to request hash
        this.#state.loaded = true;
        this.#state.maxAccessCount = maxAccessCount;

        //? Clear RPC session storage
        const query = this.getQueryObject();
        const saveInstanceId = sessionStorage.getItem("dso_instance_id") as SessionStorage["dso_instance_id"];
        if (query.instance_id != saveInstanceId) {

            // If these ids don't match, it means that the hiRPC kit isn't enabled,
            // so the RPC external session values can be outdated.

            // In this case, RPC session storage is only used to prevent
            // initialization triggering multiple times.

            // Meaningless casts to provoke an error if the SessionStorage property changes.
            sessionStorage.removeItem("dso_connected" as NonNullable<SessionStorage["dso_connected"]>);
            sessionStorage.removeItem("dso_authenticated" as NonNullable<SessionStorage["dso_authenticated"]>);

            sessionStorage.setItem("dso_instance_id", query.instance_id as NonNullable<SessionStorage["dso_instance_id"]>);
        }
        
        return new Promise(async (resolve, reject) => {

            //? In browser
            const outsideDiscord = sessionStorage.getItem("dso_outside_discord") as SessionStorage["dso_outside_discord"];
            if (outsideDiscord == "true") {
    
                // For browser-only environments, returning at this point provides basic hiRPC functionality
                
                this.#state.stateCode = StateCode.OutsideDiscord;
    
                resolve();
                return;
            }
    
            const query = this.getQueryObject();
    
            //? No query params
            if (!query.frame_id || !query.instance_id || !query.platform) {
                
                this.#state.stateCode = StateCode.OutsideDiscord;
    
                resolve();
                return;
            }
    
            //? Send handshake
            const connected = sessionStorage.getItem("dso_connected") as SessionStorage["dso_connected"];
            if (connected == "false" || connected == null) {
                await this.#connect();
            }
    
            else if (connected == "true") {
                this.#state.dispatchReady!();
            }
    
            else {
                reject(new Error(`Invalid session storage item: { dso_connected: ${connected} }`));
                return;
            }
    
            //? Authenticate
            const authenticated = sessionStorage.getItem("dso_authenticated") as SessionStorage["dso_authenticated"];
            if (authenticated == "false" || authenticated == null) {
                await this.#authenticate();
            }
    
            else if (authenticated == "true") {
                this.#state.dispatchAuth!();
            }
    
            else {
                reject(new Error(`Invalid session storage item: { dso_authenticated: ${authenticated} }`));
                return;
            }
    
            this.#state.stateCode = StateCode.Stable;
            resolve();
        });
    }

    /**
     * Send a handshake through RPC.
     * 
     * - Resolves readyPromise
     */
    async #connect(): Promise<void> {
        
        const query = this.getQueryObject();

        const buildVariables = this.getBuildVariables();

        const clientId = buildVariables.CLIENT_ID;
        const disableInfoLogs = buildVariables.DISABLE_INFO_LOGS;
        const majorMobileVersion = this.#rpc.parseMajorMobileVersion(query.mobile_app_version);

        const payload: HandshakePayload = {
            v: HANDSHAKE_VERSION,
            encoding: HANDSHAKE_ENCODING,
            client_id: clientId,
            frame_id: query.frame_id
        };

        if (query.platform === Platform.DESKTOP || majorMobileVersion >= HANDSHAKE_SDK_MINIMUM_MOBILE_VERSION) {
            payload["sdk_version"] = SDK_VERSION;
        }

        if (!disableInfoLogs) log("Connecting...");

        this.#rpc.send(Opcode.Handshake, payload);
        await this.#state.readyPromise;
    }

    /**
     * Begin authorization to authenticate through RPC.
     * 
     * - Resolves authPromise
     */
    async #authenticate(): Promise<void> {

        //\ Add authentication RPC listener
        window.addEventListener("message", this.#rpc.authentication);

        const buildVariables = this.getBuildVariables();
        
        this.#rpc.send(Opcode.Frame, {
            cmd: RpcCommands.AUTHORIZE,
            nonce: this.getNonce(),
            args: {
                client_id: buildVariables.CLIENT_ID,
                scope: buildVariables.OAUTH_SCOPES,
                response_type: AUTHORIZE_RESPONSE_TYPE,
                prompt: AUTHORIZE_PROMPT,
                state: ""
            }
        });

        await this.#state.authPromise;
    }

    #greet() {
        log(`hiRPC! version ${PackageJson.version}`);
    }

    //# LIBRARY - - - - -
    getBuildVariables(): BuildVariables {

        if (typeof window.dso_build_variables == "object") {
            return window.dso_build_variables as BuildVariables;
        }

        //? Not deployed, but imported
        else if (typeof window.Dissonity.BuildVariables == "object") {

            window.dso_build_variables = new (window.Dissonity.BuildVariables).default();

            return window.dso_build_variables as BuildVariables;
        }

        else {
            logError("Unable to access build variables. Import them through <import map>/dissonity_build_variables.js");

            this.#state.stateCode = StateCode.Errored;

            throw new Error("Unable to access build variables. Import them through <import map>/dissonity_build_variables.js")
        }
    }

    //# UTILS - - - - -
    patchUrlMappings(hash: string, mappings: Mapping[], config?: PatchUrlMappingsConfig): void {

        if (!this.#hashes.verifyHash(hash)) return;

        this.#utils.patchUrlMappings(mappings, config);
    }

    formatPrice(hash: string, price: {amount: number; currency: string}, locale?: string): string | undefined {

        if (!this.#hashes.verifyHash(hash)) return;

        return this.#utils.formatPrice(price, locale);
    }

    //# API - - - - -
    getQueryObject(): Record<string, string> {

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
    
    getNonce(): string {
        return this.#rpc.getNonce();
    }

    getVersions(): { embedded_app_sdk: string, hirpc: string } {
        return {
            embedded_app_sdk: SDK_VERSION,
            hirpc: PackageJson.version
        };
    }

    /**
     * Request a hash to access restricted functionality.
     * 
     * Call this method after hiRPC `load` and before loading the game build.
     */
    async requestHash(): Promise<string | null> {

        if (!this.#state.loaded) return null;

        return this.#hashes.requestHash();
    }

    /**
     * Send data to Discord through RPC.
     */
    async sendToRpc(hash: string, opcode = Opcode.Frame, payload: RpcPayload): Promise<void> {

        if (!this.#hashes.verifyHash(hash)) return;

        await this.#state.readyPromise;

        this.#rpc.send(opcode, payload);
    }

    /**
     * **Only used inside the game build.**
     * 
     * Send data to the JavaScript layer.
     */
    sendToJs(appHash: string, channel: string, payload: unknown): void {

        if (!this.#hashes.verifyAppHash(appHash)) return;
        if (channel == DISSONITY_CHANNEL) throw new Error(`Cannot send message through the "${DISSONITY_CHANNEL}" channel`);

        const channelListeners = this.#state.appListeners.get(channel);

        if (!channelListeners) return;

        for (const listener of channelListeners) {
            listener(payload);
        }
    }

    /**
     * Send data to the game build through a hiRPC channel.
     */
    async sendToApp(hash: string, channel: string, payload: unknown): Promise<void> {

        if (this.#hashes.verifyAccessHash(hash)) {
            if (channel == DISSONITY_CHANNEL) throw new Error(`Cannot send message through the "${DISSONITY_CHANNEL}" channel`);
        }

        else if (!this.#hashes.verifyAppHash(hash)) return;

        await this.#state.appSenderPromise;

        const interopMessage: InteropMessage = {
            hirpc_state: this.#state.stateCode,
            hirpc_message: {
                channel,
                data: payload
            }
        };

        this.#state.appSender!(this.#rpc.serializePayload(interopMessage));
    }

    addAppListener(hash: string, channel: string, listener: (data: unknown) => void): void {

        if (!this.#hashes.verifyHash(hash)) return;

        if (!this.#state.appListeners.has(channel)) {
            this.#state.appListeners.set(channel, []);
        }

        const channelListeners = this.#state.appListeners.get(channel)!;

        channelListeners.push(listener);
    }

    removeAppListener(hash: string, channel: string, listener: (data: unknown) => void): void {

        if (!this.#hashes.verifyHash(hash)) return;
        if (!this.#state.appListeners.has(channel)) return;

        const channelListeners = this.#state.appListeners.get(channel)!;

        const index = channelListeners.indexOf(listener);

        if (index == -1) return;

        channelListeners.splice(index, 1);

        if (channelListeners.length == 0) {
            this.#state.appListeners.delete(channel);
        }
    }

    /**
     * Lock hash access before opening the downward flow. After this call, `requestHash` will return null.
     */
    lockHashAccess(): void {

        this.#hashes.lock();
    }

    /**
     * Define the function that will be used to send data to the game build.
     * 
     * The first message sent will be the `MultiEvent` after authentication. Depending on the RPC state, it could contain partial data.
     * 
     * Hash access will be locked after this call, if it wasn't locked already.
     */
    async openDownwardFlow(appSender: (data: string) => void): Promise<void> {

        if (this.#state.appSender != null) throw new Error("An app sender has already been defined");

        this.#hashes.lock();
        this.#state.appSender = appSender;

        const outsideDiscord = sessionStorage.getItem("dso_outside_discord") as SessionStorage["dso_outside_discord"];
        if (outsideDiscord != "true") {
            await this.#state.authPromise;
        }

        const hiRpcPayload: DissonityChannelHandshake = {
            raw_multi_event: outsideDiscord != "true" ? this.#state.getMultiEvent() : null,
            hash: await this.#hashes.generateHash(true)
        }

        const interopMessage: InteropMessage = {
            hirpc_state: this.#state.stateCode,
            hirpc_message: {
                channel: DISSONITY_CHANNEL,
                data: hiRpcPayload,
                opening: true
            }
        };

        this.#state.appSender!(this.#rpc.serializePayload(interopMessage));

        this.#state.dispatchAppSender!();
    }

    /**
     * Free the app sender. Hash access will continue to be locked.
     */
    closeDownwardFlow(hash: string): void {

        if (!this.#hashes.verifyHash(hash)) return;
        if (this.#state.appSender == null) return;

        this.#state.appSenderPromise = new Promise(resolve => {
            this.#state.dispatchAppSender = resolve;
        });

        this.#state.appSender = null;
    }
}