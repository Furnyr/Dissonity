import CollapseButton from "../../components/CollapseButton";
import Footer from "../../components/Footer";
import HashLink from "../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../types";
import PageTitle from "../../components/PageTitle";
import BoxWarn from "../../components/BoxWarn";
import { Link } from "react-router-dom";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  const mobile = window.matchMedia && window.matchMedia("(max-width: 600px)").matches;

  const buttonGuide = !mobile ? null :
    <p>
      Tip: Open the sidebar using the <code>Expand</code> button in the upper left corner.
    </p>;

  return (
    <div className="doc-page">
        <PageTitle title="Documentation | Dissonity"/>

        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <BoxWarn title="Warning!">
          <p>
            This documentation is for Dissonity Version 2, which is still in beta. Keep in mind there might be breaking changes between updates!
          </p>
          <p>
            If you need stability, you should wait for the full release. <a href="https://github.com/Furnyr/Dissonity/releases" target="_blank">(Releases)</a>
          </p>
        </BoxWarn>

        <h1>Documentation <HashLink link="/?/docs/v2/index"/></h1> 

        <p>
          The documentation section contains useful information for developing with Dissonity, overviews and explanations of internal implementations.
        </p>

        <ul>
          <li>For a step-by-step introduction, see <Link to="/guides">Guides</Link></li>
          <li>For details of the Unity API, see <a target="_blank" href="/ref/index.html">Reference</a></li>
        </ul>

        {buttonGuide}

        <Footer />
    </div>
  );
}

export default DocsPage