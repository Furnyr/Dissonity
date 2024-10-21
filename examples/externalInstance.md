
# Using an external SDK instance

If you need access to the SDK instance, using the npm package v1.3.0+ you can pass a promise to the `useSdk` function, but you will need to handle authorization and authentication as documented here:

https://discord.com/developers/docs/activities/how-activities-work#sample-code-and-activity-lifecycle-diagram

---

`index.ts`

```ts
import { useSdk, DiscordSDK, DataPromise } from "dissonity";

window.addEventListener("DOMContentLoaded", async () => {

    const discordSdk = new DiscordSDK(/* Client id */);

    const promise: DataPromise = new Promise(async resolve => {
    
        await discordSdk.ready();
  
        // (Authorization process...)
  
        const { user } = await discordSdk.commands.authenticate({
            access_token: /* Retrieved access token */,
        });

        resolve({ discordSdk, user });
    });

    useSdk(promise);
});
```