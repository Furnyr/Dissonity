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
    #verifyHash: (hash: string | null) => boolean | null = () => null;
    #verifyAppHash: (hash: string | null) => boolean | null = () => null;

    constructor (state: State) {
        this.#state = state;
    }

    async generateHash(appHash = false): Promise<string> {

        if (appHash) {

            //? App hash already locked
            if (this.#verifyAppHash(null) != null) {
                throw new Error("App hash is locked");
            }
        }

        else {

            //? Access hash available
            if (this.#hash != null) {
                return this.#hash;
            }

            //? Access hash already locked
            if (this.#verifyHash(null) != null) {
                throw new Error("Access hash is locked");
            }
        }

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

        if (appHash) {

            // Check closure
            this.#verifyAppHash = (testHash) => {

                if (testHash == null) return false;

                return this.compareHashes(testHash, hash);
            }
        }

        else {

            // Save temporarily to serve later
            this.#hash = hash;

            // Check closure
            this.#verifyHash = (testHash) => {

                if (testHash == null) return false;

                return this.compareHashes(testHash, hash);
            }
        }

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

        return !!this.#verifyHash(hash) || !!this.#verifyAppHash(hash);
    }

    verifyAccessHash(hash: string): boolean {

        return !!this.#verifyHash(hash);
    }

    verifyAppHash(hash: string): boolean {

        return !!this.#verifyAppHash(hash);
    }

    lock(): void {
        this.#hash = null;
        this.#locked = true;
    }
}