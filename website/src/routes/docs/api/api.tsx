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
          The static class <code>Dissonity.Api</code> provides most of the package's functionality. You need to call and await <a href="#initialize"><code>Dissonity.Api.Initialize</code></a> once per runtime before using the majority of its methods and properties.
        </p>

        <h2>Table of contents</h2>

        <nav>
          <ul>
            <li><a href="/docs/v2/api#properties">Properties</a></li>
            <li><a href="/docs/v2/api#commands">Commands</a></li>
            <li><a href="/docs/v2/api#proxy">Proxy</a></li>
            <li><a href="/docs/v2/api#subscribe">Subscribe</a></li>
            <li><a href="/docs/v2/api#hirpc">HiRpc</a></li>
            <li><a href="/docs/v2/api#local-storage">LocalStorage</a></li>
            <li><a href="/docs/v2/api#methods">Methods</a></li>
          </ul>
        </nav>

        <br/>


        <BoxInfo title="Doxygen Reference">
          <p>
            Some parts of the API are documented in detail in the generated <Link to="/doxygen/api">Doxygen reference</Link>. Note that it is more verbose!
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


        <h2 id="commands"><span className="class">class</span> Commands <HashLink link="/docs/v2/api#commands" /></h2>

        <hr className="separator"/>

        <h3 id="capture-log"><span className="method">method</span> CaptureLog <HashLink link="/docs/v2/api#capture-log" /></h3>

        <p>
          Forward logs to your own logger.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#aee332a137401d70c6d529e5879f66ad4">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task CaptureLog(string consoleLevel, string message)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="encourage-hardware-acceleration"><span className="method">method</span> EncourageHardwareAcceleration <HashLink link="/docs/v2/api#encourage-hardware-acceleration" /></h3>

        <p>
          Presents a modal dialog to allow enabling of hardware acceleration.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#ae96a8fcf051d554b6f0d43e1ebb0b5d2">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<EncourageHardwareAccelerationData> EncourageHardwareAcceleration()`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="get-channel"><span className="method">method</span> GetChannel <HashLink link="/docs/v2/api#get-channel" /></h3>

        <p>
          Returns information about the channel for a provided channel ID.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#a267e90de6dfebf49584b6f5009d3ec5c">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<ChannelRpc> GetChannel(long channelId)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="get-channel-permissions"><span className="method">method</span> GetChannelPermissions <HashLink link="/docs/v2/api#get-channel-permissions" /></h3>

        <p>
          Returns permissions for the current user in the currently connected channel.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#a49abcdcbb58b0e92d45653782642c540">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<GetChannelPermissionsData> GetChannelPermissions()`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="get-entitlements"><span className="method">method</span> GetEntitlements <HashLink link="/docs/v2/api#get-entitlements" /></h3>

        <p>
          Returns a list of entitlements for the current user.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#a23154ae634444b215bddbfe9fe2e8f54">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<Entitlement[]> GetEntitlements()`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="get-skus"><span className="method">method</span> GetSkus <HashLink link="/docs/v2/api#get-skus" /></h3>

        <p>
          Returns a list of SKU objects. SKUs without prices are automatically filtered out.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#a1e699f7c809eddbca57ddbb5f6273a9b">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<Sku[]> GetSkus()`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="start-purchase"><span className="method">method</span> StartPurchase <HashLink link="/docs/v2/api#start-purchase" /></h3>

        <p>
          Launches the purchase flow for a specific SKU ID.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#a6befe0a5110dd160099cab87ef8d39df">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<Entitlement[]> StartPurchase(long skuId)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="get-platform-behaviors"><span className="method">method</span> GetPlatformBehaviors <HashLink link="/docs/v2/api#get-platform-behaviors" /></h3>

        <p>
          Returns information about supported platform behaviors.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#a81811d7036e47ebaf9da13a5af50a75f">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<GetPlatformBehaviorsData> GetPlatformBehaviors()`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="open-external-link"><span className="method">method</span> OpenExternalLink <HashLink link="/docs/v2/api#open-external-link" /></h3>

        <p>
          Allows for opening an external link from within the Discord client.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#af64347e1bbfdcf09474c00b9eef30013">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<OpenExternalLinkData> OpenExternalLink(string url)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="open-invite-dialog"><span className="method">method</span> OpenInviteDialog <HashLink link="/docs/v2/api#open-invite-dialog" /></h3>

        <p>
          Presents a modal dialog with Channel Invite UI.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#af44c32993063036149a845ee9e271c5b">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task OpenInviteDialog()`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="open-share-moment-dialog"><span className="method">method</span> OpenShareMomentDialog <HashLink link="/docs/v2/api#open-share-moment-dialog" /></h3>

        <p>
          Presents a modal dialog to share media to a channel or direct message.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#aa5008100bb8433ef489365ef76b267bc">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task OpenShareMomentDialog(string mediaUrl)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="set-activity"><span className="method">method</span> SetActivity <HashLink link="/docs/v2/api#set-activity" /></h3>

        <p>
          Modifies how your activity's rich presence is displayed in the Discord client.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#a907f2306c56c29032e50f17978fa56d0">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<Activity> SetActivity(ActivityBuilder activity)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="set-config"><span className="method">method</span> SetConfig <HashLink link="/docs/v2/api#set-config" /></h3>

        <p>
          Set whether or not the PIP (picture-in-picture) is interactive.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#a09a3634840b387857389e354dd2f1790">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<SetConfigData> SetConfig(bool useInteractivePip)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="set-orientation-lock-state"><span className="method">method</span> SetOrientationLockState <HashLink link="/docs/v2/api#set-orientation-lock-state" /></h3>

        <p>
          Locks the application to specific orientations in each of the supported layout modes.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#aa00adfa3ac1d6e0a0fa7a039119d4445">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task SetOrientationLockState(OrientationLockStateType lockState, OrientationLockStateType? pipLockState = null, OrientationLockStateType? gridLockState = null)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="user-settings-get-locale"><span className="method">method</span> UserSettingsGetLocale <HashLink link="/docs/v2/api#user-settings-get-locale" /></h3>

        <p>
          Returns the current user's locale.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#a30012f79bc3658b5a200a0f13a56ffc6">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<UserSettingsGetLocaleData> UserSettingsGetLocale()`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="initiate-image-upload"><span className="method">method</span> InitiateImageUpload <HashLink link="/docs/v2/api#initiate-image-upload" /></h3>

        <p>
          Presents the file upload flow in the Discord client.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#af23f5b1f73193c09d183858923130bb2">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<InitiateImageUploadData> InitiateImageUpload()`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="get-instance-connected-participants"><span className="method">method</span> GetInstanceConnectedParticipants <HashLink link="/docs/v2/api#get-instance-connected-participants" /></h3>

        <p>
          Returns all participants connected to the instance.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#af31c1b90e98fccad7d7c5fcaa61c955a">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<Participant[]> GetInstanceConnectedParticipants()`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="share-link"><span className="method">method</span> ShareLink <HashLink link="/docs/v2/api#share-link" /></h3>

        <p>
          Presents a modal for the user to share a link to your activity with custom query params.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_commands.html#ad299068dd147ad2aa75bd8d2f8b4cbb3">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<ShareLinkData> ShareLink(string message, string? customId = null, string? referrerId = null)`}
        </CodeBlock>


        <h2 id="proxy"><span className="class">class</span> Proxy <HashLink link="/docs/v2/api#proxy" /></h2>

        <hr className="separator"/>

        <h3 id="https-post-request"><span className="method">method</span> HttpsPostRequest <HashLink link="/docs/v2/api#https-post-request" /></h3>

        <p>
          Sends an HTTPS post request with a JSON payload to the Discord proxy.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_proxy.html#ade6619a01c683eabcf68a700ad5ffe05">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static Task<TJsonResponse> HttpsPostRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload, Dictionary<string, string>? headers = null)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="https-get-request"><span className="method">method</span> HttpsGetRequest <HashLink link="/docs/v2/api#https-get-request" /></h3>

        <p>
          Sends an HTTPS get request to the Discord proxy.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_proxy.html#a5ca9aabaef76e47e96dd446c2a6d4c71">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static Task<TJsonResponse> HttpsGetRequest<TJsonResponse>(string path, Dictionary<string, string>? headers = null)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="https-patch-request"><span className="method">method</span> HttpsPatchRequest <HashLink link="/docs/v2/api#https-patch-request" /></h3>

        <p>
          Sends an HTTPS patch request with a JSON payload to the Discord proxy.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_proxy.html#a1a2fae2800d36675c3d5aeb692487937">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static Task<TJsonResponse> HttpsPatchRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload, Dictionary<string, string>? headers = null)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="https-put-request"><span className="method">method</span> HttpsPutRequest <HashLink link="/docs/v2/api#https-put-request" /></h3>

        <p>
          Sends an HTTPS put request with a JSON payload to the Discord proxy.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_proxy.html#a57754e086011bbf5e2f122042cdb14ab">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static Task<TJsonResponse> HttpsPutRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload, Dictionary<string, string>? headers = null)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="https-delete-request"><span className="method">method</span> HttpsDeleteRequest <HashLink link="/docs/v2/api#https-delete-request" /></h3>

        <p>
          Sends an HTTPS delete request to the Discord proxy.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_proxy.html#ab0032f5af12055968b0dbf4e68bdc130">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static Task<TJsonResponse> HttpsDeleteRequest<TJsonResponse>(string path, Dictionary<string, string>? headers = null)`}
        </CodeBlock>


        <h2 id="subscribe"><span className="class">class</span> Subscribe <HashLink link="/docs/v2/api#subscribe" /></h2>

        <hr className="separator"/>

        <h3 id="activity-instance-participants-update"><span className="method">method</span> ActivityInstanceParticipantsUpdate <HashLink link="/docs/v2/api#activity-instance-participants-update" /></h3>

        <p>
          Received when the number of instance participants changes.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_subscribe.html#ad245994d5950d3b6a5b35b85de5fa884">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<DiscordSubscription> ActivityInstanceParticipantsUpdate(Action<Participant[]> listener)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="activity-layout-mode-update"><span className="method">method</span> ActivityLayoutModeUpdate <HashLink link="/docs/v2/api#activity-layout-mode-update" /></h3>

        <p>
          Received when a user changes the layout mode in the Discord client.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_subscribe.html#aff26e54ef2dda8b3b160638e15ca0d6d">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<DiscordSubscription> ActivityLayoutModeUpdate(Action<LayoutModeType> listener)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="current-user-update"><span className="method">method</span> CurrentUserUpdate <HashLink link="/docs/v2/api#current-user-update" /></h3>

        <p>
          Received when the current user object changes.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_subscribe.html#af9691932eec98663da9289d95e8c01f6">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<DiscordSubscription> CurrentUserUpdate(Action<User> listener)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="current-guild-member-update"><span className="method">method</span> CurrentGuildMemberUpdate <HashLink link="/docs/v2/api#current-guild-member-update" /></h3>

        <p>
          Received when the current guild member object changes.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_subscribe.html#aed5d5d1a610a3ca4c66d8986ee0131b4">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<DiscordSubscription> CurrentGuildMemberUpdate(long guildId, Action<GuildMemberRpc> listener)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="entitlement-create"><span className="method">method</span> EntitlementCreate <HashLink link="/docs/v2/api#entitlement-create" /></h3>

        <p>
          Received when an entitlement is created for a SKU.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_subscribe.html#a70ae67243ef5ddbc7ede6dadef386471">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<DiscordSubscription> EntitlementCreate(Action<Entitlement> listener)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="error"><span className="method">method</span> Error <HashLink link="/docs/v2/api#error" /></h3>

        <p>
          Non-subscription event sent when there is an error, including command responses.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_subscribe.html#a468f0cd2e2b6d75a2cce13b5c31be666">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static DiscordSubscription Error(Action<ErrorEventData> listener)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="orientation-update"><span className="method">method</span> OrientationUpdate <HashLink link="/docs/v2/api#orientation-update" /></h3>

        <p>
          Received when screen orientation changes
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_subscribe.html#a1b7d280fb551110a8752afcb558a77cd">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<DiscordSubscription> OrientationUpdate(Action<OrientationType> listener)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="speaking-start"><span className="method">method</span> SpeakingStart <HashLink link="/docs/v2/api#speaking-start" /></h3>

        <p>
          Received when a user in a subscribed voice channel speaks.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_subscribe.html#a22d4bc068ca06d26f824cdcabc26b5a3">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<DiscordSubscription> SpeakingStart(long channelId, Action<SpeakingData> listener)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="speaking-stop"><span className="method">method</span> SpeakingStop <HashLink link="/docs/v2/api#speaking-stop" /></h3>

        <p>
          Received when a user in a subscribed voice channel stops speaking.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_subscribe.html#a3be3370243b41cc8c7832c807d85fc41">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<DiscordSubscription> SpeakingStop(long channelId, Action<SpeakingData> listener)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="thermal-state-update"><span className="method">method</span> ThermalStateUpdate <HashLink link="/docs/v2/api#thermal-state-update" /></h3>

        <p>
          Received when Android or iOS thermal states are surfaced to the Discord mobile app.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_subscribe.html#ab4e5966625903474dde5dfcf899d6ac2">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<DiscordSubscription> ThermalStateUpdate(Action<ThermalStateType> listener)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="voice-state-update"><span className="method">method</span> VoiceStateUpdate <HashLink link="/docs/v2/api#voice-state-update" /></h3>

        <p>
          Received when a user's voice state changes in a subscribed voice channel (mute, volume, etc).
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_subscribe.html#a2125c623fa23a70291ee15d63e7d2f8f">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<DiscordSubscription> VoiceStateUpdate(long channelId, Action<UserVoiceState> listener)`}
        </CodeBlock>


        <h2 id="hirpc"><span className="class">class</span> HiRpc <HashLink link="/docs/v2/api#hirpc" /></h2>

        <hr className="separator"/>

        <h3 id="hirpc-send"><span className="method">method</span> Send <HashLink link="/docs/v2/api#hirpc-send" /></h3>

        <p>
          Send a message to JavaScript through this hiRPC channel.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_hi_rpc.html#ae6190d23276e7431d9a041c4a45c510d">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static void Send(string hiRpcChannel, object payload)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="hirpc-subscribe"><span className="method">method</span> Subscribe <HashLink link="/docs/v2/api#hirpc-subscribe" /></h3>

        <p>
          Receive messages sent through this hiRPC channel from JavaScript.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_hi_rpc.html#a6f9eff6029ba00d05d23cf37d4c1256f">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static HiRpcSubscription Subscribe(string hiRpcChannel, Action<object> listener)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="hirpc-unsubscribe-1"><span className="method">method</span> Unsubscribe (1/2) <HashLink link="/docs/v2/api#hirpc-unsubscribe-1" /></h3>

        <p>
          Remove a hiRPC subscription via a HiRpcSubscription instance (returned by HiRpc.Subscribe).
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_hi_rpc.html#a5e2cd1463ded49f3dc5554245cd69dc3">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static void Unsubscribe(HiRpcSubscription reference)`}
        </CodeBlock>

        <h3 id="hirpc-unsubscribe-2"><span className="method">method</span> Unsubscribe (2/2) <HashLink link="/docs/v2/api#hirpc-unsubscribe-2" /></h3>

        <p>
          Remove a hiRPC subscription via a method and a channel.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_hi_rpc.html#a23df9c09c73ae5469da6d6026bb1e4a0">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static void Unsubscribe(string hiRpcChannel, Action<object> listener)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="hirpc-unsubscribe-from-channel"><span className="method">method</span> UnsubscribeFromChannel <HashLink link="/docs/v2/api#hirpc-unsubscribe-from-channel" /></h3>

        <p>
          Remove all subscriptions related to a single hiRPC channel.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_hi_rpc.html#abe631c1544c6d73101cbb5a15cb024f3">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static void UnsubscribeFromChannel(string hiRpcChannel)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="hirpc-clear-all-subscriptions"><span className="method">method</span> ClearAllSubscriptions <HashLink link="/docs/v2/api#hirpc-clear-all-subscriptions" /></h3>

        <p>
          Remove all subscriptions from every hiRPC channel.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_hi_rpc.html#af6f8ff9721ab29c135f8dfe02150516b">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static void ClearAllSubscriptions()`}
        </CodeBlock>


        <h2 id="local-storage"><span className="class">class</span> LocalStorage <HashLink link="/docs/v2/api#local-storage" /></h2>

        <hr className="separator"/>

        <h3 id="set-item"><span className="method">method</span> SetItem <HashLink link="/docs/v2/api#set-item" /></h3>

        <p>
          Add persistent data to the local storage.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_local_storage.html#a5d1dab69f101411d9bf6d882a4183263">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static void SetItem(string key, string value)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="get-item"><span className="method">method</span> GetItem <HashLink link="/docs/v2/api#get-item" /></h3>

        <p>
          Get data from the local storage.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_local_storage.html#a86965731ecccdfc70ef34cc634673882">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public async static Task<string?> GetItem(string key)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="clear-item"><span className="method">method</span> Clear <HashLink link="/docs/v2/api#clear-item" /></h3>

        <p>
          Clear local storage.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api_1_1_local_storage.html#a1068d075a79557fd870e2160cdc1a601">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static void Clear()`}
        </CodeBlock>


        <h2 id="methods"> Methods <HashLink link="/docs/v2/api#methods" /></h2>

        <hr className="separator"/>

        <h3 id="initialize"><span className="method">method</span> Initialize <HashLink link="/docs/v2/api#initialize" /></h3>

        <p>
          Initializes Dissonity. You must call and await this method once before doing anything else.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api.html#a273fdd6c042921a3d722fa71ebb50092">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static Task<MultiEvent> Initialize()`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="on-ready"><span className="method">method</span> OnReady <HashLink link="/docs/v2/api#on-ready" /></h3>

        <p>
          Use this method to wait for initialization to complete. <br/>
          Returns true if the game is executed inside Discord.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api.html#a79c124012633da539a0beae757699500">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static async Task<bool> OnReady()`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="patch-url-mappings"><span className="method">method</span> PatchUrlMappings <HashLink link="/docs/v2/api#patch-url-mappings" /></h3>

        <p>
          Use this method to easily access external resources. <br/>
          If you need to use it before initialization, consider using <code> Mappings </code> and <code> PatchUrlMappingsConfig </code> from the <code> DissonityConfig </code> instead.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api.html#a094fdd62d81f420d64cc802a09bbc2c3">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static Task PatchUrlMappings(MappingBuilder[] mappings, PatchUrlMappingsConfigBuilder? config = null)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="format-price"><span className="method">method</span> FormatPrice <HashLink link="/docs/v2/api#format-price" /></h3>

        <p>
          Use this method to easily render SKU prices. When called in the Unity Editor, uses a generic currency symbol <code>¤</code>.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api.html#aae39a0054dc26c4d19bf0938d01296e7">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static Task<string> FormatPrice(SkuPrice price, string locale = Locale.UsEnglish)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="unsubscribe-1"><span className="method">method</span> Unsubscribe (1/2) <HashLink link="/docs/v2/api#unsubscribe-1" /></h3>

        <p>
          Remove a subscription via a DiscordSubscription instance (returned by subscription methods).
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api.html#a200115efbf49cd10e5bed96db4b440e9">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static void Unsubscribe(DiscordSubscription reference)`}
        </CodeBlock>

        <h3 id="unsubscribe-2"><span className="method">method</span> Unsubscribe (2/2) <HashLink link="/docs/v2/api#unsubscribe-2" /></h3>

        <p>
          Remove a subscription via the event return data type and a method.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api.html#aaaad9dc8c0b9099be5623c5a793c223b">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static void Unsubscribe<T>(Action<T> listener)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="unsubscribe-from-event"><span className="method">method</span> UnsubscribeFromEvent <HashLink link="/docs/v2/api#unsubscribe-from-event" /></h3>

        <p>
          Remove all subscriptions related to a single event.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api.html#a35b7ccd9e110d2ba143e1b3258f69687">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static void UnsubscribeFromEvent(string eventString)`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="clear-all-subscriptions"><span className="method">method</span> ClearAllSubscriptions <HashLink link="/docs/v2/api#clear-all-subscriptions" /></h3>

        <p>
          Remove all subscriptions from every event.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api.html#a4f50def33afb522f7c7e0bef2c4e15de">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static void ClearAllSubscriptions()`}
        </CodeBlock>


        <hr className="separator"/>

        <h3 id="close"><span className="method">method</span> Close <HashLink link="/docs/v2/api#close" /></h3>

        <p>
          Close the app with a specified code and reason.
        </p>

        <code className="doxygen">
          <a href="/doxygen/class_dissonity_1_1_api.html#a4b5e36d16eb2a5d7898747e0e10dde85">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`public static void Close(RpcCloseCode code, string message = "")`}
        </CodeBlock>

        <Footer />
    </div>
  );
}

export default DocsPage