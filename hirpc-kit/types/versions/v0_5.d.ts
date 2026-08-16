import { Opcode } from "../enums";
import type { Mapping, PatchUrlMappingsConfig } from "../official_types";
import type { RpcInputPayload } from "../types";
/**
 * Main hiRPC class. After instantiation, the instance will be located in window.dso_hirpc.
 *
 * Imports that must be defined:
 * - dso_bridge/
 * - dso_proxy_bridge/
 */
export default class HiRpcV0_5 {
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
     * Call this method after hiRPC `load` and before loading the game build.
     */
    requestHash(): Promise<string | null>;
    /**
     * Send data to Discord through RPC.
     */
    sendToRpc(hash: string, opcode: Opcode | undefined, payload: RpcInputPayload): Promise<void>;
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
    addAppListener(hash: string, channel: string, listener: (data: unknown) => void): void;
    removeAppListener(hash: string, channel: string, listener: (data: unknown) => void): void;
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

declare class _BuildVariables {
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
type BuildVariables = InstanceType<typeof _BuildVariables>;