
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
let MAPPINGS:                     string | Mapping[] = '[[[ MAPPINGS ]]]§'; // Mappings have a custom format, no JSON serialized here. (prefix,target,prefix,target...)
let PATCH_URL_MAPPINGS_CONFIG:    string | PatchUrlMappingsConfig = '[[[ PATCH_URL_MAPPINGS_CONFIG ]]]§'; // Mappings config has a custom format, no JSON serialized here. (key,value,key,value...)
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

    if (type == "mappings") {
        const mappingArray: Mapping[] = [];

        const partArray = raw.split(",");

        //? Prevent empty data
        if (partArray.length < 2) return mappingArray;

        let prefix = "";
        for (let i = 0; i < partArray.length; i++) {

            // Prefix
            if (i % 2 == 0) {
                prefix = partArray[i];
                continue;
            }

            // Target
            else {
                let target: string | boolean = partArray[i];

                mappingArray.push({
                    prefix,
                    target
                });
            }
        }

        return mappingArray;
    }

    if (type == "json") {
        const obj: Record<string, string | boolean> = {};

        const partArray = raw.split(",");

        //? Prevent empty data
        if (partArray.length < 2) return obj;

        let key = "";
        for (let i = 0; i < partArray.length; i++) {

            // Key
            if (i % 2 == 0) {
                key = partArray[i];
                continue;
            }

            // Value
            else {
                let value: string | boolean = partArray[i];

                if (/1|true/i.test(value)) value = true;
                else if (/0|false/i.test(value)) value = false;

                obj[key] = value;
            }
        }

        return obj;
    }

    throw new Error("Invalid parse type");
}

function loadBuildVariables() {

    if (loaded) return;

    loaded = true;

    DISABLE_INFO_LOGS = parseBuildVariable(DISABLE_INFO_LOGS as string, "boolean") as boolean;
    CLIENT_ID = parseBuildVariable(CLIENT_ID, "string") as string;
    DISABLE_CONSOLE_LOG_OVERRIDE = parseBuildVariable(DISABLE_CONSOLE_LOG_OVERRIDE as string, "boolean") as boolean;
    MAPPINGS = parseBuildVariable(MAPPINGS as string, "mappings") as Mapping[];
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