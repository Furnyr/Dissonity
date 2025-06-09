import { CLOSE_NORMAL, HANDSHAKE_UNKNOWN_VERSION_NUMBER } from "../constants";
import { Opcode, RpcCommands, RpcEvents, StateCode } from "../enums";
import { InteropMessage, RpcMessage } from "../types";

import { State } from "./state";
import { BuildVariables } from "../types";
import { log, logError } from "../logger";
import { OfficialUtils } from "./official_utils";

/**
 * Handles communication with the Discord RPC.
 */
export class Rpc {

    #state: State;
    #utils: OfficialUtils;
    #allowedOrigins = new Set([
        typeof window != "undefined"
            ? window.location.origin
            : "",
        "https://discord.com",
        "https://discordapp.com",
        "https://ptb.discord.com",
        "https://ptb.discordapp.com",
        "https://canary.discord.com",
        "https://canary.discordapp.com",
        "https://staging.discord.co",
        "http://localhost:3333",
        "https://pax.discord.com",
        "null"
    ]);

    constructor(state: State, utils: OfficialUtils) {
        this.#state = state;
        this.#utils = utils;

        // Ensure the class context is preserved in the message event
        this.receive = this.receive.bind(this);
        this.authentication = this.authentication.bind(this);
    }

    parseMajorMobileVersion(mobileAppVersion: string): number {

        if (mobileAppVersion && mobileAppVersion.includes(".")) {
            try {
                return parseInt(mobileAppVersion.split(".")[0]);
            } catch {
                return HANDSHAKE_UNKNOWN_VERSION_NUMBER;
            }
        }
        return HANDSHAKE_UNKNOWN_VERSION_NUMBER;
    }

    async receive(message: MessageEvent<RpcMessage>): Promise<void> {

        if (!this.#allowedOrigins.has(message.origin)) return;

        const rpcMessage = message.data;
        const opcode = rpcMessage?.[0];

        switch (opcode) {

            case Opcode.Handshake: {
                break;
            }

            case Opcode.Hello: {
                // The Hello opcode probably needs to be handled here whenever it's implemented
                break;
            }

            case Opcode.Close: {
                // Receiving close shouldn't do much — the official SDK just parses the message
                break;
            }

            case Opcode.Frame: {

                // Only frame opcodes are sent downward, but I would keep the opcode switch in the app level too.
                // Just in case other opcodes must be sent in the future.

                //? Authenticated
                if (this.#state.stateCode == StateCode.Stable) {

                    await this.#state.appSenderPromise;
                    
                    const interopMessage: InteropMessage = {
                        hirpc_state: this.#state.stateCode,
                        rpc_message: rpcMessage
                    }

                    this.#state.appSender!(this.serializePayload(interopMessage));

                    return;
                }

                // Here, authentication is happening through this.authentication.
                // We only need to act on responses, not trigger new actions.

                const buildVariables = window.dso_build_variables as BuildVariables;

                const payload = rpcMessage?.[1];
                const data = payload?.data;
                const event = payload?.evt;
                const command = payload?.cmd;

                //? Ready event
                if (command == RpcCommands.DISPATCH && event == RpcEvents.READY) {

                    sessionStorage.setItem("dso_connected", "true" as NonNullable<SessionStorage["dso_connected"]>);

                    const multiEvent = this.#state.getMultiEvent();
                    multiEvent.ready = this.serializePayload(data);

                    this.#state.getMultiEvent = () => {
                        return multiEvent;
                    }

                    //? Console log override
                    if (!buildVariables.DISABLE_CONSOLE_LOG_OVERRIDE) {
                        this.#overrideConsoleLogging();
                    }

                    //? Patch url mappings
                    try {
                        const mappings = buildVariables.MAPPINGS;
                        const patchUrlMappingsConfig = buildVariables.PATCH_URL_MAPPINGS_CONFIG;

                        if (mappings.length > 0) {

                            if (!buildVariables.DISABLE_INFO_LOGS) log(`Patching url mappings... (${mappings.length})`);

                            this.#utils.patchUrlMappings(mappings, patchUrlMappingsConfig);
                        }

                    } catch (err) {
                        logError(`Something went wrong patching the URL mappings: ${err}`);
                    }

                    if (!buildVariables.DISABLE_INFO_LOGS) log("Connected to RPC!");

                    this.#state.dispatchReady!();
                }

                //? Authorize
                else if (command == RpcCommands.AUTHORIZE && event != RpcEvents.ERROR) {

                    const multiEvent = this.#state.getMultiEvent();
                    multiEvent.authorize = this.serializePayload(data);

                    this.#state.getMultiEvent = () => {
                        return multiEvent;
                    }
                }

                //? Authenticate
                else if (command == RpcCommands.AUTHENTICATE) {

                    sessionStorage.setItem("dso_authenticated", "true" as NonNullable<SessionStorage["dso_authenticated"]>);

                    const multiEvent = this.#state.getMultiEvent();
                    multiEvent.authenticate = this.serializePayload(data);

                    this.#state.getMultiEvent = () => {
                        return multiEvent;
                    }

                    this.#state.dispatchAuth!();
                    // Multi event is dispatched when the app sender is established (on openDownwardFlow).
                }
            }
        }
    }

    async authentication(message: MessageEvent<RpcMessage>): Promise<void> {

        if (!this.#allowedOrigins.has(message.origin)) return;

        const buildVariables =  window.dso_build_variables as BuildVariables;

        const rpcMessage = message.data;
        const opcode = rpcMessage?.[0];
        const payload = rpcMessage?.[1];

        const data = payload?.data as { code: string };
        const event = payload?.evt;
        const command = payload?.cmd;

        //? Non-frame opcode
        if (opcode != Opcode.Frame) return;

        // Ready is already dispatched at this point
        switch (command) {

            case RpcCommands.AUTHORIZE: {

                //? No authorization
                if (event == RpcEvents.ERROR) {

                    //\ Close bridge
                    this.#state.stateCode = StateCode.Errored;
                    this.#state.errorMessage = "User unauthorized scopes";

                    window.removeEventListener("message", this.receive);
                    window.removeEventListener("message", this.authentication);

                    this.send(Opcode.Close, {
                        code: CLOSE_NORMAL,
                        message: "User unauthorized scopes",
                        nonce: this.getNonce()
                    });

                    return;
                }

                if (!buildVariables.DISABLE_INFO_LOGS) log("Authorized!");

                const tokenRequestPath = buildVariables.TOKEN_REQUEST_PATH;
                const serverRequest = buildVariables.SERVER_REQUEST;

                let body: Record<string, unknown> = {
                    code: data.code
                }

                //? Add user server request
                if (serverRequest != "") {

                    const parsedRequest = JSON.parse(serverRequest);

                    delete parsedRequest.code;

                    body = {
                        code: data.code,
                        ...parsedRequest
                    };
                }

                //\ Send token request
                const response = await fetch(`/.proxy${tokenRequestPath}`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: this.serializePayload(body)
                });
    
                //\ Parse data
                const json = await response.json();
    
                //? No token
                if (!json.token) {

                    const errorMessage = "The server JSON response didn't include a 'token' field";
                    logError(errorMessage);

                    this.#state.stateCode = StateCode.Errored;
                    this.#state.errorMessage = errorMessage;

                    return;
                }
                
                const multiEvent = this.#state.getMultiEvent();
                multiEvent.response = this.serializePayload(json);

                this.#state.getMultiEvent = () => {
                    return multiEvent;
                }

                //\ Authenticate
                this.send(Opcode.Frame, {
                    cmd: RpcCommands.AUTHENTICATE,
                    nonce: this.getNonce(),
                    args: {
                        access_token: json.token
                    }
                });

                break;
            }

            case RpcCommands.AUTHENTICATE: {

                window.removeEventListener("message", this.authentication);

                if (!buildVariables.DISABLE_INFO_LOGS) log("Authenticated!");
            }
        }
    }

    send(opcode: Opcode, payload: unknown): void {

        // The hiRPC needs to work inside a nested iframe. So it can be two iframes in the Discord client.

        let source: Window;
        let sourceOrigin: string;

        const isNested = window.parent != window.parent.parent;
        if (isNested) {

            //todo: remove
            console.log("[Dissonity Debug]: RPC nested");

            const thisParent = window.parent.parent;
            const activity = window.parent;

            source = thisParent.opener ?? thisParent;
            sourceOrigin = !!activity.document.referrer ? activity.document.referrer : "*";
        }

        else {

            const thisParent = window.parent;
            const activity = window;

            //todo: remove
            console.log("[Dissonity Debug]: Not nested RPC");

            source = thisParent.opener ?? thisParent;
            sourceOrigin = !!activity.document.referrer ? activity.document.referrer : "*";
        }

        //todo: remove
        console.log("[Dissonity Debug]: Source is:");
        console.log(source);
        console.log("[Dissonity Debug]: Source origin is:");
        console.log(sourceOrigin);
        console.log("[Dissonity Debug]: Payload is:");
        console.log([opcode, payload]);
        console.log("[Dissonity Debug]: Detailed source:");
        console.log([window.parent.opener, window.parent]);
        console.log("-----------")

        source.postMessage([opcode, payload], sourceOrigin);
    }

    getNonce() {

        const uuid = new Array(36);

        for (let i = 0; i < 36; i++) {
            uuid[i] = Math.floor(Math.random() * 16);
        }

        uuid[14] = 4;
        uuid[19] = uuid[19] &= ~(1 << 2);
        uuid[19] = uuid[19] |= (1 << 3);
        uuid[8] = uuid[13] = uuid[18] = uuid[23] = "-";
        return uuid.map((x) => x.toString(16)).join("");
    }

    serializePayload(payload: unknown): string {

        return JSON.stringify(payload, (_, value) => {
            if (typeof value == "bigint") return value.toString();
            else return value;
        });
    }

    // Literal implementation of overrideConsoleLogging from the official SDK
    #overrideConsoleLogging(): void {

        const consoleLevels = ["log", "warn", "debug", "info", "error"] as const;
        type ConsoleLevel = (typeof consoleLevels)[number];

        const captureLog = (level: ConsoleLevel, message: string) => {

            this.send(Opcode.Frame, {
                cmd: "CAPTURE_LOG",
                nonce: this.getNonce(),
                args: {
                    level,
                    message
                }
            });
        };

        consoleLevels.forEach((level) => {

            const _consoleMethod = console[level];
            const _console = console;

            if (!_consoleMethod) {
                return;
            }

            console[level] = function () {
                const args = [].slice.call(arguments);
                const message = "" + args.join(" ");
                captureLog(level, message);
                _consoleMethod.apply(_console, args);
            }
        });
    }
}