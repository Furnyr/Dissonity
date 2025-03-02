import BoxWarn from "../../../components/BoxWarn";
import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../../types";
import "../../../styles/api.css";
import CodeBlock from "../../../components/CodeBlock";
import BoxInfo from "../../../components/BoxInfo";
import { Link } from "react-router-dom";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <BoxWarn title="Warning!">
          <p>
            This documentation is for Dissonity Version 2, which is still in beta. Keep in mind there might be breaking changes between updates!
          </p>
          <p>
            If you need stability, you should wait for the full release. <a href="https://github.com/Furnyr/Dissonity/releases" target="_blank">(Releases)</a>
          </p>
        </BoxWarn>

        <h1 id="start">API Class <HashLink link="/docs/v2/api#start"/></h1>

        <p>
          The static class <code>Dissonity.Api</code> provides most of the package functionality. You need to call and await <code>Dissonity.Api.Initialize</code> once per runtime before using the majority of its methods and properties.
        </p>

        <h2>Table of contents</h2>

        <ul>
          <li><a href="/docs/v2/api#properties">Properties</a></li>
          <li><a href="/docs/v2/api#commands">Commands</a></li>
          <li><a href="/docs/v2/api#proxy">Proxy</a></li>
          <li><a href="/docs/v2/api#subscribe">Subscribe</a></li>
          <li><a href="/docs/v2/api#hirpc">HiRpc</a></li>
          <li><a href="/docs/v2/api#local-storage">LocalStorage</a></li>
          <li><a href="/docs/v2/api#methods">Methods</a></li>
        </ul>

        <br/>


        <BoxInfo title="Doxygen Reference">
          <p>
            Some parts of the API are documented in detail in the generated <Link to="/doxygen/api">Doxygen reference</Link>. Do notice it's more verbose!
          </p>
        </BoxInfo>


        <h2 id="properties"> Properties <HashLink link="/docs/v2/api#properties" /></h2>

        <hr className="separator"/>

        <h3 id="proxy-domain"><span className="property">property</span> ProxyDomain <HashLink link="/docs/v2/api#proxy-domain" /></h3>

        <CodeBlock language="csharp">
          {`public const string ProxyDomain = "discordsays.com";`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="client-id"><span className="property">property</span> ClientId <HashLink link="/docs/v2/api#client-id" /></h3>

        <p>
          <code>❄️</code> Your app's client id.
        </p>

        <CodeBlock language="csharp">
          {`public static long ClientId { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="instance-id"><span className="property">property</span> InstanceId <HashLink link="/docs/v2/api#instance-id" /></h3>

        <p>
          Unique string id for each activity instance.
        </p>

        <CodeBlock language="csharp">
          {`public string InstanceId { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="platform"><span className="property">property</span> Platform <HashLink link="/docs/v2/api#platform" /></h3>

        <p>
          The platform on which the activity is running. It's a value of <code>Models.Platform</code>.
        </p>

        <CodeBlock language="csharp">
          {`public static string Platform { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="guild-id"><span className="property">property</span> GuildId <HashLink link="/docs/v2/api#guild-id" /></h3>

        <p>
          <code>❄️</code> The id of the guild on which the activity is running.
        </p>

        <CodeBlock language="csharp">
          {`public static long GuildId { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="channel-id"><span className="property">property</span> ChannelId <HashLink link="/docs/v2/api#channel-id" /></h3>

        <p>
          <code>❄️</code> The id of the channel on which the activity is running.
        </p>

        <CodeBlock language="csharp">
          {`public static long ChannelId { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="frame-id"><span className="property">property</span> FrameId <HashLink link="/docs/v2/api#frame-id" /></h3>

        <p>
          The activity frame id.
        </p>

        <CodeBlock language="csharp">
          {`public static string FrameId { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="mobile-app-version"><span className="property">property</span> MobileAppVersion <HashLink link="/docs/v2/api#mobile-app-version" /></h3>

        <p>
          The mobile client version. Returns null in desktop.
        </p>

        <CodeBlock language="csharp">
          {`public static string? MobileAppVersion { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="custom-id"><span className="property">property</span> CustomId <HashLink link="/docs/v2/api#custom-id" /></h3>

        <p>
          Query custom id. Used when a user joins the activity via a shared link.
        </p>

        <CodeBlock language="csharp">
          {`public static string CustomId { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="referrer-id"><span className="property">property</span> ReferrerId <HashLink link="/docs/v2/api#referrer-id" /></h3>

        <p>
          Query referrer id. Used when a user joins the activity via a shared link.
        </p>

        <CodeBlock language="csharp">
          {`public static string ReferrerId { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="location-id"><span className="property">property</span> LocationId <HashLink link="/docs/v2/api#location-id" /></h3>

        <p>
          Query location id.
        </p>

        <CodeBlock language="csharp">
          {`public static string LocationId { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="user-id"><span className="property">property</span> UserId <HashLink link="/docs/v2/api#user-id" /></h3>

        <p>
          <code>❄️</code> The current user id.
        </p>

        <CodeBlock language="csharp">
          {`public static long? UserId { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="access-token"><span className="property">property</span> AccessToken <HashLink link="/docs/v2/api#access-token" /></h3>

        <p>
          Your client access token.
        </p>

        <CodeBlock language="csharp">
          {`public static string AccessToken { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="initialized"><span className="property">property</span> Initialized <HashLink link="/docs/v2/api#initialized" /></h3>

        <p>
          True after the first <code> Api.Initialize </code> call, regardless of success.
        </p>

        <CodeBlock language="csharp">
          {`public static bool Initialized { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="ready"><span className="property">property</span> Ready <HashLink link="/docs/v2/api#ready" /></h3>

        <p>
          True if the <code> Api.Initialize </code> call was successful.
        </p>

        <CodeBlock language="csharp">
          {`public static bool Ready { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="synced-user"><span className="property">property</span> SyncedUser <HashLink link="/docs/v2/api#synced-user" /></h3>

        <p>
          If <code> SynchronizeUser </code> is enabled in the config, returns the current user object.
        </p>

        <CodeBlock language="csharp">
          {`public static User? SyncedUser { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="synced-guild-member-rpc"><span className="property">property</span> SyncedGuildMemberRpc <HashLink link="/docs/v2/api#synced-guild-member-rpc" /></h3>

        <p>
          If <code> SynchronizeGuildMemberRpc </code> is enabled in the config, returns the current guild member RPC object.
        </p>

        <CodeBlock language="csharp">
          {`public static GuildMemberRpc? SyncedGuildMemberRpc { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="configuration"><span className="property">property</span> Configuration <HashLink link="/docs/v2/api#configuration" /></h3>

        <CodeBlock language="csharp">
          {`public static ISdkConfiguration Configuration { get {...} }`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="is-mock"><span className="property">property</span> IsMock <HashLink link="/docs/v2/api#is-mock" /></h3>

        <CodeBlock language="csharp">
          {`public static bool IsMock => _mock;`}
        </CodeBlock>

        <Footer />
    </div>
  );
}

export default DocsPage