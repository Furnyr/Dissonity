import CollapseButton from "../../components/CollapseButton";
import Footer from "../../components/Footer";
import HashLink from "../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../types";
import BoxWarn from "../../components/BoxWarn";

function GuidesPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
      <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

      <h1 id="start">Migrating to v2 <HashLink link="/guides/v2/migration-v2#start"/></h1>

        <BoxWarn title="WIP">
            <p>
                Article under construction! Come back soon!
            </p>
        </BoxWarn>

      <Footer />
    </div>
  );
}

export default GuidesPage