import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../../types";
import CodeBlock from "../../../components/CodeBlock";
import { Link } from "react-router-dom";
import PageTitle from "../../../components/PageTitle";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
        <PageTitle title="Build Variables | Dissonity"/>

        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1>Build Variables <HashLink link="/?/docs/v2/internals/build-variables"/></h1>

        <p>
          The C# configuration is accessible on the JavaScript layer through the <b>Build Variables</b>.
        </p>

        <p>
          While making a WebGL build, Dissonity post-processes the hiRPC files to include the information found in the <a href="/ref/classDissonity_1_1SdkConfiguration.html">SdkConfiguration</a> class.
        </p>

        <p>
          This is used internally to begin authentication before loading the Unity game.
        </p>

        <h2 id="debugging">Debugging <HashLink link="/?/docs/v2/internals/build-variables#debugging" /></h2>

        <p>
          After making a build, you should see logs from Dissonity about the post-processing. If you don't see them, it could be because you're not using a compatible WebGL Template.
        </p>

        <CodeBlock language="diff">{`[Dissonity Build]: Now post-processing build, hold on...
[Dissonity Build]: Build post-processed correctly!`}</CodeBlock>

        <h2 id="example">Example <HashLink link="/?/docs/v2/internals/build-variables#example" /></h2>

        <h3 id="dissonity-configuration">Dissonity configuration <HashLink link="/?/docs/v2/internals/build-variables#dissonity-configuration" /></h3>
        <CodeBlock language="csharp">{`using Dissonity;
using Dissonity.Models;

[DissonityConfig]
public class DissonityConfiguration : SdkConfiguration<ServerTokenRequest, ServerTokenResponse>
{
    // Basic configuration
    // Hover over properties for more information!

    // Initialization
    public override long ClientId => 1234567898765432100;
    public override string[] OauthScopes => new string[] { OauthScope.Identify, /*, your-oauth-scopes*/ };
    public override string TokenRequestPath => "/api/token";

    // Resolution
    public override ScreenResolution DesktopResolution => ScreenResolution.Max;
    public override ScreenResolution MobileResolution => ScreenResolution.Max;
}`}</CodeBlock>

        <h3 id="javascript-access">JavaScript Access <HashLink link="/?/docs/v2/internals/build-variables#javascript-access" /></h3>
        <CodeBlock language="js">{`const variables = window.dso_hirpc.getBuildVariables();
console.log(variables.CLIENT_ID);`}</CodeBlock>
        <CodeBlock language="js">{`'1234567898765432100'`}</CodeBlock>

        <h2>Related pages</h2>

        <nav>
          <ul>
            <li><Link to="/docs/v2/development/authentication">Authentication</Link></li>
            <li><Link to="/docs/v2/internals/hirpc">hiRPC</Link></li>
          </ul>
        </nav>

        <Footer />
    </div>
  );
}

export default DocsPage