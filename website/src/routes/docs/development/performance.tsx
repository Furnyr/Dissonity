import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { DocsContext } from "../../../types";

function DocsPage () {

  const context = useOutletContext() as DocsContext;

  context.setActiveItem("/docs/v2/development/performance");

  return (
    <div className="doc-page">
        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1 id="start">Performance <HashLink link="/docs/v2/development/performance#start"/></h1> 

        <p>
          Performance is an important topic when using Unity to make a WebGL game. During the development of Dissonity, we concluded that it is possible to use Unity to create web applications with good performance, if performance considerations are kept in mind.
        </p>

        <p>
          There have been considerable improvements in Unity's web technologies since the release of Unity 6, which included support for mobile devices.
        </p>

        <p>
          Here are some advices that you should keep in mind:
        </p>

        <h2 id="build">Builds <HashLink link="/docs/v2/development/performance#build" /></h2>

        <p>
          Checking "Development Build" in the build settings will help make builds quicker, but the final product will load slower. Use it while testing, but make sure to disable it for production.
        </p>

        <p>
          For final products, you can experiment with the "Code Optimization" option.
        </p>

        <h2 id="resources">Resources <HashLink link="/docs/v2/development/performance#resources" /></h2>

        <p>
          Optimizing your game using common techniques is a good idea to use as few resources as possible. Consider techniques like object pooling, LOD (Level of Detail) in 3D projects, or unloading objects that the player can't see, among other optimization techniques.
        </p>

        <h2>External links</h2>

        <ul>
          <li><a href="https://discord.com/developers/docs/activities/design-patterns#technical-considerations" target="_blank">Discord Technical considerations</a></li>
          <li><a href="https://docs.unity3d.com/6000.0/Documentation/Manual/webgl-technical-overview.html" target="_blank">Unity WebGL Technical limitations</a></li>
        </ul>

        <Footer />
    </div>
  );
}

export default DocsPage