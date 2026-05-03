import {
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar";
import { Flag, List, Megaphone } from "lucide-react";
import type { CSSProperties } from "react";
import { useMatches } from "react-router";
import { hasSidebar } from "../route-handle";

export function DashboardSidebar() {
  const matches = useMatches();

  return (
    <Sidebar
      collapsible="none"
      className="hidden md:flex"
      style={{ "--sidebar-width": "12rem" } as CSSProperties}
    >
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel className="font-heading">General</SidebarGroupLabel>
          <SidebarMenu>
            <SidebarMenuItem>
              <SidebarMenuButton>
                <Flag />
                Factions
              </SidebarMenuButton>
            </SidebarMenuItem>
            <SidebarMenuItem>
              <SidebarMenuButton>
                <Megaphone />
                Commands
              </SidebarMenuButton>
            </SidebarMenuItem>
            <SidebarMenuItem>
              <SidebarMenuButton>
                <List />
                Keywords
              </SidebarMenuButton>
            </SidebarMenuItem>
          </SidebarMenu>
        </SidebarGroup>
        {matches.map((m) => {
          if (!hasSidebar(m.handle)) return null;
          const HandleSidebarGroup = m.handle.sidebar;
          return <HandleSidebarGroup key={m.id} />;
        })}
      </SidebarContent>
    </Sidebar>
  );
}
