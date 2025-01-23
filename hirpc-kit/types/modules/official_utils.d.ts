import type { Mapping, PatchUrlMappingsConfig } from "../official_types";
/**
 * Exposes utils from the Embedded App SDK.
 */
export declare class OfficialUtils {
    formatPrice(price: {
        amount: number;
        currency: string;
    }, locale?: string): string;
    patchUrlMappings(mappings: Mapping[], { patchFetch, patchWebSocket, patchXhr, patchSrcAttributes }?: PatchUrlMappingsConfig): void;
}
