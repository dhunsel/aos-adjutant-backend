import { Anvil, Database, ListPlus, Play, Settings } from "lucide-react";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarSeparator,
} from "../ui/sidebar";
import { SidebarNavButton } from "../ui/sidebar-nav-button";
import { useMatch } from "react-router";
import { useIsMobile } from "@/hooks/use-mobile";
import { DashboardSidebarGroups } from "@/features/dashboard/layouts/dashboard-sidebar-groups";

export function AppSidebar() {
  const isInDashboard = useMatch({ path: "/dashboard", end: false });
  const isMobile = useIsMobile();

  // For now render the mode specific sidebar inside the app wide sidebar when on mobile
  // Re-evaluate later to separate the app wide sidebar and the mode specific sidebar because this file should technically not import feature specific code
  return (
    <Sidebar collapsible="icon">
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarNavButton
              showActive={false}
              className="bg-primary font-heading font-semibold text-primary-foreground hover:bg-primary hover:text-primary-foreground active:bg-primary active:text-primary-foreground"
              to="/"
            >
              <Anvil />
              AoS Adjutant
            </SidebarNavButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>
      <SidebarSeparator />
      <SidebarContent>
        <SidebarGroup>
          <SidebarMenu>
            <SidebarMenuItem>
              <SidebarNavButton className="font-heading" tooltip="Dashboard" to="/dashboard">
                <Database />
                Dashboard
              </SidebarNavButton>
            </SidebarMenuItem>
            <SidebarMenuItem>
              <SidebarNavButton
                className="font-heading"
                disabled
                tooltip="List Builder (TBD)"
                to="/list-builder"
              >
                <ListPlus />
                List Builder
              </SidebarNavButton>
            </SidebarMenuItem>
            <SidebarMenuItem>
              <SidebarNavButton
                className="font-heading"
                disabled
                tooltip="Play Mode (TBD)"
                to="/battle"
              >
                <Play />
                Battle Mode
              </SidebarNavButton>
            </SidebarMenuItem>
          </SidebarMenu>
        </SidebarGroup>
        {isMobile && isInDashboard && (
          <>
            <SidebarSeparator />
            <DashboardSidebarGroups size="xl" />
          </>
        )}
      </SidebarContent>
      <SidebarFooter>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton
              className="cursor-not-allowed font-heading text-nowrap aria-disabled:pointer-events-auto"
              aria-disabled={true}
              size="xl"
              tooltip="Settings (TBD)"
            >
              <Settings />
              Settings
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarFooter>
    </Sidebar>
  );
}
