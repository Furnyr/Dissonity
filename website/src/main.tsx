import { ReactNode, StrictMode, useEffect } from "react";
import { createRoot } from "react-dom/client";
import {
  createBrowserRouter,
  Navigate,
  RouterProvider,
} from "react-router-dom";
import "./index.css";

import hljs from "highlight.js/lib/core";
import javascriptHl from "highlight.js/lib/languages/javascript";
import typescriptHl from "highlight.js/lib/languages/typescript";
import xmlHl from "highlight.js/lib/languages/xml";
import rustHl from "highlight.js/lib/languages/rust";
import csharpHl from "highlight.js/lib/languages/csharp";

import Root from "./routes/root.tsx";
import Docs from "./routes/docs.tsx";
import Guides from "./routes/guides.tsx";
import ErrorPage from "./routes/error-page.tsx";
import About from "./routes/about.tsx";

import GuidesIndex from "./routes/guides/guidesIndex.tsx";
import GettingStarted from "./routes/guides/getting-started.tsx";
import HowDoesItWork from "./routes/guides/how-does-it-work.tsx";
import WhyDissonity from "./routes/guides/why-dissonity.tsx";
import MigrationV2 from "./routes/guides/migration-v2.tsx";

import DocsIndex from "./routes/docs/docsIndex.tsx";
import Performance from "./routes/docs/development/performance.tsx";
import Authentication from "./routes/docs/development/authentication.tsx";
import Security from "./routes/docs/development/security.tsx";
import Multiplayer from "./routes/docs/development/multiplayer.tsx";
import Debugging from "./routes/docs/development/debugging.tsx";

import StaticApi from "./routes/docs/api/api.tsx";
import Configuration from "./routes/docs/api/configuration.tsx";
import Utils from "./routes/docs/api/utils.tsx";
import Exceptions from "./routes/docs/api/exceptions.tsx";

import LocalDevelopment from "./routes/docs/internals/local-development.tsx";
import Design from "./routes/docs/internals/design.tsx";
import Mock from "./routes/docs/internals/mock.tsx";
import WebGLTemplate from "./routes/docs/internals/webgl-template.tsx";
import BuildVariables from "./routes/docs/internals/build-variables.tsx";
import HiRpc from "./routes/docs/internals/hirpc.tsx";

import AutoScrollOnLoad from "./components/AutoScrollOnLoad.tsx";

hljs.registerLanguage("javascript", javascriptHl);
hljs.registerLanguage("typescript", typescriptHl);
hljs.registerLanguage("rust", rustHl);
hljs.registerLanguage("csharp", csharpHl);
hljs.registerLanguage("xml", xmlHl);

(async () => {
  const lightMode = window.matchMedia && window.matchMedia("(prefers-color-scheme: light)").matches;

  lightMode
    ? await import("highlight.js/styles/atom-one-light.css")
    : await import("highlight.js/styles/atom-one-dark.css");
})();

const RedirectToReference = () => {
  useEffect(() => {
    window.location.href = "/ref/index.html";
  }, []);

  return null;
};

const wrapElement = (element: ReactNode) => {
  return (
    <AutoScrollOnLoad>
      {element}
    </AutoScrollOnLoad>
  );
}

const router = createBrowserRouter([
  {
    path: "/",
    element: wrapElement(<Root/>),
    errorElement: wrapElement(<ErrorPage/>)
  },
  {
    path: "/error-test",
    element: wrapElement(<ErrorPage />)
  },
  {
    path: "/about",
    element: wrapElement(<About />)
  },
  {
    path: "/docs",
    element: <Navigate to="/docs/v2/index" replace />
  },
  {
    path: "/docs/v2",
    element: wrapElement(<Docs />),
    children: [
      {
        path: "/docs/v2/index",
        element: <DocsIndex />
      },
      {
        path: "/docs/v2/api",
        element: <StaticApi />
      },
      {
        path: "/docs/v2/api/config",
        element: <Configuration />
      },
      {
        path: "/docs/v2/api/utils",
        element: <Utils />
      },{
        path: "/docs/v2/api/configuration",
        element: <Configuration />
      },
      {
        path: "/docs/v2/api/exceptions",
        element: <Exceptions />
      },
      {
        path: "/docs/v2/development/performance",
        element: <Performance />
      },
      {
        path: "/docs/v2/development/authentication",
        element: <Authentication />
      },
      {
        path: "/docs/v2/development/security",
        element: <Security />
      },
      {
        path: "/docs/v2/development/multiplayer",
        element: <Multiplayer />
      },
      {
        path: "/docs/v2/development/debugging",
        element: <Debugging />
      },
      {
        path: "/docs/v2/internals/local-development",
        element: <LocalDevelopment />
      },
      {
        path: "/docs/v2/internals/design",
        element: <Design />
      },
      {
        path: "/docs/v2/internals/mock",
        element: <Mock />
      },
      {
        path: "/docs/v2/internals/webgl-template",
        element: <WebGLTemplate />
      },
      {
        path: "/docs/v2/internals/build-variables",
        element: <BuildVariables />
      },
      {
        path: "/docs/v2/internals/hirpc",
        element: <HiRpc />
      },
      {
        path: "/docs/v2/dev/getting-started2",
        element: <GettingStarted />
      }
    ]
  },
  {
    path: "/guides",
    element: <Navigate to="/guides/v2/index" replace />
  },
  {
    path: "/guides/v2",
    element: wrapElement(<Guides />),
    children: [
      {
        path: "/guides/v2/index",
        element: <GuidesIndex />
      },
      {
        path: "/guides/v2/getting-started",
        element: <GettingStarted />
      },
      {
        path: "/guides/v2/how-does-it-work",
        element: <HowDoesItWork />
      },
      {
        path: "/guides/v2/why-dissonity",
        element: <WhyDissonity />
      },
      {
        path: "/guides/v2/migration-v2",
        element: <MigrationV2 />
      },
    ]
  },
  {
    path: "/team",
    element: <Navigate to="/about" replace />
  },
  {
    path: "/doxygen/*",
    element: <RedirectToReference />
  }
])

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>
);
