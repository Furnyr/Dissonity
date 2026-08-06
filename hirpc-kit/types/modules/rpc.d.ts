import { Opcode } from "../enums.js";
import { RpcMessage } from "../types.js";
import { State } from "./state.js";
/**
 * Handles communication with the Discord RPC.
 */
export declare class Rpc {
    #private;
    constructor(state: State);
    parseMajorMobileVersion(mobileAppVersion: string): number;
    receive(message: MessageEvent<RpcMessage>): Promise<void>;
    authentication(message: MessageEvent<RpcMessage>): Promise<void>;
    send(opcode: Opcode, payload: unknown): void;
    getNonce(): string;
    serializePayload(payload: unknown): string;
}
