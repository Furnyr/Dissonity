import { useRouteError, Link } from 'react-router-dom';
import DoonyDark from "./assets/doony_dark.png";
import DoonyLight from "./assets/doony_light.png";

export default function ErrorPage() {
  const error = useRouteError() as { statusText: string, message: string, status?: number };
  console.error(error);

  if (error?.status == 404) {

    let image = "";

    const lightMode = window.matchMedia && window.matchMedia('(prefers-color-scheme: light)').matches;
    if (lightMode) {
      image = DoonyLight;
    }

    else {
      image = DoonyDark;
    }

    return (
      <>
        <div id='error-page'>
          <h1 id="status">404</h1>
          <p>We couldn't find what you were looking for.</p>
          <ul>
            <li><Link to={'/'}>Home Page</Link></li>
            <li><Link to={'/docs'}>Documentation</Link></li>
          </ul>
        </div>
        <div id='error-page-img'>
          <img src={image} width={400}/>
        </div>
      </>
    );
  }

  return (
    <div id='error-page'>
      <h1>Oops!</h1>
      <p>Sorry, an unexpected error has occurred.</p>
      <p>
        <i>{error?.statusText} -- {error?.message}</i>
      </p>
    </div>
  );
}