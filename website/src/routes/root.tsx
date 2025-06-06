import "../styles/root.css"
import HeaderImage from "../assets/dissonity_header.png";
import Button from "../components/Button";
import Footer from "../components/Footer";

function App () {
  return (
    <>
      <div className="main">

        {/* Header (brand) */}
        <header className="header">
          <img className="header-image" src={HeaderImage} alt="Dissonity logo" width="670" />
          <div className="inter">
            <h1 className="header-title">Integrate <span className="unity-text">Unity</span> games into <span className="discord-text">Discord</span> activities</h1>
          </div>
          <div>
            <p className="sub">
            Dissonity is a Unity SDK that allows you to develop, test and release Discord activities. It simplifies communication with Discord and offers a testing environment within Unity, allowing you to focus on creating your app.
            </p>
          </div>
        </header>

        {/* Buttons */}
        <div className="button-container">
          <Button text="Guides" goto={"guides"}/>
          <Button text="Docs" goto={"docs"}/>
          <Button text="Reference" link={"/ref/index.html"}/>
        </div>

        <Footer />
      </div>
    </>
  );
}

export default App