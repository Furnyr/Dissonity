import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../../types";
import PageTitle from "../../../components/PageTitle";
import { Link } from "react-router-dom";
import Mermaid from "../../../components/Mermaid";
import BoxTip from "../../../components/BoxTip";
import CodeBlock from "../../../components/CodeBlock";
import BoxImportant from "../../../components/BoxImportant";
import CopyText from "../../../components/CopyText";
import { useState } from "react";
import { GITHUB_FILE_EXAMPLES_LINK } from "../../../constants";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  const [npmCopied, setNpmCopied] = useState(false);

  return (
    <div className="doc-page">
        <PageTitle title="hiRPC | Dissonity"/>

        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1>hiRPC <HashLink link="/?/docs/v2/internals/hirpc"/></h1> 

        <h2>Table of Contents</h2>

        <nav>
          <ul>
            <li><a href="#initial-terminology">Initial Terminology</a></li>
            <li><a href="#introduction">Introduction</a></li>
            <li><a href="#what-it-does">What it does</a></li>
            <li><a href="#how-it-works">How it works</a></li>
            <li><a href="#about-security">About security</a></li>
            <li><a href="#protocol">The hiRPC Protocol</a></li>
            <li><a href="#kit">JavaScript Kit</a></li>
            <li><a href="#final-terminology">Final Terminology</a></li>
          </ul>
        </nav>

        <br/>

        <hr className="separator"/>

        <h2 id="initial-terminology">Initial Terminology <HashLink link="/?/docs/v2/internals/hirpc#initial-terminology" /></h2>

        <ul>
          <li><b>Discord RPC</b>: Remote Procedure Call protocol used by an activity to interact with the Discord client.</li>
          <li><b>Application / Game build</b>: Executable build compiled by a game engine (e.g., Unity).</li>
          <li><b>hiRPC</b>: Module that implements the Discord RPC.</li>
          <li><b>Activity</b>: Website embedded inside an Iframe. Includes the game build and hiRPC.</li>
        </ul>

        <h2 id="introduction">Introduction <HashLink link="/?/docs/v2/internals/hirpc#introduction" /></h2>

        <p>
          hiRPC is the name given to the internal implementation of the <a href="https://discord.com/developers/docs/activities/how-activities-work#how-activities-work" target="_blank">Discord RPC protocol</a> used by Dissonity. Not only does it allow an application to interact with Discord, but it also allows <a href="#protocol">interoperation</a> with any non-JavaScript coded app. hiRPC takes the form of portable files that are served directly with the game build, without relying on external dependencies.
        </p>

        <h2 id="what-it-does">What it does <HashLink link="/?/docs/v2/internals/hirpc#what-it-does" /></h2>

        <p>
          In addition to its use to enable communication between all the activity components, hiRPC also solves important problems for activities made in game engines:
        </p>

        <ol>
          <li>It allows authentication to start before loading the game build.</li>
          <li>It helps safely expose APIs that Dissonity originally abstracted to JavaScript.</li>
          <li>It allows the use of any type of backend (no dependence on Node.js).</li>
          <li>It's less abstracted than the official SDK, so the API-like interface can be maintained at the C# layer (in the case of Dissonity).</li>
        </ol>

        <h2 id="how-it-works">How it works <HashLink link="/?/docs/v2/internals/hirpc#how-it-works" /></h2>

        <p>
          hiRPC functionality depends on an <b>instance</b> located in <code>window.dso_hirpc</code> since the beginning of the process. Any part of the activity can access this instance to send messages to Discord, to the app or to the JavaScript layer, if it has <b>hash access</b>.
        </p>

        <p>
          The hiRPC instance is created by an <b>app loader</b> that contains the implementation for the given game engine. This and the plugin that enables calling hiRPC methods from the game engine is what's called <b>hiRPC interface</b>.
        </p>

        <h2 id="about-security">About security <HashLink link="/?/docs/v2/internals/hirpc#about-security" /></h2>

        <BoxImportant title="hiRPC and RPC data">
          <p>
            For security concerns, you should read the <Link to="/docs/v2/development/security">"Security"</Link> page.
          </p>
        </BoxImportant>

        <p>
          Since all of these communications occur client-side, they can potentially be accessed by external users. You shouldn't trust sensitive information received via hiRPC. That being said, not all APIs exposed by the instance object are "public". Some are restricted by requiring a hash value that's used like a key to access methods. This hash can only be accessed under certain conditions:
        </p>

        <ol>
          <li>The communication channel with the application has not yet been opened.</li>
          <li>The maximum number of hash accesses has not been exceeded.</li>
          <li>Hash access hasn't been directly locked.</li>
        </ol>

        <h2 id="protocol">The hiRPC Protocol <HashLink link="/?/docs/v2/internals/hirpc#protocol" /></h2>

        <p>
          The communication processes performed through hiRPC are known as the <b>hiRPC protocol</b> and can be considered a superset of Discord's RPC in most cases. This includes communication between the hiRPC module and the application, which is known as <b>downward communication</b>.
        </p>

        <p>
          This process is very flexible and can occur in very different environments:
        </p>

        <ol>
          <li>Inside the Discord client without other JS components.</li>
          <li>Inside the Discord client with other JS components.</li>
          <li>Inside the Discord client, alongside an instance of the Embedded App SDK.</li>
          <li>Outside the Discord client without other JS components.</li>
          <li>Outside the Discord client with other JS components.</li>
        </ol>

        <BoxTip title="@dissonity/hirpc-kit">
          <p>
            To easily interact with the hiRPC module from the JS layer, you will normally use the hiRPC kit. It is explained later in this page.
          </p>
        </BoxTip>

        <h3 className="h3-margin" id="responding-to-the-environment">Responding to the environment <HashLink link="/?/docs/v2/internals/hirpc#responding-to-the-environment" /></h3>

        <p>
          The hiRPC module can recognize its environment via the query parameters and the presence of the <code>.proxy</code> path prefix. It is also designed to be able to work alongside the official SDK. If it detects the RPC <code>READY</code> event, it skips sending the RPC handshake.
        </p>

        <p>
          All these conditions and other internal data are tracked by <a href="https://developer.mozilla.org/en-US/docs/Web/API/Window/sessionStorage" target="_blank">sessionStorage</a>.
        </p>

        <h3 className="h3-margin" id="multi-event">Multi Event <HashLink link="/?/docs/v2/internals/hirpc#multi-event" /></h3>

        <p>
          The module securely stores the ready event, authorization response, authentication response and server response. This data is later sent to the application in a single payload known as <b>MultiEvent</b>, which is part of the <b>hiRPC handshake</b>.
        </p>

        <h3 className="h3-margin" id="hirpc-handshake">hiRPC Handshake <HashLink link="/?/docs/v2/internals/hirpc#hirpc-handshake" /></h3>

        <p>
          It is not the same as the RPC handshake. The former is sent when the Discord RPC connection is established. The latter is sent when the downward communication is opened. It includes:
        </p>

        <ul>
          <li>The multi event</li>
          <li>The app hash — an access hash with extra permissions, only sent in this scenario</li>
        </ul>

        <CodeBlock language="js">{`{
    "hirpc_state": 4,
    "hirpc_message": {
        "channel": "dissonity",
        "data": {
            "raw_multi_event": {
                "ready": "<serialized JSON>",
                "authorize": "<serialized JSON>",
                "authenticate": "<serialized JSON>",
                "response": "<serialized JSON>"
            },
            "hash": "<app-hash>"
        },
        "opening": true
    }
}`}</CodeBlock>

        <i>Example JSON representation of the hiRPC handshake.</i>

        <h3 className="h3-margin" id="diagram-inside-discord">hiRPC inside Discord — Flow Diagram <HashLink link="/?/docs/v2/internals/hirpc#diagram-inside-discord" /></h3>

        <p>
          This flow diagram explains the hiRPC protocol when the application is loaded by its index.html directly.
        </p>

        <ul>
          <li><code>hiRPC</code> represents both the module and the interface.</li>
          <li>The Application Iframe includes <code>hiRPC</code> and <code>Embedded-Application</code>.</li>
          <li><code>Discord-Client</code> has just mounted the Application Iframe.</li>
          <li>The hiRPC module has been mounted and loaded.</li>
        </ul>

        <Mermaid syntax={`sequenceDiagram
title Dissonity: hiRPC protocol inside Discord. Process without using the Kit.
participant A as Discord-Client
participant B1 as hiRPC
participant B2 as Embedded-Application
participant C as Discord API
participant D as Application-Server
alt RPC Authentication
B1->>B2: Begin loading game build
B1->>A: Initiate Handshake<br/>(No nonce included)
A->>B1: Open Socket
A->>C: Fetch application info
C->>A: Return application info
A->>B1: Ready Payload<br/>(No nonce included)
B1->>A: Request to authorize scopes<br/>by reading build variables.<br/>(This step opens the OAuth modal)
A->>B1: Reply with OAuth authorize code
B1->>D: Send OAuth code to application server
D->>C: Use OAuth code and client secret<br/>to fetch access_token from developer portal
C->>D: Reply with access_token
D->>B1: Reply with access_token
B1->>A: Authenticate with access_token
A->>B1: Validate authentication
end
alt Downward Communication
B2->>B1: Establish app sender function<br/>via hiRPC interface
B1->>B2: Send hiRPC handshake<br/>(multi event and app hash)
end
alt App Receives RPC Data Example
A->>B1: Data is dispatched to the Iframe
B1->>B2: Send rpc_message via app sender function
end`}/>

        <br/><br/>

        <hr className="separator"/>

        <h3 className="h3-margin" id="diagram-inside-discord-using-kit">hiRPC inside Discord, using JS Kit — Flow Diagram <HashLink link="/?/docs/v2/internals/hirpc#diagram-inside-discord-using-kit" /></h3>

        <p>
          This flow diagram explains the hiRPC protocol when the application is loaded by a JavaScript file, inside a nested Iframe.
        </p>

        <ul>
          <li><code>hiRPC</code> represents both the module and the interface.</li>
          <li><code>JavaScript-Layer</code> represents the JS code outside the nested Iframe.</li>
          <li>The main Application Iframe includes <code>hiRPC</code>, <code>Embedded-Application</code> and <code>JavaScript-Layer</code>.</li>
          <li><code>Discord-Client</code> has just mounted the Application Iframe.</li>
        </ul>

<Mermaid syntax={`sequenceDiagram
title Dissonity: hiRPC protocol inside Discord. Process using the Kit.
participant A as Discord-Client
participant B0 as JavaScript-Layer
participant B1 as hiRPC
participant B2 as Embedded-Application
participant C as Discord API
participant D as Application-Server
alt RPC Authentication
B0->>B1: Mounts hiRPC module
B0->>B0: (Optional) (Would skip the hiRPC RPC handshake)<br/>Create instance of the Embedded App SDK
B0->>B1: Load hiRPC module with x hash accesses
B0->>B1: (Optional) Request hash
B1->>B0: (Optional) Reply with access hash
B0->>B2: Load game build<br/>(will occur in parallel to RPC authentication)
B1->>A: Initiate Handshake<br/>(No nonce included)
A->>B1: Open Socket
A->>C: Fetch application info
C->>A: Return application info
A->>B1: Ready Payload<br/>(No nonce included)
B1->>A: Request to authorize scopes<br/>by reading build variables.<br/>(This step opens the OAuth modal)
A->>B1: Reply with OAuth authorize code
B1->>D: Send OAuth code to application server
D->>C: Use OAuth code and client secret<br/>to fetch access_token from developer portal
C->>D: Reply with access_token
D->>B1: Reply with access_token
B1->>A: Authenticate with access_token
A->>B1: Validate authentication
end
alt Downward Communication
B2->>B1: Establish app sender function<br/>via hiRPC interface
B1->>B2: Send hiRPC handshake<br/>(multi event and app hash)
end
alt Sending Message to JS Example
B2->>B1: Send message through "dog" channel
B1->>B1: Dispatch message if the request is valid<br/>(app hash)
B1->>B0: JavaScript components with hash access<br/>and subscribed to "dog" receive message
end
alt Sending Message to App Example
B0->>B1: Send message through "dog" channel
B1->>B1: Dispatch message if the request is valid<br/>(correct channel and access hash)
B1->>B2: Send message as a hirpc_message
end`}/>

        <h3 className="h3-margin" id="initializing-hirpc">Initializing hiRPC <HashLink link="/?/docs/v2/internals/hirpc#initializing-hirpc" /></h3>

        <p>
          In the same way that Dissonity uses it internally, you can access hiRPC from JavaScript code anywhere in your activity to send data to the game build.
        </p>

        <p>
          But before accessing hiRPC, you must indicate where the files are using an import map in your top index.html:
        </p>

        <CodeBlock language="xml">{`<html>
  <head>
    <script type="importmap">
      {
        "imports": {
          "dso_bridge/": "./Unity/Bridge/",
          "dso_proxy_bridge/": "./.proxy/Unity/Bridge/"
        }
      }
    </script>
    <script src="index.js"></script>
  </head>
</html>`}</CodeBlock>

        <i>Example import map, where Unity/Bridge contains dissonity_hirpc.js and other hiRPC files.</i>

        <p>
          Then, you need to check whether the module has been created. If not, you need to <b>mount</b> and <b>load</b> it.
        </p>

        <ul>
          <li><b>Mounting</b>: Sometimes known as deploying. The action of creating the hiRPC instance and saving it in <code>window.dso_hirpc</code>. This allows the module to listen to its environment.</li>
          <li><b>Loading</b>: Calling the <code>load</code> method to begin the hash access phase and start authentication.</li>
        </ul>

        <p>
          However, this process is slightly more complex, since you also need to clear hiRPC-related storage items, in case they persist from another load. To simplify this process, you can use the <b>JavaScript Kit</b>.
        </p>

        <h2 id="kit">JavaScript Kit <HashLink link="/?/docs/v2/internals/hirpc#kit" /></h2>

        <p>
          Now that you understand the basics of hiRPC, install the hiRPC Kit:
        </p>

        <CopyText text="npm install @dissonity/hirpc-kit" stateBool={npmCopied} stateFunction={setNpmCopied}/>

        <p>
          You can use <code>setupHiRpc</code> to mount and load hiRPC. You'll need to specify the hiRPC version you're using to provide the types.<br/>Using Dissonity, you can access the hiRPC version you're using in <code>{`<Build folder>/Bridge/version`}</code>.
        </p>
        
        <p>
          Then, use <code>loadIframe</code> to load the game build inside a nested Iframe.
        </p>

        <CodeBlock language="js">{`import { setupHiRpc, loadIframe } from "@dissonity/hirpc-kit";
import { version } from "./Unity/Bridge/version";

async function main () {

    // Create hiRPC instance (starts listening)
    const hiRpc = await setupHiRpc(version);

    // Load hiRPC with one hash access
    const authPromise = hiRpc.load(1);

    // Request hash. Keep it safe!
    const hash = (await hiRpc.requestHash())!;

    // (You can use hiRPC functionality from here)

    // Begin loading the Unity game
    loadIframe("Unity/index.html", "iframe-id");

    // Any RPC-related commands need to run after this promise resolution
    await authPromise;
}

main();`}</CodeBlock>

        <i>Example index.ts using @dissonity/hirpc-kit</i>

        <p>
          You can find further examples in the repository's <a href={GITHUB_FILE_EXAMPLES_LINK} target="_blank">examples folder</a>.
        </p>

        <h2 id="final-terminology">Final Terminology <HashLink link="/?/docs/v2/internals/hirpc#final-terminology" /></h2>

        <ul>
          <li><b>hiRPC</b>: Module that uses the Discord RPC to connect Discord, an application and the JavaScript layer.</li>
          <li><b>hiRPC Module</b>: Instance located in <code>window.dso_hirpc</code> used to access functionality.</li>
          <li><b>hiRPC Interface</b>: Scripts that contain the hiRPC implementation for a specific game engine.</li>
          <li><b>hiRPC Protocol</b>: Communication that occurs through the hiRPC module and <code>postMessage</code>.</li>
          <li><b>Access hash</b>: Key used to access restricted hiRPC functionality.</li>
          <li><b>App hash</b>: Key only given to the application, used to access restricted hiRPC functionality.</li>
        </ul>

        <Footer />
    </div>
  );
}

export default DocsPage