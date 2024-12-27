import BoxWarn from "../../../components/BoxWarn";
import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";

function DocsPage () {

  const context = useOutletContext() as { onClick: () => void, collapsed: boolean };
  return (
    <div className="doc-page">
        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1 id="start">Build Variables <HashLink link="/docs/v2/internals/build-variables#start"/></h1> 

        <h2 id="subtitle">Subtitle <HashLink link="/docs/v2/internals/build-variables#subtitle" /></h2>

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