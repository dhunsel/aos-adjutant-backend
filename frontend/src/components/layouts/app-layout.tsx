import { Outlet } from "react-router";
import { SidebarProvider } from "../ui/sidebar";
import { AppHeader } from "./app-header";
import { AppFooter } from "./app-footer";
import { AppSidebar } from "./app-sidebar";

export function AppLayout() {
  return (
    // On desktop always show icon rail as sidebar (i.e., sidebar is always closed)
    // On mobile use sheet
    <SidebarProvider open={false}>
      <AppSidebar />
      <div className="flex min-h-screen flex-1 flex-col">
        <AppHeader />
        <div className="flex flex-1">
          <Outlet />
        </div>
        <AppFooter />
      </div>
    </SidebarProvider>
  );
}
