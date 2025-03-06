
import { DiscordSDK } from "@discord/embedded-app-sdk";


//# CONFIG - - - - -
export type ConfigOptions = {
  clientId: string;
  scope: ScopeArgument;
  tokenRoute: `/${string}`
};


//# DISCORD DATA - - - - -
export type DiscordSDKEvents = Parameters<DiscordSDK["subscribe"]>[0];
export type ScopeArgument = Parameters<DiscordSDK["commands"]["authorize"]>[0]["scope"];
export type AuthUser = Awaited<ReturnType<DiscordSDK["commands"]["authenticate"]>>["user"];

// Represents the commands sent to the parent iframe or the child iframe
export type MessageChildCommand = "LOADED" | "DISPATCH" | "GET_INSTANCE_ID" | "GET_APPLICATION_ID" | "SET_ACTIVITY" | "GET_CHANNEL_ID" | "GET_GUILD_ID" | "GET_USER" | "GET_USER_ID"
  | "GET_INSTANCE_PARTICIPANTS" | "HARDWARE_ACCELERATION" | "GET_CHANNEL" | "GET_CHANNEL_PERMISSIONS" | "GET_ENTITLEMENTS" | "GET_PLATFORM_BEHAVIORS"
  | "GET_SKUS" | "IMAGE_UPLOAD" | "EXTERNAL_LINK" | "INVITE_DIALOG" | "SHARE_MOMENT_DIALOG" | "SET_ORIENTATION_LOCK_STATE" | "START_PURCHASE" | "GET_LOCALE" | "SET_CONFIG";

export type MessageParentCommand = Exclude<MessageChildCommand, "DISPATCH" | "LOADED"> | "SUBSCRIBE" | "UNSUBSCRIBE" | "PING_LOAD";

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

export type DataPromise = Promise<{
  discordSdk: DiscordSDK,
  user: AuthUser | null
}>;