import BoxWarn from "../../../components/BoxWarn";
import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { DocsContext } from "../../../types";

function DocsPage () {

  const context = useOutletContext() as DocsContext;

  context.setActiveItem("/docs/v2/api/");

  return (
    <div className="doc-page">
        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1 id="start">Static API <HashLink link="/docs/v2/api#start"/></h1> 

        <BoxWarn title="WIP">
            <p>
                Article under construction! Come back soon!
            </p>
        </BoxWarn>

        <Footer />
    </div>
  );
}

export default DocsPage