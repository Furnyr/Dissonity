import { Link } from "react-router-dom";
import CollapseButton from "../../components/CollapseButton";
import Footer from "../../components/Footer";
import HashLink from "../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../types";

function GuidesPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
      <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

      <h1 id="start">How does it work? <HashLink link="/guides/v2/how-does-it-work#start"/></h1>

      <h2 id="activities">The way activities work <HashLink link="/guides/v2/how-does-it-work#activities"/></h2>
      
      <p>
        Discord activities are applications that are executed inside the Discord client, and are available everywhere on Discord. 
        Under the hood, they are just <a target="_blank" href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/iframe">Iframes</a> that
        load a website through the <a target="_blank" href="https://discord.com/developers/docs/activities/development-guides#activity-proxy-considerations">Discord Proxy</a>.
      </p>

      <p>
        Unity supports building games using <a target="_blank" href="https://docs.unity3d.com/6000.0/Documentation/Manual/webgl.html">WebGL</a>, which allows them to run on the web. If
        all we wanted was to embed a Unity game inside Discord, we wouldn't really need anything else. But things change when we want <b>interaction</b>. Things
        like playing with other users, seeing your name and avatar, sharing moments easily or being able to join the same lobby when clicking "Join activity".
      </p>

      <h2 id="embedded-app-sdk">Embedded App SDK <HashLink link="/guides/v2/how-does-it-work#embedded-app-sdk"/></h2>

      <p>
        Discord provides an <a target="_blank" href="https://www.npmjs.com/package/@discord/embedded-app-sdk">SDK</a> to communicate with the client from an activity, but it is a <a target="_blank" href="https://nodejs.org/en">Node.js</a> package. This means that
        an activity that uses the official SDK needs to use a Node.js-based server, or at least depend on Node.js for the development process. Dissonity gives you the option not to use Node.js while simplifying the development process.
      </p>

      {/*<h2 id="why-use-dissonity">Why use Dissonity? <HashLink link="/guides/v2/how-does-it-work#why-use-dissonity" /></h2>

      <ul>
        <li>100% Coverage of the official SDK</li>
        <li>API Designed for Unity</li>
        <li>Easy testing</li>
        <li>Support for different backends</li>
        <li>JavaScript interop APIs</li>
      </ul>*/}

      <h2 id="dissonity-implementation">Dissonity implementation <HashLink link="/guides/v2/how-does-it-work#dissonity-implementation"/></h2>

      <p>
        Dissonity provides an integrated way to communicate with the Discord client — <Link to="/docs/v2/internals/hirpc">hiRPC</Link>. Everything needed to run the activity is included in your Unity project. You can use any backend you want, as long as it can handle <Link to="/docs/v2/development/authentication">authentication</Link>. The hiRPC module also allows the JavaScript layer to interact with the compiled game.
      </p>

      <p>
        Within Unity, Dissonity enables <b>Mock Mode</b>. You don't need to compile and host your game each time you need to test something, you can
        simulate the Discord client in the Unity editor.
      </p>

      {/*<p>
        However, the most useful feature for development is the testing environment, also known as <b>Mock Mode</b>. You don't need to compile and host your game each time you need to test something, you can
        simulate the Discord client in the editor. This significantly reduces development time.
      </p>*/}

      <p>
        Once you want to try your game inside Discord, you just need to build the project for WebGL, using the Dissonity WebGL template, and host it on your server.
      </p>

      <Footer />

    </div>
  );
}

export default GuidesPage