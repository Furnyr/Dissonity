import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../../types";
import BoxInfo from "../../../components/BoxInfo";
import { Link } from "react-router-dom";
import PageTitle from "../../../components/PageTitle";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
        <PageTitle title="Mocking | Dissonity"/>

        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1>Mocking <HashLink link="/?/docs/v2/internals/mocking"/></h1> 

        <p>
          Mocking is a feature that allows the execution of the Discord activity within Unity. It consists of:
        </p>

        <ul>
          <li><b>@DiscordMock</b>: Script with a custom UI that holds data to simulate the Discord client. </li>
          <li><b>@JavascriptMock</b>: Script with a custom UI that holds data to simulate interactions with JavaScript. </li>
          <li><b>API Mock Mode</b>: Mode where Dissonity interacts with the mocks instead of the real Discord client. It's automatically enabled in the Unity editor. </li>
        </ul>

        <h2 id="creating-a-mock-object">Creating a mock object <HashLink link="/?/docs/v2/internals/mocking#creating-a-mock-object" /></h2>

        <p>
          You can access the mock creation panel by right-clicking the hierarchy and selecting "Dissonity".
        </p>

        <p>
          The DiscordMock is automatically created if there isn't one already. The JavascriptMock needs to be created manually, but it's only required for non-Discord features (such as LocalStorage and HiRpc).
        </p>

        <BoxInfo title="Don't Destroy On Load">
          <p>
            Both mock objects and the <Link to="/guides/v2/how-does-it-work#dissonity-implementation">DissonityBridge</Link> are not destroyed on load. If there's another mock object in the next scene, it will be replaced by the first one.
          </p>
        </BoxInfo>

        <h2 id="mock-object-structure">Mock object structure <HashLink link="/?/docs/v2/internals/mocking#mock-object-structure" /></h2>

        <p>
          Mock objects are structured in multiple foldouts. Inside some foldouts there are buttons that can be used to dispatch events or add new data. When creating new structures, such as adding a mock player, Dissonity will assign them unique snowflake ids.
        </p>

        <p>
          Buttons across user interfaces are color-coded:
        </p>

        <ul>
          <li><b>Grey</b>: Do an action (e.g., dispatch an event).</li>
          <li><b>Light Blue</b>: Open a submenu.</li>
          <li><b>Dark grey</b>: Close a submenu.</li>
          <li><b>Red</b>: Do something dangerous (e.g., clear a list).</li>
        </ul>

        <h2 id="player-terminology">Player terminology <HashLink link="/?/docs/v2/internals/mocking#player-terminology" /></h2>

        <p>
          The term "player" is not used in the official Discord SDK; it's only used by Dissonity.
        </p>

        <p>
          Mocking a Discord "user" can also mean mocking its guild member or other structures assigned to it, hence, Dissonity refers to a "player" when it's the information related to a single individual.
        </p>

        <h2 id="building-with-a-mock">Building with a mock <HashLink link="/?/docs/v2/internals/mocking#building-with-a-mock" /></h2>

        <p>
          There's no problem with making a build with a mock object in the scene. Mocks are automatically destroyed when running outside of the Unity editor.
        </p>

        <Footer />
    </div>
  );
}

export default DocsPage