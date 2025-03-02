import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { DocsContext } from "../../../types";

function DocsPage () {

  const context = useOutletContext() as DocsContext;

  context.setActiveItem("/docs/v2/development/security");

  return (
    <div className="doc-page">
        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1 id="start">Security <HashLink link="/docs/v2/development/security#start"/></h1> 

        <p>
          Discord has a good security considerations guide linked at the bottom of this page. While working on your activities, it's important to understand which data you can trust and which you cannot.
        </p>

        <h2 id="rpc-protocol">RPC protocol <HashLink link="/docs/v2/development/security#rpc-protocol" /></h2>

        <p>
          The RPC protocol used to communicate with the Discord client isn't always a source of truth. It is possible to falsify messages as if they were sent by Discord.
        </p>

        <p>
          While it's okay to use RPC data locally, if you need data in a trusted manner, you must contact Discord API from your application's server. You can use the access token returned during authentication (accessible via <code>Api.AccessToken</code>).
        </p>

        <p>
          For instance:
        </p>

        <h3>❌ Do not ❌</h3>

        <ol>
          <li>Receive ENTITLEMENT_CREATE</li>
          <li>Send data to your server</li>
          <li>Grant the entitlement</li>
        </ol>

        <h3>✅ Do ✅</h3>

        <ol>
          <li>Receive ENTITLEMENT_CREATE</li>
          <li>Send data to your server</li>
          <li>Contact Discord API to get user entitlements</li>
          <li>Decide whether to grant the entitlement</li>
        </ol>

        <h2 id="hirpc-protocol">hiRPC protocol <HashLink link="/docs/v2/development/security#hirpc-protocol" /></h2>

        <p>
          Given the nature of client code, hiRPC cannot be considered a source of truth either. While there are mechanisms to prevent unauthorized access to hiRPC channels, it may be possible to intercept or falsify messages on any channel other than the <code>dissonity</code> channel.
        </p>

        <p>
          Overall, you can safely use data within the activity, but you should verify the data in your backend.
        </p>

        <h2>External links</h2>

        <ul>
          <li><a href="https://discord.com/developers/docs/activities/development-guides#security-considerations" target="_blank">Security considerations</a></li>
        </ul>

        <Footer />
    </div>
  );
}

export default DocsPage