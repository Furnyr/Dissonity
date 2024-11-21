import { useRouteError } from 'react-router-dom';

export default function ErrorPage() {
  const error = useRouteError() as { statusText: string, message: string, status?: number };
  console.error(error);

  if (error.status == 404) {
    return (
      <div id='error-page'>
        <h1>404</h1>
        <p>This isn't what you are looking for</p>
        <p>
          <i>{error.statusText} -- {error.message}</i>
        </p>
      </div>
    );
  }

  return (
    <div id='error-page'>
      <h1>Oops!</h1>
      <p>Sorry, an unexpected error has occurred.</p>
      <p>
        <i>{error.statusText} -- {error.message}</i>
      </p>
    </div>
  );
}