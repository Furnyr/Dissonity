import type { Mapping, PatchUrlMappingsConfig } from "../official_types";
/**
 * This class is bundled separately.
 *
 * Its contents will be overwritten by the game engine post-processing the game build.
 *
 * Each variable is separated by § (alt 21 win) (\u00A7)
 */
export default class BuildVariables {
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
