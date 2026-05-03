import { Outlet } from "react-router";
import { DashboardSidebar } from "./dashboard-sidebar";
import { SidebarProvider } from "@/components/ui/sidebar";

export function DashBoardLayout() {
  return (
    <SidebarProvider>
      <DashboardSidebar />
      <main className="flex-1 px-3 py-2 md:px-5 md:py-6">
        <div className="mx-auto max-w-7xl">
          <Outlet />
        </div>
      </main>
    </SidebarProvider>
  );
}
