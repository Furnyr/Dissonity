
import type { Mapping, PatchUrlMappingsConfig } from "../official_types";
import type { ParseVariableType } from "../types";

/**
 * This class is bundled separately.
 * 
 * Its contents will be overwritten by the game engine post-processing the game build.
 * 
 * Each variable is separated by § (alt 21 win) (\u00A7)
 */
export default class BuildVariables {

    // After build post-processing, constants look like "[[[ CLIENT_ID ]]] 123454321§"

    #DISABLE_INFO_LOGS             = '[[[ DISABLE_INFO_LOGS ]]]§';
    #CLIENT_ID                     = '[[[ CLIENT_ID ]]]§';
    #DISABLE_CONSOLE_LOG_OVERRIDE  = '[[[ DISABLE_CONSOLE_LOG_OVERRIDE ]]]§';
    #MAPPINGS                      = '[[[ MAPPINGS ]]]§'; // Keep these single quotes ('), (") breaks the string when the JSON is loaded.
    #PATCH_URL_MAPPINGS_CONFIG     = '[[[ PATCH_URL_MAPPINGS_CONFIG ]]]§'; // Keep these single quotes ('), (") breaks the string when the JSON is loaded.
    #OAUTH_SCOPES                  = '[[[ OAUTH_SCOPES ]]]§';
    #TOKEN_REQUEST_PATH            = '[[[ TOKEN_REQUEST_PATH ]]]§';
    #SERVER_REQUEST                = '[[[ SERVER_REQUEST ]]]§'; // Keep these single quotes ('), (") breaks the string when the JSON is loaded.

    DISABLE_INFO_LOGS:            boolean;
    CLIENT_ID:                    string;
    DISABLE_CONSOLE_LOG_OVERRIDE: boolean;
    MAPPINGS:                     Mapping[];
    PATCH_URL_MAPPINGS_CONFIG:    PatchUrlMappingsConfig;
    OAUTH_SCOPES:                 string[];
    TOKEN_REQUEST_PATH:           string;
    SERVER_REQUEST:               string;

    constructor() {
        this.DISABLE_INFO_LOGS = this.#parseBuildVariable(this.#DISABLE_INFO_LOGS, "boolean") as boolean;
        this.CLIENT_ID = this.#parseBuildVariable(this.#CLIENT_ID, "string") as string;
        this.DISABLE_CONSOLE_LOG_OVERRIDE = this.#parseBuildVariable(this.#DISABLE_CONSOLE_LOG_OVERRIDE, "boolean") as boolean;
        this.MAPPINGS = this.#parseBuildVariable(this.#MAPPINGS, "json_array") as Mapping[];
        this.PATCH_URL_MAPPINGS_CONFIG = this.#parseBuildVariable(this.#PATCH_URL_MAPPINGS_CONFIG, "json") as PatchUrlMappingsConfig;
        this.OAUTH_SCOPES = this.#parseBuildVariable(this.#OAUTH_SCOPES, "string[]") as string[];
        this.TOKEN_REQUEST_PATH = this.#parseBuildVariable(this.#TOKEN_REQUEST_PATH, "string") as string;
        this.SERVER_REQUEST = this.#parseBuildVariable(this.#SERVER_REQUEST, "string") as string;
    }

    #parseBuildVariable(variable: string, type: ParseVariableType): string | boolean | string[] | Mapping[] | PatchUrlMappingsConfig | null {

        let raw: string;

        try {
            raw = variable.split("]]] ")[1].slice(0, -1);
        } catch(err) {
            throw(new Error("Build variable undefined"));
        }
    
        if (type == "string") return raw;
    
        if (type == "boolean") {
    
            if (/1|true/i.test(variable)) return true;
            else if (/0|false/i.test(variable)) return false;
            throw new Error("Invalid boolean string");
        }
    
        if (type == "string[]") {
            const array = raw.split(",");
            if (array.length == 1 && array[0] == "") return [];
            return array;
        }

        if (type == "json" || type == "json_array") {

            try {
                return JSON.parse(raw);
            }

            catch (_err) {
                if (type == "json_array") return [];
                return null;
            }
        }

        throw new Error("Invalid parse type");
    }
}