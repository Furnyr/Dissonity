import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../../types";
import CodeBlock from "../../../components/CodeBlock";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1 id="start">Debugging <HashLink link="/docs/v2/development/debugging#start"/></h1> 

        <p>
          While testing the activity within Discord, logging can be helpful to clearly see what is happening. To access the Discord console, install <a href="https://discord.com/download" target="_blank">Discord PTB</a> and use the key combination <code>Ctrl&nbsp;+&nbsp;Shift&nbsp;+&nbsp;I</code> (Windows).
        </p>

        <h2 id="utils">Utils <HashLink link="/docs/v2/development/debugging#utils" /></h2>

        <p>
          To easily filter your logs, you can use the <code>Utils.DissonityLog</code> method. It's like Unity's <code>Debug.Log</code>, but it adds <b>[Dissonity]</b> before the message in the Discord client.
        </p>

        <CodeBlock language="csharp">{`using UnityEngine;
using Dissonity;

public class ExampleScript : MonoBehaviour
{
    void Start()
    {
        Utils.DissonityLog("Log!");

        Utils.DissonityLogWarning("Warn!");

        Utils.DissonityLogError("Error!");
    }
}`}</CodeBlock>
        <Footer />
    </div>
  );
}

export default DocsPage