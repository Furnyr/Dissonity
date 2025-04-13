import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../../types";
import "../../../styles/api.css";
import BoxInfo from "../../../components/BoxInfo";
import { Link } from "react-router-dom";
import PageTitle from "../../../components/PageTitle";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  return (
    <>
    <div className="doc-page">
        <PageTitle title="API | Dissonity"/>

        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1>API Class <HashLink link="/?/docs/v2/api"/></h1>

        <p>
          The static class <a href="/ref/classDissonity_1_1Api.html"><code>Dissonity.Api</code></a> provides most of the package's functionality. You need to call and await <a href="/ref/classDissonity_1_1Api.html#a273fdd6c042921a3d722fa71ebb50092"><code>Dissonity.Api.Initialize</code></a> once per runtime before using the majority of its methods and properties.
        </p>

        <p>
          You can access the static API across Unity scenes.
        </p>

        <h2>Related pages</h2>

        <nav>
          <ul>
            <li><Link to="/docs/v2/internals/design">Design</Link></li>
            <li><Link to="/docs/v2/development/authentication">Authentication</Link></li>
          </ul>
        </nav>

        <br/>

        <BoxInfo title="SDK Reference">
          <p>
            The API is documented in detail in the <a target="_blank" href="/ref/index.html"> Doxygen reference</a>.
          </p>
        </BoxInfo>

        <Footer />
    </div>
    </>
  );
}

export default DocsPage