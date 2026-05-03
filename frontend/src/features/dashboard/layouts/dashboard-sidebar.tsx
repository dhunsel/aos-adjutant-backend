import { Sidebar, SidebarContent } from "@/components/ui/sidebar";
import type { CSSProperties } from "react";
import { DashboardSidebarGroups } from "./dashboard-sidebar-groups";

export function DashboardSidebar() {
  return (
    <Sidebar
      collapsible="none"
      className="hidden md:flex"
      style={{ "--sidebar-width": "12rem" } as CSSProperties}
    >
      <SidebarContent>
        <DashboardSidebarGroups />
      </SidebarContent>
    </Sidebar>
  );
}
