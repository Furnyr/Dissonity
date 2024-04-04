
import type { DiscordSDK } from "@discord/embedded-app-sdk";


export type DiscordSDKEvents = Parameters<InstanceType<typeof DiscordSDK>["subscribe"]>[0];

// You can extend these commands as you need
export type MessageChildCommand = "DISPATCH" | "GET_INSTANCE_ID" | "SET_ACTIVITY" | "GET_CHANNEL_ID" | "GET_GUILD_ID" | "GET_USER"
  | "GET_INSTANCE_PARTICIPANTS";
export type MessageParentCommand = "SUBSCRIBE" | "UNSUBSCRIBE" | "GET_INSTANCE_ID" | "SET_ACTIVITY" | "GET_CHANNEL_ID" | "GET_GUILD_ID" | "GET_USER" |
  "GET_INSTANCE_PARTICIPANTS";

export interface MessageData {
  nonce?: string;
  event?: DiscordSDKEvents;
  command?: MessageChildCommand | MessageParentCommand;
  data?: any;
  args?: any;
}

// This type represents the properties a user object should have.
// It's necessary since the name for certain data is inconsistent,
// e.g. in the CURRENT_USER_UPDATE event it's "flags" while after authenticate() it's "public_flags"
export interface CompatibleUser {
  username: string;
  discriminator: string;
  id: string;
  public_flags: number;
  avatar?: string | null | undefined;
  global_name?: string | null | undefined;

  flags: number;
  bot: boolean;
}