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

        <h1>Exceptions <HashLink link="/?/docs/v2/api/exceptions"/></h1> 

        <h2 id="outside-discord-exception"><a href="/ref/classDissonity_1_1OutsideDiscordException.html">OutsideDiscordException</a> <HashLink link="/?/docs/v2/api/exceptions#outside-discord-exception"/></h2>

        <p>
          This exception is thrown by the <code>Api.Initialize</code> method when the application runs outside the Discord client.
        </p>

        <h3>Example use</h3>

        <CodeBlock language="csharp">{`using UnityEngine;
using Dissonity;
using static Dissonity.Api;

public class ExampleScript : MonoBehaviour
{
    async void Start()
    {
        try
        {
            await Initialize();
            Debug.Log("Within Discord! Api.Ready is true.");
        }
        catch (OutsideDiscordException e)
        {
            Debug.Log(e.Message);
            Debug.Log("Outside Discord! Api.Ready is false.");
        }
    }
}`}</CodeBlock>

        <h2 id="command-exception"><a href="/ref/classDissonity_1_1CommandException.html">CommandException</a> <HashLink link="/?/docs/v2/api/exceptions#command-exception"/></h2>

        <p>
          This exception is thrown by the <code>Api.Commands</code> and <code>Api.Subscribe</code> methods when the Discord client sends an error response for a command.
        </p>

        <h3>Example use</h3>

        <CodeBlock language="csharp">{`using UnityEngine;
using Dissonity;
using static Dissonity.Api;

public class ExampleScript : MonoBehaviour
{
    async void Start()
    {
        // (Assuming API is already initialized at this point.)
        try
        {
            var response = await Commands.InitiateImageUpload();
            Debug.Log($"Image uploaded: {response.ImageUrl}");
        }
        catch (CommandException e)
        {
            Debug.Log(e.Code);
            Debug.Log(e.Message);
            Debug.Log("Image upload canceled.");
        }
    }
}`}</CodeBlock>

        <Footer />
    </div>
  );
}

export default DocsPage