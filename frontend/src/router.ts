import { createBrowserRouter } from "react-router";
import { AppLayout } from "./components/layouts/app-layout";
import { Home } from "./pages/home";
import { NotFound } from "./pages/not-found";
import { UnexpectedError } from "./pages/unexpected-error";
import { dashboardRouter } from "./features/dashboard/router";

export const router = createBrowserRouter([
  {
    path: "/",
    ErrorBoundary: UnexpectedError,
    Component: AppLayout,
    children: [{ index: true, Component: Home }, dashboardRouter],
  },
  {
    path: "*",
    Component: NotFound,
  },
]);
