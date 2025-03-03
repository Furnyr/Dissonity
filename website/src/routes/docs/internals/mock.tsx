import BoxWarn from "../../../components/BoxWarn";
import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../../types";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1 id="start">Mock <HashLink link="/docs/v2/internals/mock#start"/></h1> 

        {/*<h2 id="subtitle">Subtitle <HashLink link="/docs/v2/internals/mock#subtitle" /></h2>*/}

        {/*
        //todo
        // */}
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