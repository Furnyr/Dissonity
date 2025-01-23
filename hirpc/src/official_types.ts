
/*

    Types from the Embedded App SDK.

*/

export interface Mapping {
    prefix: string;
    target: string;
}

export interface RemapInput {
    url: URL;
    mappings: Mapping[];
}

export interface PatchUrlMappingsConfig {
    patchFetch?: boolean;
    patchWebSocket?: boolean;
    patchXhr?: boolean;
    patchSrcAttributes?: boolean;
}

export interface MatchAndRewriteURLInputs {
    originalURL: URL;
    prefixHost: string;
    prefix: string;
    target: string;
}

export interface HandshakePayload {
    v: number;
    encoding: string;
    client_id: string;
    frame_id: string;
    sdk_version?: string;
}