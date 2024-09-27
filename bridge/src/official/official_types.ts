
/*

This file will contain official types that don't require any modification to
work with Dissonity.

Hence, the following code doesn't belong to Dissonity. It belongs to its
corresponding contributors at https://github.com/discord/embedded-app-sdk

*/

// [Patch url mappings functionality] commit source: https://github.com/discord/embedded-app-sdk/pull/252/commits/02b54b71250e2f970709d798fdb132cad3d8d758

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