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
        <main className="flex-1 px-3 py-2 md:px-5 md:py-6">
          <div className="mx-auto max-w-7xl">
            <Outlet />
          </div>
        </main>
        <AppFooter />
      </div>
    </SidebarProvider>
  );
}
