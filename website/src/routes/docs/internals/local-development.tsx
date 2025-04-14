import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../../types";
import PageTitle from "../../../components/PageTitle";
import { GITHUB_LOCAL_AUTOMATION_LINK } from "../../../constants";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
        <PageTitle title="Local Development | Dissonity"/>

        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1>Local Development <HashLink link="/?/docs/v2/internals/local-development"/></h1> 

        <p>
          While working on the Unity package, it shouldn't be installed through the Package Manager. Instead, you can use the <a href={GITHUB_LOCAL_AUTOMATION_LINK}><code>local-automation</code></a> module.
        </p>

        <h2 id="the-local-automation-module">The local-automation module <HashLink link="/?/docs/v2/internals/local-development#the-local-automation-module" /></h2>

        <p>
          This module includes CLI commands that can be used to easily move files between a Unity project and the local repository folder.
        </p>

        <p>
          For more details, please see the <code>README.md</code> file in the local-automation folder of the repository.
        </p>

        <h2>External links</h2>
        
        <nav>
          <ul>
            <li><a href={GITHUB_LOCAL_AUTOMATION_LINK} target="_blank">Local Automation Module</a></li>
          </ul>
        </nav>

        <Footer />
    </div>
  );
}

export default DocsPage