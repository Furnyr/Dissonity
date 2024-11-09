/**
 * Here you can add your Colyseus rooms
 * (https://docs.colyseus.io/server/room/)
 */

import { Room } from "colyseus";
import { GameState, MatchmakingState, Player } from "./structures";

import type { Client } from "colyseus";
import type { ExpectedCreateOptions, ExpectedJoinOptions } from "./types";

// Used to keep track of existing rooms. The keys are activity instance ids.
const roomsMap = new Map<string, boolean>();

// Users can connect to this room to know whether they have to create a new room
// or connect to an existing one. They'll connect, receive instructions and disconnect.
export class MatchmakingRoom extends Room {
    override maxClients: number = 1;

    private disconnectTimeout: NodeJS.Timeout | null = null;

    override onCreate(options: ExpectedCreateOptions): void | Promise<any> {
        
        //? Check validity
        if (typeof options.instanceId != "string") return this.disconnect();

        //\ Set state
        this.setState(new MatchmakingState());
    }

    override onJoin(client: Client, options: Required<ExpectedCreateOptions>): void | Promise<any> {

        // Client should receive the instructions and disconnect before 5s
        this.disconnectTimeout = setTimeout(() => { client.leave(); }, 5_000);
        
        //? Room exists
        const roomValue = roomsMap.get(options.instanceId);
        client.send("matchmake", {
            exists: roomValue ?? false,
        });
    }

    override onLeave(_client: Client, _consented?: boolean): void | Promise<any> {
        clearTimeout(this.disconnectTimeout as NodeJS.Timeout);
    }
}

// This is your actual game room!
export class GameRoom extends Room {

    override onCreate(options: ExpectedCreateOptions): void | Promise<any> {

        //? Check validity
        if (typeof options.instanceId != "string") return this.disconnect();

        roomsMap.set(options.instanceId, true);

        // Increasing the reservation time to increase flexibility with the client
        this.setSeatReservationTime(20);
        
        //\ Set id and state
        this.roomId = options.instanceId;
        this.setState(new GameState());


        // You can set up your listeners here
        // this.onMessage("someMessage", () => {});
    }

    override onJoin(client: Client, options?: ExpectedJoinOptions): void | Promise<any> {

        //? Check validity
        if (typeof options?.userId != "string") return client.leave();

        console.log(`Client joined to room with instance id: ${this.roomId}`);

        //\ Set user id to player
        const player = new Player();
        player.userId = options.userId;

        //\ Save player to state (for other clients to receive it)
        const state = this.state as GameState;
        state.players.set(client.sessionId, new Player());
    }

    override async onLeave(client: Client, consented?: boolean | undefined): Promise<any> {

        const state = this.state as GameState;
        
        // Mark player as disconnected
        state.players.get(client.sessionId)!.connected = false;

        try {
            if (consented) {
                throw new Error("Consented disconnect");
            }

            // Client has 5 seconds to reconnect
            await this.allowReconnection(client, 5);

            // Client's saved
            state.players.get(client.sessionId)!.connected = true;

        } catch (err) {

            console.log(`Client left room with instance id: ${this.roomId}`);

            // Client will be removed
            state.players.delete(client.sessionId);
        }
    }

    override onDispose(): void | Promise<any> {

        console.log(`Room with instance id ${this.roomId} disposed\n`);

        roomsMap.delete(this.roomId);
    }
}