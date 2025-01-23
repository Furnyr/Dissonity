interface UnknownHiRpc {
}

/**
 * Discord RPC Opcode
 */
declare enum Opcode {
    Handshake = 0,
    Frame = 1,
    Close = 2,
    Hello = 3
}

interface Mapping {
    prefix: string;
    target: string;
}
interface PatchUrlMappingsConfig {
    patchFetch?: boolean;
    patchWebSocket?: boolean;
    patchXhr?: boolean;
    patchSrcAttributes?: boolean;
}

/**
 * This class is bundled separately.
 *
 * Its contents will be overwritten by the game engine post-processing the game build.
 *
 * Each variable is separated by § (alt 21 win) (\u00A7)
 */
declare class BuildVariables$1 {
    #private;
    DISABLE_INFO_LOGS: boolean;
    CLIENT_ID: string;
    DISABLE_CONSOLE_LOG_OVERRIDE: boolean;
    MAPPINGS: Mapping[];
    PATCH_URL_MAPPINGS_CONFIG: PatchUrlMappingsConfig;
    OAUTH_SCOPES: string[];
    TOKEN_REQUEST_PATH: string;
    SERVER_REQUEST: string;
    constructor();
}

type RpcPayload = {
    evt?: string;
    cmd?: string;
    nonce?: string;
    data?: unknown;
};
type BuildVariables = InstanceType<typeof BuildVariables$1>;

/**
 * Main hiRPC class. An instance should be located in window.dso_hirpc.
 *
 * Since this class is loaded from the hiRPC interface, we can assume the following properties exist:
 * - window.dso_hirpc
 * - sessionStorage.dso_outside_discord
 * - sessionStorage.dso_needs_prefix
 *
 * Imports that must be defined:
 * - dso_bridge/
 * - dso_proxy_bridge/
 */
declare class HiRpc0_4_0 {
    #private;
    constructor();
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
    load(maxAccessCount?: number): Promise<void>;
    getBuildVariables(): BuildVariables;
    patchUrlMappings(hash: string, mappings: Mapping[], config?: PatchUrlMappingsConfig): void;
    formatPrice(hash: string, price: {
        amount: number;
        currency: string;
    }, locale?: string): string | undefined;
    getQueryObject(): Record<string, string>;
    getNonce(): string;
    getVersions(): {
        embedded_app_sdk: string;
        hirpc: string;
    };
    /**
     * Request a hash to access restricted functionality.
     *
     * Call this method before loading the game build.
     */
    requestHash(): Promise<string | null>;
    /**
     * Send data to Discord through RPC.
     */
    sendToRpc(hash: string, opcode: Opcode | undefined, payload: RpcPayload): Promise<void>;
    /**
     * **Only used inside the game build.**
     *
     * Send data to the JavaScript layer.
     */
    sendToJs(appHash: string, channel: string, payload: unknown): void;
    /**
     * Send data to the game build through a hiRPC channel.
     */
    sendToApp(hash: string, channel: string, payload: unknown): Promise<void>;
    addAppListener(hash: string, listener: (data: unknown) => void): void;
    removeAppListener(hash: string, listener: (data: unknown) => void): void;
    /**
     * Lock hash access before opening the downward flow. After this call, `requestHash` will return null.
     */
    lockHashAccess(): void;
    /**
     * Define the function that will be used to send data to the game build.
     *
     * The first message sent will be the `MultiEvent` after authentication. Depending on the RPC state, it could contain partial data.
     *
     * Hash access will be locked after this call, if it wasn't locked already.
     */
    openDownwardFlow(appSender: (data: string) => void): Promise<void>;
    /**
     * Free the app sender. Hash access will continue to be locked.
     */
    closeDownwardFlow(hash: string): void;
}

type StartWith<V extends string> = `${V}${string}`;
type HiRpc<V extends string> = V extends StartWith<"0.4"> ? HiRpc0_4_0 : UnknownHiRpc;

declare function setupHiRpc<V extends string>(_hiRpcVersion: V): Promise<HiRpc<V>>;

export { setupHiRpc };
