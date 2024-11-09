
/*

Build variables are replaced by post-processing the Unity build.

They are separated by "§" (alt 21 win)

*/

//todo SDK_VERSION will probably be directly inside the hiRPC

import type { ParseVariableType } from "./types";

let loaded = false;

//# BUILD VARIABLES - - - - -
// After build post-processing, constants look like "[[[ SDK_VERSION ]]] 1.5.0§"

let HANDSHAKE_SDK_VERSION:        string = '[[[ SDK_VERSION ]]]§'; // Embedded App SDK version that Dissonity is simulating
let DISABLE_INFO_LOGS:            string | boolean = '[[[ DISABLE_INFO_LOGS ]]]§';
let CLIENT_ID:                    string = '[[[ CLIENT_ID ]]]§';
let DISABLE_CONSOLE_LOG_OVERRIDE: string | boolean = '[[[ DISABLE_CONSOLE_LOG_OVERRIDE ]]]§';
let MAPPINGS:                     string | Map<string, string> = '[[[ MAPPINGS ]]]§'; // Mappings have a custom format, no JSON serialized here
let PATCH_URL_MAPPINGS_CONFIG:    string | Map<string, boolean> = '[[[ PATCH_URL_MAPPINGS_CONFIG ]]]§';
let OAUTH_SCOPES:                 string | string[] = '[[[ OAUTH_SCOPES ]]]§';
let TOKEN_REQUEST_PATH:           string = '[[[ TOKEN_REQUEST_PATH ]]]§';
let SERVER_REQUEST:               string | Record<string, unknown> = '[[[ SERVER_REQUEST ]]]§'; // Keep these single quotes ('), (") breaks the string when the JSON is loaded.

function parseBuildVariable(variable: string, type: ParseVariableType): string | boolean | string[] | Map<string, string | boolean> | Record<string, unknown> | null {

    let raw: string;

    try {
        raw = variable.split("]]] ")[1].slice(0, -1);
    } catch(err) {
        throw(new Error("[Dissonity Utils]: Build variable undefined"));
    }
  
    if (type == "string") return raw;
  
    if (type == "boolean") {
  
        if (/1|true/i.test(variable)) return true;
        else if (/0|false/i.test(variable)) return false;
        throw new Error("[Dissonity Utils]: Invalid boolean string");
    }
  
    if (type == "string[]") {
        const array = raw.split(",");
        if (array.length == 1 && array[0] == "") return [];
        return array;
    }
  
    if (type == "json") {
        try
        {
            return JSON.parse(raw);
        }
        catch
        {
            return null;
        }
    }

    const map = new Map<string, string | boolean>();

    const array = raw.split(",");

    //? Prevent empty data
    if (array.length < 2) return map;

    let lastValue: string | boolean = "";
    for (let i = 0; i < array.length; i++) {
        
        // Key
        if (i % 2 == 0) {

            lastValue = array[i];
            continue;
        }

        // Value
        else {

            const value = array[1];
            
            if (value == "True") map.set(lastValue, true);
            else if (value == "False") map.set(lastValue, false);
            else map.set(lastValue, value);

            continue;
        }
    }

    return map;
}

function loadBuildVariables() {

    if (loaded) return;

    loaded = true;

    HANDSHAKE_SDK_VERSION = parseBuildVariable(HANDSHAKE_SDK_VERSION, "string") as string;
    DISABLE_INFO_LOGS = parseBuildVariable(DISABLE_INFO_LOGS as string, "boolean") as boolean;
    CLIENT_ID = parseBuildVariable(CLIENT_ID, "string") as string;
    DISABLE_CONSOLE_LOG_OVERRIDE = parseBuildVariable(DISABLE_CONSOLE_LOG_OVERRIDE as string, "boolean") as boolean;
    MAPPINGS = parseBuildVariable(MAPPINGS as string, "map") as Map<string, string>;
    PATCH_URL_MAPPINGS_CONFIG = parseBuildVariable(PATCH_URL_MAPPINGS_CONFIG as string, "map") as Map<string, boolean>;
    OAUTH_SCOPES = parseBuildVariable(OAUTH_SCOPES as string, "string[]") as string[];
    TOKEN_REQUEST_PATH = parseBuildVariable(TOKEN_REQUEST_PATH, "string") as string;
    SERVER_REQUEST = parseBuildVariable(SERVER_REQUEST as string, "json") as Record<string, unknown>;
}

export default function () {
    
    loadBuildVariables();

    return {
        HANDSHAKE_SDK_VERSION,
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