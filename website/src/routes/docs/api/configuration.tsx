import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../../types";
import CodeBlock from "../../../components/CodeBlock";
import { Link } from "react-router-dom";
import BoxWarn from "../../../components/BoxWarn";
import PageTitle from "../../../components/PageTitle";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
        <PageTitle title="Configuration | Dissonity"/>

        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1>Configuration <HashLink link="/?/docs/v2/api/config"/></h1> 

        <p>
          The project configuration is a class that's normally located in a single file. It is used to define the scopes your activity needs, where to send the <Link to="/docs/v2/development/authentication">authentication request</Link>, the screen resolution behaviour, and whether to enable other utilities.
        </p>

        <p>
          You can only have one configuration file per project, so it's recommended to generate it through the <a href="#dialogs"><b>Welcome&nbsp;Dialog</b></a>, rather than manually creating it.
        </p>

        <p>
          It must inherit from <code>SdkConfiguration</code> and use the <code>DissonityConfig</code> attribute. <br/>
          The <code>SdkConfiguration</code> class takes two type arguments:
        </p>

        <ul>
          <li>A <code>ServerTokenRequest</code>, that is sent to the server</li>
          <li>An expected <code>ServerTokenResponse</code></li>
        </ul>

        <p>You can simply pass <code>ServerTokenRequest</code> and <code>ServerTokenResponse</code> if you don't need to send or receive data during authentication.</p>

        <CodeBlock language="csharp">{`using Dissonity;
using Dissonity.Models;

[DissonityConfig]
public class DissonityConfiguration : SdkConfiguration<ServerTokenRequest, ServerTokenResponse>
{
    public override long ClientId => 0;
    public override string[] OauthScopes => new string[] { OauthScope.Identify };
    public override string TokenRequestPath => "/api/token";
}`}</CodeBlock>

        <i>Example configuration file.</i>

        <h2 id="dialogs">Dialogs <HashLink link="/?/docs/v2/api/config#dialogs"/></h2>

        <p>
          Dialogs are pop-up windows that appear when updating/installing Dissonity or that can be triggered through the <code>Assets/Dissonity/Dialogs.asset</code> file.
        </p>

        <h3 className="h3-margin">👋 Welcome Dialog 👋</h3>

        <p>
          This dialog normally appears after installing Dissonity in a Unity project. It has starting instructions and buttons that can be used to generate a configuration file in <code>Assets/Dissonity/DissonityConfiguration.cs</code>.
        </p>

        <h3 className="h3-margin">⬇️ Update Dialog ⬇️</h3>

        <p>
          It normally appears after updating Dissonity in a project where it's installed. It contains the changelog and important updates.
        </p>

        <h3 className="h3-margin">🗑️ Uninstaller Dialog 🗑️</h3>

        <p>
          This dialog can be triggered through the Dialogs file. You can use it to uninstall Dissonity and clean up the folders and files that were generated. It doesn't modify your code. If you were using Dissonity in your project you will have errors after executing the uninstaller.
        </p>

        <h2 id="screen-resolution">Screen resolution <HashLink link="/?/docs/v2/api/config#screen-resolution"/></h2>

        <p>
          There are three configuration properties used to control screen resolution and/or screen resizing behaviour, for three different platforms:
        </p>

        <ul>
          <li><code>DesktopResolution</code> (Discord desktop)</li>
          <li><code>MobileResolution</code> (Discord mobile)</li>
          <li><code>BrowserResolution</code> (Browser outside Discord)</li>
        </ul>

        <p>
          And four possible values:
        </p>

        <ul>
          <li><b>ScreenResolution.Default</b>: Uses the resolution in the Unity WebGL settings</li>
          <li><b>ScreenResolution.Viewport</b>: Uses the resolution from the Unity viewport</li>
          <li><b>ScreenResolution.Dynamic</b>*: Lets Unity handle the resolution dynamically</li>
          <li><b>ScreenResolution.Max</b>: Tries to use as much space as possible</li>
        </ul>

        <BoxWarn title="* Unity Dynamic Resolution">
          <p>
            You may have issues with the screen flashing black when using Dynamic resolution on desktop. The Max option is recommended in this case (already the default).
          </p>
        </BoxWarn>
        
        <h2 id="structure-synchronization">Structure synchronization <HashLink link="/?/docs/v2/api/config#structure-synchronization"/></h2>

        <p>
          Dissonity offers utility properties to access the current state of the User and GuildMember objects of the player. This is achieved by internally listening to RPC events.
        </p>
        
        <ul>
          <li><code>SynchronizeUser</code>: Allows accessing <code>Api.SyncedUser</code></li>
          <li><code>SynchronizeGuildMemberRpc</code>: Allows accessing <code>Api.SyncedGuildMemberRpc</code></li>
        </ul>

        <h2 id="logs">Logs <HashLink link="/?/docs/v2/api/config#logs"/></h2>

        <p>
          You can enable <a href="https://discord.com/developers/docs/activities/development-guides#disabling-logging" target="_blank">Console Log Override</a> setting <code>DisableConsoleLogOverride</code> to false. It is disabled by default, unlike the official SDK.
        </p>

        <p>
          Additionally, Dissonity prints information logs during the initialization phase. To disable them, set <code>DisableDissonityInfoLogs</code> to true.
        </p>

        <h2 id="url-mappings">URL Mappings <HashLink link="/?/docs/v2/api/config#url-mappings"/></h2>

        <p>
          If you need to patch static URL mappings, you should add them to the config file instead of calling <code>Api.PatchUrlMappings</code>. This allows Dissonity to patch them before starting the game.
        </p>

        <CodeBlock language="csharp">{`using Dissonity;
using Dissonity.Models;
using Dissonity.Models.Builders;

(...)

public override MappingBuilder[] Mappings => new MappingBuilder[] {
    new()
    {
        Prefix = "/foo",
        Target = "foo.com"
    }
};

public override PatchUrlMappingsConfigBuilder PatchUrlMappingsConfig => new()
{
    PatchFetch = true,
    PatchWebSocket = true,
    PatchXhr = true,
    PatchSrcAttributes = false
};`}</CodeBlock>

        <i>Example URL mappings configuration.</i>

        <h2>External links</h2>

        <ul>
          <li><a href="https://discord.com/developers/docs/activities/overview" target="_blank">Overview of Activities</a></li>
        </ul>

        <Footer />
    </div>
  );
}

export default DocsPage