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
import { SidebarNavButton } from "./sidebar-nav-button";

export function AppSidebar() {
  return (
    <Sidebar collapsible="icon">
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarNavButton
              showActive={false}
              className="bg-primary font-semibold text-primary-foreground hover:bg-primary hover:text-primary-foreground active:bg-primary active:text-primary-foreground"
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
              <SidebarNavButton tooltip="Dashboard" to="/factions">
                <Database />
                Dashboard
              </SidebarNavButton>
            </SidebarMenuItem>
            <SidebarMenuItem>
              <SidebarNavButton disabled tooltip="List Builder (TBD)" to="/list-builder">
                <ListPlus />
                List Builder
              </SidebarNavButton>
            </SidebarMenuItem>
            <SidebarMenuItem>
              <SidebarNavButton disabled tooltip="Play Mode (TBD)" to="/battle">
                <Play />
                Battle Mode
              </SidebarNavButton>
            </SidebarMenuItem>
          </SidebarMenu>
        </SidebarGroup>
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
