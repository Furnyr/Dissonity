import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import {
  createHashRouter,
  RouterProvider
} from 'react-router-dom';
import './index.css'
import Root from './routes/root.tsx'
import Docs from './routes/docs.tsx'
import ErrorPage from './error-page.tsx'

const router = createHashRouter([
  {
    path: '/',
    element: <Root />,
    errorElement: <ErrorPage />,
  },
  {
    path: '/docs',
    element: <Docs />,
  }
])

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <RouterProvider router={router}/>
  </StrictMode>,
)