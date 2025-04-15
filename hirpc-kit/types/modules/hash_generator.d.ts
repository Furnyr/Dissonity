import { State } from "./state";
/**
 * General hash functionality.
 *
 * Hash access should be locked when the game build loads. It doesn't need to be unlocked when loading another build, since the hash to the app directly.
 */
export declare class HashGenerator {
    #private;
    constructor(state: State);
    generateHash(appHash?: boolean): Promise<string>;
    requestHash(): Promise<string | null>;
    compareHashes(hash1: string, hash2: string): boolean;
    verifyHash(hash: string): boolean;
    verifyAccessHash(hash: string): boolean;
    verifyAppHash(hash: string): boolean;
    lock(): void;
}
