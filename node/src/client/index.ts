
import { initializeSdk } from './utils/initializeSdk';

import type { EventSchema } from '@discord/embedded-app-sdk/output/schema/events';
import type {CompatibleUser, MessageData} from './utils/types';
import type { DiscordSDK } from '@discord/embedded-app-sdk';

window.addEventListener('DOMContentLoaded', setupParentIframe);

//* Get the child iframe
function getChildIframe(): HTMLIFrameElement {

  const iframe = document.getElementById('child-iframe') as HTMLIFrameElement | null;

  if (iframe == null) {
    throw new Error('Child iframe not found');
  }

  return iframe;
}

export async function setupParentIframe() {

  //\ Promise initialization
  const loadPromise = initializeSdk();
  let discordSdk: DiscordSDK | null = null;
  let user: CompatibleUser | null = null;

  async function handleMessage({data: messageData}: MessageEvent<MessageData>) {

    // Bail out if messageData is not an "{}" object
    if (typeof messageData !== 'object' || Array.isArray(messageData) || messageData === null) {
      return;
    }

    //? Not loaded yet
    if (discordSdk == null || user == null) {
      await loadPromise;
    }

    const {nonce, event, command, data } = messageData;
    let { args } = messageData;

    // Sends a message to child iframe
    function handleSubscribeEvent(eventData: Record<string, unknown>) {

      getChildIframe().contentWindow?.postMessage(
        {
          event,
          command: 'DISPATCH',
          data: eventData,
        },
        '*'
      );
    }

    // Handle the command
    // You could add cases here to extend functionality
    switch (command) {

      case "SUBSCRIBE": {

        if (event == null) {
          throw new Error("SUBSCRIBE event is undefined");
        }

        //? Use channel id
        if (args == "channel_id") args = discordSdk!.channelId;

        discordSdk!.subscribe(event as keyof typeof EventSchema, handleSubscribeEvent, args as any); //! any
        break;
      }

      case "UNSUBSCRIBE": {

        if (event == null) {
          throw new Error("UNSUBSCRIBE event is undefined");
        }

        //? Use channel id
        if (args == "channel_id") args = discordSdk!.channelId;

        discordSdk!.unsubscribe(event as keyof typeof EventSchema, handleSubscribeEvent);
        break;
      }

      case "SET_ACTIVITY": {

        const reply = await discordSdk!.commands.setActivity(data as any);
        getChildIframe().contentWindow?.postMessage({nonce, event, command, data: reply}, "*");
        break;
      }

      case "GET_INSTANCE_ID": {

        const { instanceId } = discordSdk!;
        getChildIframe().contentWindow?.postMessage({nonce, command, data: instanceId, args}, "*");
        break;
      }

      case "GET_CHANNEL_ID": {

        const { channelId } = discordSdk!;
        getChildIframe().contentWindow?.postMessage({nonce, command, data: channelId, args}, "*");
        break;
      }

      case "GET_GUILD_ID": {

        const { guildId } = discordSdk!;
        getChildIframe().contentWindow?.postMessage({nonce, command, data: guildId, args}, "*");
        break;
      }

      case "GET_USER": {
        
        getChildIframe().contentWindow?.postMessage({nonce, command, data: user, args}, "*");
        break;
      }

      case "GET_INSTANCE_PARTICIPANTS": {

        const data = await discordSdk!.commands.getInstanceConnectedParticipants();
        getChildIframe().contentWindow?.postMessage({nonce, command, data, args}, "*");
        break;
      }
    }
  }
  
  //\ Setup message event handler
  window.addEventListener("message", handleMessage);

  //\ Load promise
  const promiseData = await loadPromise;
  discordSdk = promiseData.discordSdk;
  user = promiseData.user;
}