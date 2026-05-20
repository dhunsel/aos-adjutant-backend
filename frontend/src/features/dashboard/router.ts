import { DashBoardLayout } from "./layouts/dashboard-layout";
import { FactionSidebarGroup } from "./layouts/faction-sidebar-group";
import type { DasboardRouteHandle } from "./route-handle";
import type { RouteObject } from "react-router";
import { FactionListPage } from "./factions/pages/faction-list-page";

export const dashboardRouter: RouteObject = {
  path: "/dashboard",
  Component: DashBoardLayout,
  children: [
    {
      path: "factions",
      Component: FactionListPage,
      children: [
        {
          path: ":factionId",
          Component: FactionListPage,
          handle: { sidebar: FactionSidebarGroup } satisfies DasboardRouteHandle,
        },
      ],
    },
  ],
};
