//import Card from "../components/Card";
import "../styles/about.css"
import Footer from "../components/Footer";
import BoxWarn from "../components/BoxWarn";

function Team () {
  return (
    <>
    <div className="about-main">
    <div className="about-internal">

      <h1>About</h1>
      {/*
      
      <div className="card-container">

        <Card
          img={"https://placehold.co/1184x1149"}
          title="Nyrrren"
          link="https://github.com/Furnyr"
          subtitle="Core maintainer"
          alt={`Nyrrren's profile`}
        >
          <p>
            [Placeholder text]
          </p>
        </Card>

        <Card
          img={"https://placehold.co/1184x1149"}
          title="[]"
          subtitle="Main tester"
          alt={`[]'s profile`}
        >
          <p>
            [Placeholder text]
          </p>
        </Card>
      </div>
      //todo
      */}

      <BoxWarn title="WIP">
        <p>Work in progress!</p>
      </BoxWarn>

      <Footer />

    </div>
    </div>
    </>
  );
}

export default Team