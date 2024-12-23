import CollapseButton from "../../components/CollapseButton";
import HashLink from "../../components/HashLink";
import { useOutletContext } from "react-router-dom";

function DocsPage () {

  const context = useOutletContext() as { onClick: () => void, collapsed: boolean };
  return (
    <div className='doc-page'>
      <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

      <h1 id="start">How does it work? <HashLink link="/docs/how-does-it-work#start"/></h1>
      
      <p>
        Discord activities are applications that are executed inside the Discord client, and are available everywhere on Discord. 
        Under the hood, they are just <a target='_blank' href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/iframe">Iframes</a> that
        load a website through the <a target='_blank' href="https://discord.com/developers/docs/activities/development-guides#activity-proxy-considerations">Discord Proxy</a>.

        <br /><br />

        Unity supports building games using <a target='_blank' href="https://docs.unity3d.com/6000.0/Documentation/Manual/webgl.html">WebGL</a>, which allows them to run on the web, so if all we wanted was to embed a Unity game inside Discord, we wouldn't really need anything else. But what makes activities fun is <b>interaction</b>. Things like playing with other users, seeing your name and avatar, and sharing moments easily.

        <br /><br />

        Discord provides an <a target='_blank' href="https://www.npmjs.com/package/@discord/embedded-app-sdk">SDK</a> to communicate with the client from an activity, but it is a <a target='_blank' href="https://nodejs.org/en">Node.js</a> package, meaning that
        an activity that uses the official SDK needs to use a Node.js-based server.


      </p>
    </div>
  );

  /*
  
  Activities are just web pages inside iframes in the Discord client.
      Unity gives the option to build a game for WebGL, which allows it to run in the web. If all we wanted was to 
      embed a Unity game into Discord we wouldn't really need anything else.
  */
}

export default DocsPage