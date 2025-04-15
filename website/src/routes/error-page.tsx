import { useRouteError, Link } from "react-router-dom";
import DoonyDark from "../assets/doony_dark.png";
import DoonyLight from "../assets/doony_light.png";
import Footer from "../components/Footer";
import "../styles/error-page.css";

export default function ErrorPage() {
  const error = useRouteError() as { statusText: string, message: string, status?: number };
  console.error(error);

  if (error?.status == 404) {

    const lightMode = window.matchMedia && window.matchMedia("(prefers-color-scheme: light)").matches;

    const image = lightMode
      ? DoonyLight
      : DoonyDark;

    return (
      <>
        <div id="error-page-wrapper-2">
        <div id="error-page-wrapper-1">

          <div id="error-page">
            <h1 id="status">404</h1>
            <p>We could not find what you were looking for.</p>
            <ul>
              <li><Link to={"/"}>Home Page</Link></li>
              <li><Link to={"/guides"}>Guides</Link></li>
              <li><Link to={"/docs"}>Documentation</Link></li>
            </ul>
          </div>
          <div id="error-page-img">
            <img src={image} width={400} alt="Confused mascot"/>
          </div>

        </div>
        <div id="error-page-footer-container">

        <Footer />
        </div>
        </div>
      </>
    );
  }

  return (
    <div id="error-page-wrapper-2">

    <div id="error-page-margin">
      <div id="error-page">
        <h1>Oops!</h1>
        <p>Sorry, an unexpected error has occurred.</p>
        <p>
          <i>{error?.statusText} -- {error?.message}</i>
        </p>

      </div>
    </div>
    <Footer />
    </div>
  );
}