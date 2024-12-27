
import type { Mapping, PatchUrlMappingsConfig } from "../official_types";
import type { ParseVariableType } from "../types";

/**
 * This class is bundled separately.
 * 
 * Its contents will be overwritten by the game engine post-processing the game build.
 * 
 * Each variable is separated by § (alt 21 win)
 */
export default class BuildVariables {

    // After build post-processing, constants look like "[[[ CLIENT_ID ]]] 123454321§"

    public DISABLE_INFO_LOGS:            string | boolean = '[[[ DISABLE_INFO_LOGS ]]]§';
    public CLIENT_ID:                    string = '[[[ CLIENT_ID ]]]§';
    public DISABLE_CONSOLE_LOG_OVERRIDE: string | boolean = '[[[ DISABLE_CONSOLE_LOG_OVERRIDE ]]]§';
    public MAPPINGS:                     string | Mapping[] = '[[[ MAPPINGS ]]]§'; // Keep these single quotes ('), (") breaks the string when the JSON is loaded.
    public PATCH_URL_MAPPINGS_CONFIG:    string | PatchUrlMappingsConfig = '[[[ PATCH_URL_MAPPINGS_CONFIG ]]]§'; // Keep these single quotes ('), (") breaks the string when the JSON is loaded.
    public OAUTH_SCOPES:                 string | string[] = '[[[ OAUTH_SCOPES ]]]§';
    public TOKEN_REQUEST_PATH:           string = '[[[ TOKEN_REQUEST_PATH ]]]§';
    public SERVER_REQUEST:               string = '[[[ SERVER_REQUEST ]]]§'; // Keep these single quotes ('), (") breaks the string when the JSON is loaded.

    public constructor() {
        this.DISABLE_INFO_LOGS = this.parseBuildVariable(this.DISABLE_INFO_LOGS as string, "boolean") as boolean;
        this.CLIENT_ID = this.parseBuildVariable(this.CLIENT_ID, "string") as string;
        this.DISABLE_CONSOLE_LOG_OVERRIDE = this.parseBuildVariable(this.DISABLE_CONSOLE_LOG_OVERRIDE as string, "boolean") as boolean;
        this.MAPPINGS = this.parseBuildVariable(this.MAPPINGS as string, "json") as Mapping[];
        this.PATCH_URL_MAPPINGS_CONFIG = this.parseBuildVariable(this.PATCH_URL_MAPPINGS_CONFIG as string, "json") as PatchUrlMappingsConfig;
        this.OAUTH_SCOPES = this.parseBuildVariable(this.OAUTH_SCOPES as string, "string[]") as string[];
        this.TOKEN_REQUEST_PATH = this.parseBuildVariable(this.TOKEN_REQUEST_PATH, "string") as string;
        this.SERVER_REQUEST = this.parseBuildVariable(this.SERVER_REQUEST, "string") as string;
    }

    private parseBuildVariable(variable: string, type: ParseVariableType): string | boolean | string[] | Mapping[] | PatchUrlMappingsConfig | null {

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

        if (type == "json") {

            try {
                return JSON.parse(raw);
            }

            catch (_err) {
                return null;
            }
        }

        throw new Error("Invalid parse type");
    }
}