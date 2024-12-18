
/*

Build variables are replaced by post-processing the Unity build.

They are separated by "§" (alt 21 win)

*/

import type { Mapping, PatchUrlMappingsConfig } from "./official_types";
import type { ParseVariableType } from "./types";

let loaded = false;

//# BUILD VARIABLES - - - - -
// After build post-processing, constants look like "[[[ CLIENT_ID ]]] 123454321§"

let DISABLE_INFO_LOGS:            string | boolean = '[[[ DISABLE_INFO_LOGS ]]]§';
let CLIENT_ID:                    string = '[[[ CLIENT_ID ]]]§';
let DISABLE_CONSOLE_LOG_OVERRIDE: string | boolean = '[[[ DISABLE_CONSOLE_LOG_OVERRIDE ]]]§';
let MAPPINGS:                     string | Mapping[] = '[[[ MAPPINGS ]]]§'; // Keep these single quotes ('), (") breaks the string when the JSON is loaded.
let PATCH_URL_MAPPINGS_CONFIG:    string | PatchUrlMappingsConfig = '[[[ PATCH_URL_MAPPINGS_CONFIG ]]]§'; // Keep these single quotes ('), (") breaks the string when the JSON is loaded.
let OAUTH_SCOPES:                 string | string[] = '[[[ OAUTH_SCOPES ]]]§';
let TOKEN_REQUEST_PATH:           string = '[[[ TOKEN_REQUEST_PATH ]]]§';
let SERVER_REQUEST:               string = '[[[ SERVER_REQUEST ]]]§'; // Keep these single quotes ('), (") breaks the string when the JSON is loaded.

function parseBuildVariable(variable: string, type: ParseVariableType): string | boolean | string[] | Mapping[] | PatchUrlMappingsConfig | null {

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

function loadBuildVariables() {

    if (loaded) return;

    loaded = true;

    DISABLE_INFO_LOGS = parseBuildVariable(DISABLE_INFO_LOGS as string, "boolean") as boolean;
    CLIENT_ID = parseBuildVariable(CLIENT_ID, "string") as string;
    DISABLE_CONSOLE_LOG_OVERRIDE = parseBuildVariable(DISABLE_CONSOLE_LOG_OVERRIDE as string, "boolean") as boolean;
    MAPPINGS = parseBuildVariable(MAPPINGS as string, "json") as Mapping[];
    PATCH_URL_MAPPINGS_CONFIG = parseBuildVariable(PATCH_URL_MAPPINGS_CONFIG as string, "json") as PatchUrlMappingsConfig;
    OAUTH_SCOPES = parseBuildVariable(OAUTH_SCOPES as string, "string[]") as string[];
    TOKEN_REQUEST_PATH = parseBuildVariable(TOKEN_REQUEST_PATH, "string") as string;
    SERVER_REQUEST = parseBuildVariable(SERVER_REQUEST, "string") as string;
}

export default function () {
    
    loadBuildVariables();

    return {
        DISABLE_INFO_LOGS,
        CLIENT_ID,
        DISABLE_CONSOLE_LOG_OVERRIDE,
        MAPPINGS,
        PATCH_URL_MAPPINGS_CONFIG,
        OAUTH_SCOPES,
        TOKEN_REQUEST_PATH,
        SERVER_REQUEST
    };
}