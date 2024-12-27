import { HASH_RANDOM_BYTES } from "../constants";

/**
 * General hash functionality.
 * 
 * Hash generation should be locked when the game build loads, and unlocked before loading another build.
 */
export class HashGenerator {

    private locked: boolean = false;
    private hashes: string[] = [];
    private app_hash: string | null = null;

    public async generateHash(): Promise<string> {
        
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
        this.hashes.push(hash);

        return hash;
    }

    public compareHashes(hash1: string, hash2: string): boolean {

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

    public verifyHash(hash: string): boolean {

        for (const generatedHash in this.hashes) {
            if (this.compareHashes(hash, generatedHash)) {
                return true;
            }
        }

        return false;
    }
}