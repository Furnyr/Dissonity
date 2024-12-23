import CollapseButton from '../../components/CollapseButton';
import HashLink from '../../components/HashLink';
import { useOutletContext } from 'react-router-dom';
import { FaClipboardList, FaClipboardCheck  } from 'react-icons/fa';
import { useState } from 'react';

function DocsPage () {

  const context = useOutletContext() as { onClick: () => void, collapsed: boolean };

  const [isCopiedPackage, setIsCopiedPackage] = useState(false);
  const [isCopiedRedirect, setIsCopiedRedirect] = useState(false);

  return (
    <div className='doc-page'>
      <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

      <div className='warning-container'>
        <p className='inter'>
          WARNING!
          <br /><br />
          This documentation is for Dissonity Version 2, which is still in alpha, so it will break your code between updates!
          <br /><br />
          Use it for preview or wait for the full release. <a href='https://github.com/Furnyr/Dissonity/releases'>https://github.com/Furnyr/Dissonity/releases</a>
        </p>
      </div>

      <h1 id='start'>Getting Started <HashLink link='/docs/getting-started#start'/></h1>

      <p>
        In this section you will learn how to create your first Discord activity using Unity. You should have installed:
        <br />
        <ul>
          <li><a href='https://unity.com/download' target='_blank'>Unity</a></li>
          <li><a href='https://discord.com/download' target='_blank'>Discord PTB</a> (recommended)</li>
          <li><a href='https://nodejs.org/en' target='_blank'>Node.js</a> (only for this guide)</li>
          <li><a href='https://developers.cloudflare.com/cloudflare-one/connections/connect-networks/downloads/' target='_blank'>cloudflared</a> (only for this guide)</li>
        </ul>

        This article is equivalent to the official <a href='https://discord.com/developers/docs/activities/building-an-activity' target='_blank'>Quickstart guide</a>.
      </p>

      <br />

      <h2 id='enable-developer-mode'>0. Enable Developer Mode <HashLink link='/docs/getting-started#enable-developer-mode'/></h2>

      <p>
        Enabling Developer Mode in your Discord account will allow you to run in-development activities and expose resource IDs that will be helpful later. To enable Developer Mode:
        <ol>
          <li>Go to your User Settings in your Discord client</li>
          <li>Click on <b>Advanced</b> tab from the left-hand sidebar and toggle on <b>Developer Mode</b></li>
        </ol>
      </p>

      <br />

      <h2 id='create-a-unity-project'>1. Create a Unity project <HashLink link='/docs/getting-started#create-a-unity-project'/></h2>

      <p>
        Open Unity Hub and create a new project using v2021.3 or later.
      </p>

      <br />

      <h2 id='install-dissonity'>2. Install Dissonity <HashLink link='/docs/getting-started#install-dissonity'/></h2>

      <ol>
        <li>Open the package manager (Window &gt; Package Manager)</li>
        <li>Install package from git URL: <code>https://github.com/Furnyr/Dissonity.git?path=/unity#v2</code> <button style={{fontSize:'13px'}} onClick={() => {
          navigator.clipboard.writeText('https://github.com/Furnyr/Dissonity.git?path=/unity#v2');
          
          setIsCopiedPackage(true);

          setTimeout(() => setIsCopiedPackage(false), 2_000);
        }}>{isCopiedPackage ? <FaClipboardCheck /> : <FaClipboardList />}</button></li>
        <li>Set the build platform to Web / WebGL</li>
        <li>Player settings &gt; Resolution and Presentation &gt; Set the WebGL template to Dissonity</li>
      </ol>

      <p>
        Dissonity is now installed! We are almost done. Let's see what has changed in out project:
      </p>

      <ul>
        <li>A new folder was added (Assets/WebGLTemplates)</li>
        <li>A new file was added (Assets/DissonityUserConfiguration.cs)</li>
      </ul>

      <br />

      <h2 id='create-an-app'>3. Create an App <HashLink link='/docs/getting-started#create-an-app'/></h2>

      <p>
        Now that Dissonity is installed in our Unity project, let's create a Discord app that will hold the activity.
        <br />
      </p>
      <ol>
        <li>
          Create a new app in the <a href='https://discord.dev/applications' target='_blank'>developer portal</a> if you don't have one already. Then, scroll
          down to <b>Activities</b> and <b>Enable Activities</b> in the settings
        </li>
        <li>
          Choose <a href='https://discord.com/developers/docs/activities/building-an-activity#choose-installation-contexts' target='_blank'>installation context</a> (user, servers or both)
        </li>
        <li>
          Go to <b>OAuth2</b> and add a placeholder <b>Redirect</b>: <code>http://127.0.0.1</code> <button style={{fontSize:'13px'}} onClick={() => {
          navigator.clipboard.writeText('http://127.0.0.1');
          
          setIsCopiedRedirect(true);

          setTimeout(() => setIsCopiedRedirect(false), 2_000);
        }}>{isCopiedRedirect ? <FaClipboardCheck /> : <FaClipboardList />}</button>
        </li>
      </ol>

      <br />

      <h2 id='configure-dissonity'>4. Configure Dissonity <HashLink link='/docs/getting-started#configure-dissonity'/></h2>

      <p>
        Unlike your <code>BOT_TOKEN</code> or <code>CLIENT_SECRET</code>, the application ID (<code>CLIENT_ID</code>) is not sensitive data, which means that it can be publicly exposed without problem.
      </p>

      <ol>
        <li>Go to <b>OAuth2</b> and copy <b>Client ID</b></li>
        <li>In your Unity project, open Assets/DissonityUserConfiguration.cs</li>
        <li>Paste the application ID in <b>ClientId</b></li>
      </ol>

      Dissonity is now up and running! Create a new MonoBehaviour script and add: //todo
    </div>
  );

  /*
  
  Activities are just web pages inside iframes in the Discord client.
      Unity gives the option to build a game for WebGL, which allows it to run in the web. If all we wanted was to 
      embed a Unity game into Discord we wouldn't really need anything else.
  */
}

export default DocsPage