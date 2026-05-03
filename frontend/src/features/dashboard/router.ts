import { DashBoardLayout } from "./layouts/dashboard-layout";
import App from "@/App";
import { FactionSidebarGroup } from "./layouts/faction-sidebar-group";
import type { DasboardRouteHandle } from "./route-handle";
import type { RouteObject } from "react-router";

export const dashboardRouter: RouteObject = {
  path: "/dashboard",
  Component: DashBoardLayout,
  children: [
    {
      path: "factions",
      Component: App,
      children: [
        {
          path: ":factionId",
          Component: App,
          handle: { sidebar: FactionSidebarGroup } satisfies DasboardRouteHandle,
        },
      ],
    },
  ],
};
