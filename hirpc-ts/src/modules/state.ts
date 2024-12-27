
import { StateCode } from "../enums";

/**
 * Holds general runtime data.
 * 
 * This class exposes all of its members, but it must be private inside the main class.
 */
export class State {

    // Comms
    public appSender: ((data: string) => void) | null = null;
    public appSenderPromise: Promise<void> | null = null;
    public appListeners: ((data: unknown) => void)[] = [];

    // RPC
    public allowedOrigins: string[] = [
        window.location.origin,
        "https://discord.com",
        "https://discordapp.com",
        "https://ptb.discord.com",
        "https://ptb.discordapp.com",
        "https://canary.discord.com",
        "https://canary.discordapp.com",
        "https://staging.discord.co",
        "http://localhost:3333",
        "https://pax.discord.com",
        "null"
    ];
    public initialized: boolean = false;
    public ready: boolean = false;
    public stateCode: StateCode = StateCode.Loading;
    public readyPromise: Promise<void> | null = null;
    public dispatchReady: (() => void) | null = null;
}