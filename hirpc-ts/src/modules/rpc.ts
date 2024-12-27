import { Opcode } from "../enums";

/**
 * Handles communication with the Discord RPC.
 */
export class Rpc {

    //todo
    public receive(message: MessageEvent) {

    }

    public authentication(message: MessageEvent) {

    }

    public send(opcode: Opcode, payload: unknown) {

        const source: Window = window.parent.opener ?? window.parent;
        const sourceOrigin = !!document.referrer ? document.referrer : "*";

        source.postMessage([opcode, payload], sourceOrigin);
    }
}