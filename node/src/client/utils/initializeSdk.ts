
import { DiscordSDK } from "@discord/embedded-app-sdk";
import fetch from "cross-fetch";

import type { CompatibleUser } from "./types";

/**
 * This function should be called from the top-level of your embedded app
 * It initializes the SDK, then authorizes, and authenticates the embedded app
 */
export async function initializeSdk(): Promise<{ discordSdk: DiscordSDK, user: CompatibleUser }> {

  //? No PUBLIC_CLIENT_ID
  if (typeof process.env.PUBLIC_CLIENT_ID !== 'string') {
    throw new Error("Must specify 'PUBLIC_CLIENT_ID");
  }

  //\ Initialize
  const discordSdk = new DiscordSDK(process.env.PUBLIC_CLIENT_ID);
  await discordSdk.ready();

  
  // Pop open the OAuth permission modal and request for access to scopes listed in scope array below
  const { code } = await discordSdk.commands.authorize({
    client_id: process.env.PUBLIC_CLIENT_ID,
    response_type: 'code',
    state: '',
    prompt: 'none',
    scope: ['identify', 'rpc.activities.write', 'rpc.voice.read'], //!? guilds
  });

  //\ Retrieve access token from the embedded app's server
  const response = await fetch('/api/token', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      code
    }),
  });

  const { access_token } = await response.json();

  //\ Authenticate with Discord client (using the access token)
  const { user } = await discordSdk.commands.authenticate({
    access_token,
  });

  console.log("SDK is ready, client is authenticated");

  // Better compatibility with the SDKBridge
  (user as CompatibleUser).flags = user.public_flags;
  (user as CompatibleUser).bot = false;

  return { discordSdk, user: (user as CompatibleUser) };
}