import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import {
  createBrowserRouter,
  Navigate,
  RouterProvider
} from 'react-router-dom';
import './index.css'
import Root from './routes/root.tsx'
import Docs from './routes/docs.tsx'
import GettingStarted from './routes/docs/getting-started.tsx'
import HowDoesItWork from './routes/docs/how-does-it-work.tsx'
import ErrorPage from './error-page.tsx'

//todo routes
/*
  /docs/*
  /team
  /about
  /support
  /contact
  /community?
  /docs/backend
*/
const router = createBrowserRouter([
  {
    path: '/',
    element: <Root />,
    errorElement: <ErrorPage />
  },
  {
    path: '/error-test',
    element: <ErrorPage />
  },
  {
    path: '/docs',
    element: <Docs />,
    children: [
      {
        path: "/docs",
        element: <Navigate to="/docs/getting-started" replace />
      },
      {
        path: "/docs/getting-started",
        element: <GettingStarted />
      },
      {
        path: "/docs/how-does-it-work",
        element: <HowDoesItWork />
      },
      {
        path: "/docs/dev/getting-started2",
        element: <GettingStarted />
      }
    ]
  }
])

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <RouterProvider router={router}/>
  </StrictMode>,
)