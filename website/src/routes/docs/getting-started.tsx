import CollapseButton from "../../components/CollapseButton";
import HashLink from "../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { useState } from "react";
import CopyText from "../../components/CopyText";
import CodeBlock from "../../components/CodeBlock";
import BoxInfo from "../../components/BoxInfo";
import BoxWarn from "../../components/BoxWarn";
import Footer from "../../components/Footer";

function DocsPage () {

  const context = useOutletContext() as { onClick: () => void, collapsed: boolean };

  const [isCopiedPackage, setIsCopiedPackage] = useState(false);
  const [isCopiedRedirect, setIsCopiedRedirect] = useState(false);

  return (
    <div className="doc-page">
      <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>
      
      <BoxWarn title="Warning!">
        <p>
          This documentation is for Dissonity Version 2, which is still in alpha, so it will break your code between updates!
        </p>
        <p>
          Use it for preview or wait for the full release. <a href="https://github.com/Furnyr/Dissonity/releases" target="_blank">(Releases)</a>
        </p>
      </BoxWarn>

      <h1 id="start">Getting Started <HashLink link="/docs/v2/getting-started#start"/></h1>

      <p>
        In this section you will learn how to create your first Discord activity using Unity. You should have installed:
        <ul>
          <li><a href="https://unity.com/download" target="_blank">Unity</a></li>
          <li><a href="https://discord.com/download" target="_blank">Discord PTB</a> (recommended)</li>
          <li><a href="https://nodejs.org/en" target="_blank">Node.js</a> (only for this guide)</li>
          <li><a href="https://developers.cloudflare.com/cloudflare-one/connections/connect-networks/downloads/" target="_blank">cloudflared</a> (only for this guide)</li>
        </ul>

        This article is equivalent to the official <a href="https://discord.com/developers/docs/activities/building-an-activity" target="_blank">Quickstart guide</a>.
      </p>

      <h2 id="enable-developer-mode">0. Enable Developer Mode <HashLink link="/docs/v2/getting-started#enable-developer-mode"/></h2>

      <p>
        Enabling Developer Mode in your Discord account will allow you to run in-development activities and expose resource IDs that will be helpful later. To enable Developer Mode:
        <ol>
          <li>Go to your User Settings in your Discord client</li>
          <li>Click on <b>Advanced</b> tab from the left-hand sidebar and toggle on <b>Developer Mode</b></li>
        </ol>
      </p>

      <h2 id="create-a-unity-project">1. Create a Unity project <HashLink link="/docs/v2/getting-started#create-a-unity-project"/></h2>

      <p>
        Open Unity Hub and create a new project using v2021.3 or later. If you want to support mobile devices, you must use v6000 or later.
      </p>

      <h2 id="install-dissonity">2. Install Dissonity <HashLink link="/docs/v2/getting-started#install-dissonity"/></h2>

      <ol>
        <li>Open the package manager (Window &gt; Package Manager)</li>
        <li>Install package from git URL: <CopyText text="https://github.com/Furnyr/Dissonity.git?path=/unity#v2" stateFunction={setIsCopiedPackage} stateBool={isCopiedPackage}/></li>
        <li>Set the build platform to Web / WebGL</li>
        <li>Player settings &gt; Resolution and Presentation &gt; Set the WebGL template to Dissonity</li>
      </ol>

      <p>
        Dissonity is now installed! We are almost done. Let's see what has changed in our project:
      </p>

      <ul>
        <li>A new folder was added (Assets/WebGLTemplates)</li>
        <li>A new file was added (Assets/DissonityUserConfiguration.cs)</li>
      </ul>

      <h2 id="create-an-app">3. Create an App <HashLink link="/docs/v2/getting-started#create-an-app"/></h2>

      <p>
        Now that Dissonity is installed in our Unity project, let's create a Discord app that will hold the activity.
      </p>
      <ol>
        <li>
          Create a new app in the <a href="https://discord.com/developers/applications" target="_blank">developer portal</a> if you don't have one already. Then, scroll
          down to <b>Activities</b> and <b>Enable Activities</b> in the settings
        </li>
        <li>
          Choose <a href="https://discord.com/developers/docs/activities/building-an-activity#choose-installation-contexts" target="_blank">installation context</a> (user, servers or both)
        </li>
        <li>
          Go to <b>OAuth2</b> and add a placeholder <b>Redirect</b>: <CopyText text="http://127.0.0.1" stateFunction={setIsCopiedRedirect} stateBool={isCopiedRedirect}/>
        </li>
      </ol>

      <h2 id="configure-dissonity">4. Configure Dissonity <HashLink link="/docs/v2/getting-started#configure-dissonity"/></h2>

      <p>
        Unlike your <code>BOT_TOKEN</code> or <code>CLIENT_SECRET</code>, the application ID (<code>CLIENT_ID</code>) is not sensitive data, which means that it can be publicly exposed without problem.
      </p>

      <ol>
        <li>Go to <b>OAuth2</b> and copy <b>Client ID</b></li>
        <li>In your Unity project, open Assets/DissonityUserConfiguration.cs</li>
        <li>Paste the application ID in <b>ClientId</b></li>
      </ol>

      Dissonity is now up and running! Create a new MonoBehaviour script and add:

      <CodeBlock language="csharp">{`using UnityEngine;
using static Dissonity.Api;

public class MyScript : MonoBehaviour
{
    async void Start()
    {
        // Initialize Dissonity
        await Initialize();

        Debug.Log($"User with ID {UserId} is playing!");
    }
}`}</CodeBlock>

      <p>
        This code will print the ID of the user that is playing the activity. Notice that:
      </p>

      <ul>
        <li>You need to call and await <code>Dissonity.Api.Initialize</code> before anything else</li>
        <li>To use await, you need to convert Start to an async method</li>
      </ul>

      <BoxInfo title="Note">
        <p>
          You can only call <code>Api.Initialize</code> once per runtime. To ensure that the API is initialized at the same time across multiple scripts, use <code>await Api.OnReady();</code>
        </p>
      </BoxInfo>

      <p>
        Now add the script to a GameObject in your scene and press play. You should see a placeholder value printed on the console.
        <br />
        Congratulations, you are using <b>API Mock Mode</b>!
      </p>

      <h2 id="api-mock-mode">5. API Mock Mode <HashLink link="/docs/v2/getting-started#api-mock-mode"/></h2>

      <p>
        Dissonity simulates the Discord client inside Unity so you don't need to build the game each time you want to test it. This is called <b>mocking</b>.
      </p>
      <p>
        By right clicking the hierarchy and selecting (Dissonity &gt; Discord Mock) you can manually create a mock object. Use this object to establish your testing environment as if it was a Discord server.
      </p>

      <h2 id="build-the-game">6. Build the game <HashLink link="/docs/v2/getting-started#build-the-game"/></h2>

      <p>
        Add some changes to the game as you want, then open the build settings and check <b>Development build</b>. This will make building the game much faster, but the final product will be heavier, so make sure to disable it when making a final build!
      </p>
      <p>
        After some time, the activity files will be added to the folder you selected. And that's where Dissonity's job ends. Dissonity helps in the game-making process, but you need to host the activity yourself. This means that you need a server that:
      </p>

      <ol>
        <li>Will send the activity files when requested</li>
        <li>Can process JSON requests for authentication</li>
      </ol>

      <p>
        For the simplicity of this guide, we will use an already configured Node.js server, but you can use anything that fits.
      </p>

      <h2 id="build-the-game">7. Preparing the server <HashLink link="/docs/v2/getting-started#build-the-game"/></h2>

      {/*
        //todo
      */}
      <BoxWarn title="WIP">
        <p>
          This last part will be written in the future since I still need to finish a few things.
        </p>
      </BoxWarn>

      <h2 id="opening-an-http-tunnel">8. Opening an HTTP tunnel for development <HashLink link="/docs/v2/getting-started#opening-an-http-tunnel"/></h2>

      <h2 id="next-steps">9. Done! What's next? <HashLink link="/docs/v2/getting-started#next-steps"/></h2>

      <Footer />

    </div>
  );
}

export default DocsPage