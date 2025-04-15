import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../../types";
import CodeBlock from "../../../components/CodeBlock";
import PageTitle from "../../../components/PageTitle";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
        <PageTitle title="Utils | Dissonity"/>

        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1>Utils <HashLink link="/?/docs/v2/api/utils"/></h1> 

        <p>
          The term <b>Utils</b> generally refers to additional functionality offered by Dissonity and may not be related to the Embedded App SDK itself.
        </p>

        <p>
          Some utility additions are:
        </p>

        <ul>
          <li><a href="/ref/classDissonity_1_1Api.html#a5e23d445f7ea34f9bea6489e4a8c6977">Api.SyncedUser</a></li>
          <li><a href="/ref/classDissonity_1_1Api.html#a32df5733bd6c9a5446b9167a0006af4d">Api.SyncedGuildMemberRpc</a></li>
          <li><a href="/ref/classDissonity_1_1Api_1_1HiRpc.html">Api.HiRpc</a></li>
          <li><a href="/ref/classDissonity_1_1Api_1_1Proxy.html">Api.Proxy</a></li>
          <li><a href="/ref/classDissonity_1_1SdkConfiguration.html">SdkConfiguration</a></li>
          <li><code>&lt;User&gt;.DisplayName</code></li>
        </ul>

        <h2 id="class">Dissonity.Utils <HashLink link="/?/docs/v2/api/utils#class"/></h2>

        <p>
          The <code>Dissonity.Utils</code> static class includes miscellaneous methods that do not require initialization.
        </p>

        <hr className="separator"/>

        <h3 id="dissonity-log"><span className="method">method</span> DissonityLog <HashLink link="/?/docs/v2/api/utils#dissonity-log" /></h3>

        <p>
          Log a message adding [Dissonity] in the browser console.
        </p>

        <code className="doxygen">
          <a href="/ref/classDissonity_1_1Utils.html#a1bf6f0431f4460c71b0b89eb098768e9">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`static void DissonityLog(object message)`}
        </CodeBlock>


        <h3 id="dissonity-log-warning"><span className="method">method</span> DissonityLogWarning <HashLink link="/?/docs/v2/api/utils#dissonity-log-warning" /></h3>

        <p>
          Log a message adding [Dissonity] in the browser console.
        </p>

        <code className="doxygen">
          <a href="/ref/classDissonity_1_1Utils.html#ac95f245f8f14484e8227b403b0717782">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`static void DissonityLogWarning(object message)`}
        </CodeBlock>


        <h3 id="dissonity-log-error"><span className="method">method</span> DissonityLogError <HashLink link="/?/docs/v2/api/utils#dissonity-log-error" /></h3>

        <p>
          Log a message adding [Dissonity] in the browser console.
        </p>

        <code className="doxygen">
          <a href="/ref/classDissonity_1_1Utils.html#abc06d70b850b6812859263fa9145276d">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`static void DissonityLogError(object message)`}
        </CodeBlock>


        <h3 id="get-mock-snowflake"><span className="method">method</span> GetMockSnowflake <HashLink link="/?/docs/v2/api/utils#get-mock-snowflake" /></h3>

        <p>
          Get a snowflake to use as a placeholder.
        </p>

        <code className="doxygen">
          <a href="/ref/classDissonity_1_1Utils.html#aee95d249a5adf2da3133e3fa7faaecc8">Doxygen Reference</a>
        </code>

        <CodeBlock language="csharp">
          {`static long GetMockSnowflake()`}
        </CodeBlock>

        <Footer />
    </div>
  );
}

export default DocsPage