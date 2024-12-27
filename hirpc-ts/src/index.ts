import { Opcode, Platform, StateCode } from "./enums";
import PackageJson from "../package.json"
import { HANDSHAKE_ENCODING, HANDSHAKE_SDK_MINIMUM_MOBILE_VERSION,
    HANDSHAKE_UNKNOWN_VERSION_NUMBER, HANDSHAKE_VERSION, SDK_VERSION } from "./constants";

import { HashGenerator } from "./modules/hash_generator";
import { State } from "./modules/state";
import { OfficialUtils } from "./modules/official_utils";

import type { HandshakePayload } from "./official_types";
import type { BuildVariables, RpcMessage } from "./types";
import type IShape from "./shape";
import { Rpc } from "./modules/rpc";

/**
 * Main hiRPC class. An instance would normally live in window.dso_hirpc.
 * 
 * Since this class is loaded from the hiRPC interface, we can assume the following properties exist:
 * - window.dso_hirpc
 * - window.outside_discord
 * - window.dso_needs_suffix
 */
export default class implements IShape {

    private state: State;
    private hashes: HashGenerator;
    private utils: OfficialUtils;
    private rpc: Rpc;

    public constructor() {

        this.state = new State();
        this.hashes = new HashGenerator();
        this.utils = new OfficialUtils();
        this.rpc = new Rpc();

        this.state.readyPromise = new Promise((resolve) => {
            this.state.dispatchReady = resolve;
        });
    }

    //# META - - - - -
    private log(message?: any) {
        console.log(`%c[DissonityHiRpc]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    }

    private logError(message?: any) {
        console.error(`%c[DissonityHiRpc]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    }

    private greet() {
        this.log(`hiRPC! version ${PackageJson.version}`);
    }

    private parseMajorMobileVersion(mobileAppVersion: string): number {

        if (mobileAppVersion && mobileAppVersion.includes(".")) {
            try {
                return parseInt(mobileAppVersion.split(".")[0]);
            } catch {
                return HANDSHAKE_UNKNOWN_VERSION_NUMBER;
            }
        }
        return HANDSHAKE_UNKNOWN_VERSION_NUMBER;
    }

    //# PUBLIC - - - - -
    public getQueryObject(): Record<string, string> {

        const isParent = window.location.search.includes("?");

        const queryStringKeyValue = (isParent)
            ? window.location.search.replace("?", "").split("&")
            : window.parent.location.search.replace("?", "").split("&");

        const qsJsonObject: Record<string, string> = {};
        if (queryStringKeyValue.length != 0) {
            for (let i = 0; i < queryStringKeyValue.length; i++) {
                qsJsonObject[queryStringKeyValue[i].split("=")[0]] = queryStringKeyValue[i].split("=")[1];
            }
        }

        return qsJsonObject;
    }

    public async connect(hash: string): Promise<boolean> {

        if (!this.hashes.verifyHash(hash)) return false;
        if (this.state.initialized) return false;

        this.state.initialized = true;

        //? Web environment
        if (typeof window == undefined) {

            this.state.stateCode = StateCode.Unfunctional;

            this.logError("Not in a web environment");

            return false;
        }

        //\ Load build variables
        if (window.dso_needs_suffix)
        window.dso_build_variables = new window["Dissonity.BuildVariables"].default();

        this.greet();

        //? In browser
        if (window.dso_outside_discord) {

            this.state.stateCode = StateCode.OutsideDiscord;

            return false;
        }

        const query = this.getQueryObject();

        //? No query params
        if (!query.frame_id || !query.instance_id || !query.platform) {

            this.state.stateCode = StateCode.OutsideDiscord;

            return false;
        }

        //\ Add main RPC listener
        window.addEventListener("message", this.rpc.receive);

        //? Not connected
        if (!window.dso_connected) {

            window.addEventListener("message", this.rpc.authentication);

            const clientId = (window.dso_build_variables as BuildVariables).CLIENT_ID;
            const disableInfoLogs = (window.dso_build_variables as BuildVariables).DISABLE_INFO_LOGS;
            const majorMobileVersion = this.parseMajorMobileVersion(query.mobile_app_version);

            const payload: HandshakePayload = {
                v: HANDSHAKE_VERSION,
                encoding: HANDSHAKE_ENCODING,
                client_id: clientId,
                frame_id: query.frame_id
            }

            if (query.platform === Platform.DESKTOP || majorMobileVersion >= HANDSHAKE_SDK_MINIMUM_MOBILE_VERSION) {
                payload["sdk_version"] = SDK_VERSION;
            }

            if (!disableInfoLogs) this.log("Connecting...");

            this.rpc.send(Opcode.Handshake, payload);
            await this.state.readyPromise;
        }

        //? Connected
        else {

            this.state.stateCode = StateCode.Stable;
            this.state.ready = true;
            this.state.dispatchReady!();
        }

        return true;
    }

    public test() {
        return new HashGenerator().generateHash();
    }

    //todo
    public addRpcListener(hash: string, listener: (message: RpcMessage) => void) {

    }

    public removeRpcListener(hash: string, listener: (message: RpcMessage) => void) {

    }

    public async sendToRpc(hash: string, opcode = Opcode.Frame, payload: unknown) {

        if (!this.hashes.verifyHash(hash)) return false;

        if (!this.state.ready) {
            await this.state.readyPromise;
        }

        this.rpc.send(opcode, payload);
    }
}