
import type { PossibleTypeof } from "./types";

//* Check if an object contains certain keys with certain values. Used to verify client-received data.
// (You don't need this, but it's useful if you want to verify data from the client)
export function checkValidity <T extends Record<string, unknown>>(obj: T, expected: Partial<Record<keyof T, PossibleTypeof>>): boolean {

    for (const key in obj) {
        if (typeof obj[key] != typeof expected[key]) return false;
    }

    return true;
}