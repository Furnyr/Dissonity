import CollapseButton from "../../components/CollapseButton";
import Footer from "../../components/Footer";
import HashLink from "../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../types";
import BoxWarn from "../../components/BoxWarn";
import PageTitle from "../../components/PageTitle";

function GuidesPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
      <PageTitle title="Migrating to v2 | Dissonity Guides"/>

      <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

      <h1>Migrating to v2 <HashLink link="/?/guides/v2/migration-v2"/></h1>

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