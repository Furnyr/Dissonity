import CollapseButton from "../../components/CollapseButton";
import Footer from "../../components/Footer";
import HashLink from "../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../types";
import PageTitle from "../../components/PageTitle";
import CodeBlock from "../../components/CodeBlock";
import CopyText from "../../components/CopyText";
import { useState } from "react";
import { Link } from "react-router-dom";

function GuidesPage () {

  const context = useOutletContext() as PageContext;

  const [isCopiedPackage, setIsCopiedPackage] = useState(false);

  return (
    <div className="doc-page">
      <PageTitle title="Migrating to v2 | Dissonity Guides"/>

      <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

      <h1>Migrating to v2 <HashLink link="/?/guides/v2/migration-v2"/></h1>

      <h2 id="before-you-start">Before you start <HashLink link="/?/guides/v2/migration-v2#before-you-start"/></h2>

      <p>
        Make sure you are using Unity 2021.3 or later. Mobile activities are only supported on Unity 6 and later.
      </p>

      <ol>
        <li>Delete the <code>DiscordBridge</code> GameObject.</li>
        <li>Uninstall v1 throught the Package Manager.</li>
      </ol>

      <h2 id="installation">Installation <HashLink link="/?/guides/v2/migration-v2#installation"/></h2>

      <ol>
        <li>Open the package manager (Window &gt; Package Manager)</li>
        <li>Install package from git URL: <CopyText text="https://github.com/Furnyr/Dissonity.git?path=/unity#v2" stateFunction={setIsCopiedPackage} stateBool={isCopiedPackage}/></li>
      </ol>

      <p>
        You will likely see many errors in the console. You must fix them before continuing:
      </p>

      <h2 id="major-changes">Major Changes <HashLink link="/?/guides/v2/migration-v2#major-changes"/></h2>

      <h3 id="subscription-changes">Subscription changes <HashLink link="/?/guides/v2/migration-v2#subscription-changes"/></h3>
      
      <CodeBlock language="diff">{`- Api.SubActivityInstanceParticipantsUpdate(data => {});
+ Api.Subscribe.ActivityInstanceParticipantsUpdate(data => {});`}</CodeBlock>

      <h3 id="unsubscription-changes">Unsubscription changes <HashLink link="/?/guides/v2/migration-v2#unsubscription-changes"/></h3>
      
      <CodeBlock language="diff">{`- Api.UnsubActivityInstanceParticipantsUpdate(Listener);

+ Api.Unsubscribe<Participant[]>(Listener);

// Or

+ DiscordSubscription subscription = await Api.Subscribe.ActivityInstanceParticipantsUpdate(Listener);
+ Api.Unsubscribe(subscription);

// Or

+ Api.UnsubscribeFromEvent(DiscordEventType.ActivityInstanceParticipantsUpdate);

// Or

+ Api.ClearAllSubscriptions();`}</CodeBlock>

      <h3 id="snowflake-changes">Snowflake changes <HashLink link="/?/guides/v2/migration-v2#snowflake-changes"/></h3>

      <p>
        Snowflake values are now stored as longs:
      </p>

      <CodeBlock language="diff">{`- string clientId = await Api.GetApplicationId();
+ long clientId = Api.ClientId;`}</CodeBlock>

      <h3 id="value-changes">Value changes <HashLink link="/?/guides/v2/migration-v2#value-changes"/></h3>

      <p>
        Query parameters, IDs and other values are accessible through the API class directly:
      </p>
      
      <CodeBlock language="diff">{`- string userId = await Api.GetUserId();
+ var userId = Api.UserId;

- string instanceId = await Api.GetSDKInstanceId();
+ string instanceId = Api.InstanceId;`}</CodeBlock>

      <h3 id="command-changes">Command changes <HashLink link="/?/guides/v2/migration-v2#command-changes"/></h3>

      <ul>
        <li>Command names are not shortened.</li>
        <li>Return data is more intuitive.</li>
      </ul>

<CodeBlock language="diff">{`- InstanceParticipantsData data = await Api.GetInstanceParticipants();
+ Participant[] participants = await Api.Commands.GetInstanceConnectedParticipants();`}</CodeBlock>

      <h3 id="util-changes">Util changes <HashLink link="/?/guides/v2/migration-v2#util-changes"/></h3>

<CodeBlock language="diff">{`- Api.DissonityLog("Hello world!");
+ Utils.DissonityLog("Hello world!");`}</CodeBlock>

      <h3 id="command-error-changes">Command error changes <HashLink link="/?/guides/v2/migration-v2#command-error-changes"/></h3>

<CodeBlock language="diff">{`- ImageUploadData data = await Api.InitiateImageUpload();
- 
- if (image.canceled) {
-     return;
- }
- 
- Debug.Log(data.image_url);

+ try
+ {
+     InitiateImageUploadData data = await Api.Commands.InitiateImageUpload();
+     Debug.Log(data.ImageUrl);
+ }
+ catch (CommandException)
+ {
+     return;
+ }`}</CodeBlock>

      <h3 id="behaviour-changes">Behaviour changes <HashLink link="/?/guides/v2/migration-v2#behaviour-changes"/></h3>

      <ul>
        <li>Configuration is set in <code>Assets/Dissonity/DissonityConfiguration.cs</code> rather than via an npm package.</li>
        <li>You need to call and await Api.Initialize once per runtime before using most functionality.</li>
        <li>You don't need to manually create a DiscordBridge in your scene.</li>
        <li>Overrides have been replaced with an optional DiscordMock object.</li>
      </ul>

      <h2 id="configuration">Configuration <HashLink link="/?/guides/v2/migration-v2#configuration"/></h2>

      <p>
        After fixing all errors, a pop-up dialog should appear. If you don't see it, go to <code>Assets/Dissonity/Dialogs.asset</code> and select "Show Welcome Dialog". Follow the instructions that appear on the screen.
      </p>

      <p>
        When you get to that point, to get your previous activity working again, update your backend to implement <Link to="/docs/v2/development/authentication#server-requirements">v2 Authentication</Link>.
      </p>

      <p>
        You can also check out the <Link to="/guides/v2/getting-started">Getting Started</Link> guide.
      </p>

      <Footer />
    </div>
  );
}

export default GuidesPage