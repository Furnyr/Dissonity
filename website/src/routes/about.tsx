//import Card from "../components/Card";
import "../styles/about.css"
import Footer from "../components/Footer";
import HashLink from "../components/HashLink";
import PageTitle from "../components/PageTitle";
import Card from "../components/Card";
import { GITHUB_LINK } from "../constants";

import NyrrrenProfile from "../assets/nyrrren_profile.png";
import KistarProfile from "../assets/kistar_profile.png";

function Team () {
  return (
    <>
    <div className="about-main">
    <div className="about-internal">
      <PageTitle title="About | Dissonity"/>

      <h1>About <HashLink link="/?/about"/></h1>

      <p>
        Dissonity is a community-driven implementation of the <a target="_blank" href="https://github.com/discord/embedded-app-sdk">Discord Embedded App SDK</a> designed for seamless integration within Unity. Its formal development started in April 2024, around a month after the Activities Developer Preview began.
      </p>

      <p>
        It is possible thanks to the support of people like you! Consider contributing or giving us a ⭐ on <a target="_blank" href={GITHUB_LINK}>GitHub</a>.
      </p>

      <h2 id="acknowledgements">Acknowledgements <HashLink link="/?/about#acknowledgements"/></h2>

      <p>
        Dissonity is developed by <a href="https://github.com/Furnyr/Dissonity/graphs/contributors" target="_blank">our contributors</a> and the Dissonity Core Team. Though, many people have influenced the project without directly contributing code.
      </p>

      <h2 id="core-team">Core Team <HashLink link="/?/about#core-team"/></h2>

      <p>
        The Core Team consists of people who have been working on the project for a long time and make decisions that affect its direction.
      </p>
      
      <div className="card-container">

        <Card
          img={NyrrrenProfile}
          title="Nyrrren"
          link="https://github.com/Furnyr"
          subtitle="Lead Maintainer"
          alt={`Nyrrren's profile`}
        >
        </Card>

        <Card
          img={KistarProfile}
          title="Kistar"
          subtitle="Tester"
          alt={`Kistar's profile`}
        >
        </Card>
      </div>

      <Footer />

    </div>
    </div>
    </>
  );
}

export default Team