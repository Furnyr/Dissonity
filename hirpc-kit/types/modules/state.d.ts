import { StateCode } from "../enums";
import { MultiEvent } from "../types";
/**
 * Holds general runtime data.
 *
 * This class exposes all of its members, but it must be private inside the main class.
 */
export declare class State {
    maxAccessCount: number;
    accessCount: number;
    appSender: ((data: string) => void) | null;
    appSenderPromise: Promise<void> | null;
    dispatchAppSender: (() => void) | null;
    appListeners: ((data: unknown) => void)[];
    /**
     * True after a load method is called.
     */
    loaded: boolean;
    stateCode: StateCode;
    readyPromise: Promise<void> | null;
    dispatchReady: (() => void) | null;
    authPromise: Promise<void> | null;
    dispatchAuth: (() => void) | null;
    multiEvent: MultiEvent;
}
