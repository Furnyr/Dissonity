import CollapseButton from "../../components/CollapseButton";
import Footer from "../../components/Footer";
import HashLink from "../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../types";
import "../../styles/why-dissonity.css";
import PageTitle from "../../components/PageTitle";

function GuidesPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
      <PageTitle title="Why Dissonity?"/>

      <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

      <h1>Why Dissonity? <HashLink link="/?/guides/v2/why-dissonity"/></h1>

      <p>
        There are several reasons to consider using Dissonity instead of the Embedded App SDK when using Unity:
      </p>

      <div className="why-list">
        <ul>
            <li>✨ <b>Simplicity</b>: No need to code your own bridge implementation. Dissonity handles RPC for you.</li>
            <li>🧪 <b>Mocking</b>: Test your game directly inside Unity.</li>
            <li>📏 <b>Flexibility</b>: Run your app from its index.html, inside a nested iframe, or even let users play outside Discord.</li>
            <li>⚒️ <b>Functionality</b>: Dissonity includes APIs beyond the official SDK, such as JS/C# interoperation and compatibility with different environments.</li>
            <li>🧱 <b>Designed for Unity</b>: The static API allows you to use Dissonity between Unity scenes. Updating is as easy as clicking a button!</li>
            <li>💖 <b>Open source</b>: Dissonity will always be open source and free.</li>
        </ul>
      </div>

      <h2 id="what-is-it">What is Dissonity exactly? <HashLink link="/?/guides/v2/why-dissonity#what-is-it"/></h2>

      <p>
        Dissonity is a Unity <b>SDK</b> that modifies the WebGL build to enable interaction in a Discord activity. Its modules mirror the official <a target="_blank" href="https://www.npmjs.com/package/@discord/embedded-app-sdk">Embedded App SDK</a>.
      </p>

      <Footer />
    </div>
  );
}

export default GuidesPage