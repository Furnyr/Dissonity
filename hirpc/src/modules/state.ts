
import { StateCode } from "../enums";
import { MultiEvent } from "../types";

/**
 * Holds general runtime data.
 * 
 * This class exposes all of its members, but it must be private inside the main class.
 */
export class State {

    // Hashes
    maxAccessCount = 1;
    accessCount = 0;

    // Comms
    appSender: ((data: string) => void) | null = null;
    appSenderPromise: Promise<void> | null = null;
    dispatchAppSender: (() => void) | null = null;
    appListeners: ((data: unknown) => void)[] = [];

    // RPC
    /**
     * True after a load method is called.
     */
    loaded = false;
    stateCode: StateCode = StateCode.Loading;
    readyPromise: Promise<void> | null = null;
    dispatchReady: (() => void) | null = null;
    authPromise: Promise<void> | null = null;
    dispatchAuth: (() => void) | null = null;
    multiEvent: MultiEvent = {
        ready: "",
        authorize: "",
        authenticate: "",
        response: ""
    };
}