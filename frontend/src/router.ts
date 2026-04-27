import { createBrowserRouter } from "react-router";
import App from "./App";
import { AppLayout } from "./components/layouts/app-layout";
import { Home } from "./pages/home";
import { NotFound } from "./pages/not-found";
import { UnexpectedError } from "./pages/unexpected-error";

export const router = createBrowserRouter([
  {
    path: "/",
    ErrorBoundary: UnexpectedError,
    Component: AppLayout,
    children: [
      { index: true, Component: Home },
      {
        path: "factions",
        children: [{ index: true, Component: App }],
      },
    ],
  },
  {
    path: "*",
    Component: NotFound,
  },
]);
