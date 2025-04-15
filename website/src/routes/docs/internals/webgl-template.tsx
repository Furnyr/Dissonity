import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../../types";
import PageTitle from "../../../components/PageTitle";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
        <PageTitle title="WebGL Template | Dissonity"/>

        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1>WebGL Template <HashLink link="/?/docs/v2/internals/webgl-template"/></h1> 

        <p>
          Unity doesn't allow packages to include custom WebGL templates natively. Hence, Dissonity uses a custom implementation to install the WebGL template in the developer's project.
        </p>

        <h2 id="implementation">Implementation <HashLink link="/?/docs/v2/internals/webgl-template#implementation" /></h2>

        <ol>
          <li>The code runs in a <b>OnPostprocessAllAssets</b> method from a child of the <b>AssetPostprocessor</b> class</li>
          <li>Checks if the folders to <code>Assets/Dissonity</code> exist to generate <code>Dialogs.asset</code>.</li>
          <li>Checks if the folders to <code>Assets/WebGLTemplates/Dissonity/Bridge</code> exist.</li>
          <li>If every folder existed and the version matches, the template is considered generated.</li>
          <li>If any folder didn't exist, or the template version is outdated, the template is added to the project.</li>
        </ol>

        <p>
          After adding the WebGL template, a log will be sent to the console to let the developer know that Dissonity added something to their project.
        </p>

        <Footer />
    </div>
  );
}

export default DocsPage