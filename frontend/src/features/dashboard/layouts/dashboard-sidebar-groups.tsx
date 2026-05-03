import { SidebarNavButton } from "@/components/ui/sidebar-nav-button";
import {
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuItem,
} from "@/components/ui/sidebar";
import { Flag, List, Megaphone } from "lucide-react";
import { hasSidebar } from "../route-handle";
import { useMatches } from "react-router";
import type { ComponentProps } from "react";

export function DashboardSidebarGroups({
  size,
}: {
  size?: ComponentProps<typeof SidebarNavButton>["size"];
}) {
  const matches = useMatches();

  return (
    <>
      <SidebarGroup>
        <SidebarGroupLabel className="font-heading">General</SidebarGroupLabel>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarNavButton className="font-sans" to="/dashboard/factions" size={size}>
              <Flag />
              Factions
            </SidebarNavButton>
          </SidebarMenuItem>
          <SidebarMenuItem>
            <SidebarNavButton disabled className="font-sans" to="/dashboard/commands" size={size}>
              <Megaphone />
              Commands
            </SidebarNavButton>
          </SidebarMenuItem>
          <SidebarMenuItem>
            <SidebarNavButton disabled className="font-sans" to="/dashboard/keywords" size={size}>
              <List />
              Keywords
            </SidebarNavButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarGroup>
      {matches.map((m) => {
        if (!hasSidebar(m.handle)) return null;
        const HandleSidebarGroup = m.handle.sidebar;
        return <HandleSidebarGroup size={size} key={m.id} />;
      })}
    </>
  );
}
