import { HASH_RANDOM_BYTES } from "../constants";
import { State } from "./state";

/**
 * General hash functionality.
 * 
 * Hash access should be locked when the game build loads. It doesn't need to be unlocked when loading another build, since the hash to the app directly.
 */
export class HashGenerator {

    #state: State;

    #locked: boolean = false;
    #hash: string | null = null;
    #appHash: string | null = null;

    constructor (state: State) {
        this.#state = state;
    }

    async generateHash(appHash = false): Promise<string> {

        if (appHash && this.#appHash != null) return this.#appHash;
        if (this.#hash != null) return this.#hash;

        //\ Generate random bytes
        const salt = window.crypto.getRandomValues(new Uint8Array(HASH_RANDOM_BYTES));

        //\ Encode timestamp into Uint8Array
        const timestamp = new TextEncoder().encode(Date.now().toString());

        //\ Create key
        const key = new Uint8Array(salt.length + timestamp.length);
        key.set(timestamp);
        key.set(salt, timestamp.length);

        //\ Digest hash
        const hashBuffer = await window.crypto.subtle.digest("SHA-256", key);
        const hashArray = Array.from(new Uint8Array(hashBuffer));
        const hash = hashArray
            .map(item => item.toString(16).padStart(2, "0"))
            .join("");

        //\ Save hash
        if (appHash) this.#appHash = hash;
        else this.#hash = hash;

        return hash;
    }

    async requestHash(): Promise<string | null> {

        if (this.#locked) return null;
        if (this.#state.accessCount >= this.#state.maxAccessCount) return null;

        this.#state.accessCount++;
        
        return await this.generateHash();
    }

    compareHashes(hash1: string, hash2: string): boolean {

        if (hash1.length != hash2.length) return false;

        let result = 0;

        /**
         * For each character of the hashes, assign the XOR comparison.
         * 00[1]10 ^ 00[1]10 = 0
         * 00[0]10 ^ 00[1]10 = 1
         * 
         * So if the hashes are equal, result == 0.
         * This comparison uses constant time.
         */
        for (let i = 0; i < hash1.length; i++) {
            result |= hash1.charCodeAt(i) ^ hash2.charCodeAt(i);
        }

        return result == 0;
    }

    verifyHash(hash: string): boolean {

        return this.verifyAccessHash(hash) || this.verifyAppHash(hash);
    }

    verifyAccessHash(hash: string): boolean {

        if (this.#hash == null) return false;

        return this.compareHashes(hash, this.#hash);
    }

    verifyAppHash(hash: string): boolean {

        if (this.#appHash == null) return false;

        return this.compareHashes(hash, this.#appHash);
    }

    lock(): void {
        this.#locked = true;
    }
}