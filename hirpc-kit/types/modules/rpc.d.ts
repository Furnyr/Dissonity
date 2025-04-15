import { Opcode } from "../enums";
import { RpcMessage } from "../types";
import { State } from "./state";
import { OfficialUtils } from "./official_utils";
/**
 * Handles communication with the Discord RPC.
 */
export declare class Rpc {
    #private;
    constructor(state: State, utils: OfficialUtils);
    parseMajorMobileVersion(mobileAppVersion: string): number;
    receive(message: MessageEvent<RpcMessage>): Promise<void>;
    authentication(message: MessageEvent<RpcMessage>): Promise<void>;
    send(opcode: Opcode, payload: unknown): void;
    getNonce(): string;
    serializePayload(payload: unknown): string;
}
