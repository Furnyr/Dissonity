import { Link } from "react-router-dom";
import CollapseButton from "../../components/CollapseButton";
import Footer from "../../components/Footer";
import HashLink from "../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../types";
import PageTitle from "../../components/PageTitle";
import CodeBlock from "../../components/CodeBlock";

function GuidesPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
      <PageTitle title="Third-party support | Dissonity Guides"/>

      <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

      <h1>Third-party support <HashLink link="/?/guides/v2/third-party-support"/></h1>

      <h2 id="introduction">Introduction <HashLink link="/?/guides/v2/third-party-support#introduction"/></h2>

      <p>
        Dissonity can work in parallel with other third-party libraries that are using the Embedded App SDK. When Dissonity detects that another library or SDK is trying to perform authentication, it lets them handle it, while listening and storing the necessary information.
      </p>

      <p>
        For this compatibility process to work, it's needed that:
      </p>

      <ol>
        <li>The hiRPC module is <Link to="/docs/v2/internals/hirpc#initializing-hirpc">mounted</Link> before the third-party software initializes</li>
        <li>The hiRPC module is <Link to="/docs/v2/internals/hirpc#initializing-hirpc">loaded</Link> after the third-party software initializes</li>
      </ol>

      <p>
        This way, Dissonity can listen to what the other library is doing and can finish the process seamlessly. The third-party library can send the RPC handshake, authorize, authenticate, all of the above or nothing. Dissonity will resume the process where it left off. Once the hiRPC module is loaded, you should be able to use both Dissonity and the third-party software without major problems.
      </p>

      <h2 id="js-instructions">Instructions for a JS library <HashLink link="/?/guides/v2/third-party-support#js-instructions"/></h2>

      <p>
        You will need to use <a href="https://www.npmjs.com/package/@dissonity/hirpc-kit" target="_blank">@dissonity/hirpc-kit</a> to configure a JavaScript library.
      </p>

      <ol>
        <li>Install @dissonity/hirpc-kit in your client</li>
        <li>Move your Unity build to a folder (for example, <code>/Unity</code>)</li>
        <li>Add an <Link to="/docs/v2/internals/hirpc#initializing-hirpc">import map</Link> referencing the hiRPC files in the build to your index.html</li>
        <li>Add an index.js (or index.ts if you're using TypeScript) where you'll configure both the third-party library and Dissonity:</li>
      </ol>

      <CodeBlock language="js">{`import { setupHiRpc, loadIframe } from "@dissonity/hirpc-kit";
import { version } from "./Unity/Bridge/version";

async function main () {

    // Create hiRPC instance (starts listening)
    const hiRpc = await setupHiRpc(version);

    /*
      Run and await if needed the third-party initialization HERE!
      Some examples are:

      - Embedded App SDK
      await discordSdk.ready();

      - PlayroomKit
      await insertCoin({
        discord: true
      });
    */

    // Load hiRPC with necessary hash accesses
    const authPromise = hiRpc.load(0);

    // Begin loading the Unity game
    loadIframe("Unity/index.html", "dissonity-child");

    // You can safely use Discord functionality after this promise resolution.
    await authPromise;
}

main();`}</CodeBlock>

      <i>Compatibility example using @dissonity/hirpc-kit</i>

      <h2 id="cs-instructions">Instructions for a C# library <HashLink link="/?/guides/v2/third-party-support#cs-instructions"/></h2>

      <p>
        If the third-party library does authentication through C# code, you will likely need <Link to="/docs/v2/api/config#lazy-hirpc-load">Lazy hiRPC loading</Link>. This disables authenticating before loading the game, so the third-party does the auth.
      </p>

      <ol>
        <li>Enable <code>LazyHiRpcLoad</code> in the Dissonity config file. The option is included in the advanced configuration</li>
        <li>Run the third-party initialization</li>
        <li>Call Dissonity's <code>Api.Initialize</code> after the third-party initialization</li>
      </ol>

      <CodeBlock language="cs">{`void Start()
{
    _playroomKit.InsertCoin(new InitOptions()
    {
        discord = true
    }, () =>
    {
        // Initialize in the third-party callback
        Dissonity.Api.Initialize();
    });
}`}</CodeBlock>

      <i>Initialization example for the Playroom Unity SDK (Dissonity is not affiliated with Playroom)</i>

      <p>
        Since you can only call <code>Api.Initialize</code> once, you can use <code>Api.OnReady</code> to wait for Dissonity initialization in other parts of your code.
      </p>

      <Footer />

    </div>
  );
}

export default GuidesPage