import type { Mapping, PatchUrlMappingsConfig } from "../official_types.js";
export declare function formatPrice(price: {
    amount: number;
    currency: string;
}, locale?: string): string;
export declare function patchUrlMappings(mappings: Mapping[], { patchFetch, patchWebSocket, patchXhr, patchSrcAttributes }?: PatchUrlMappingsConfig): void;
