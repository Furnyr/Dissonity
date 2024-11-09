/**
 * You can add your Colyseus schemas in this file.
 * The Unity C# scripts will be generated inside _unity_colyseus
 * after running "npm run colyseus".
 */

import { Schema, type, MapSchema } from "@colyseus/schema";

// Example player schema, you could add more properties, a constructor...
export class Player extends Schema {

    @type("boolean")
    connected: boolean = true;

    @type("string")
    userId: string = "";
}

// Matchmaking doesn't require state
export class MatchmakingState extends Schema {}

// Example game state
export class GameState extends Schema {

    @type({ map: Player })
    players = new MapSchema<Player>();
}