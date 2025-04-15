import CollapseButton from "../../components/CollapseButton";
import HashLink from "../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import Footer from "../../components/Footer";
import { PageContext } from "../../types";
import PageTitle from "../../components/PageTitle";
import { Link } from "react-router-dom";

function GuidesPage () {

  const context = useOutletContext() as PageContext;

  const mobile = window.matchMedia && window.matchMedia("(max-width: 600px)").matches;

  const buttonGuide = !mobile ? null :
    <p>
      Tip: Open the sidebar using the <code>Expand</code> button in the upper left corner.
    </p>;

  return (
    <div className="doc-page">
      <PageTitle title="Guides | Dissonity"/>

      <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

      <h1>Guides <HashLink link="/?/guides/v2/index"/></h1>

      <p>
        The guides section includes how to get started developing with Dissonity and basic information.
      </p>

      <ul>
        <li>For more specific information, see <Link to="/docs">Documentation</Link></li>
        <li>For details of the Unity API, see <a target="_blank" href="/ref/index.html">Reference</a></li>
      </ul>

      {buttonGuide}

      <Link to="/guides/v2/getting-started"><button>Start here!</button></Link>

      <Footer />

    </div>
  );
}

export default GuidesPage