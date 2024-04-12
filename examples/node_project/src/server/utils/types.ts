
export type PossibleTypeof = "bigint" | "boolean" | "function" | "number" | "object" | "string" | "symbol" | "undefined";

/**
 * Partial types could be used to represent the data that the client should send, but there's
 * no guarantee that's what is received.
 */

export type ExpectedCreateOptions = Partial<{
    instanceId: string
}>;

export type ExpectedJoinOptions = Partial<{
    userId: string
}>;