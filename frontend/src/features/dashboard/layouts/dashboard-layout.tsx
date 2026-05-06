import { Outlet } from "react-router";
import { DashboardSidebar } from "./dashboard-sidebar";

export function DashBoardLayout() {
  return (
    <div className="flex flex-1">
      <DashboardSidebar />
      <main className="flex-1 px-3 py-2 md:px-5 md:py-6">
        <div className="mx-auto max-w-7xl">
          <Outlet />
        </div>
      </main>
    </div>
  );
}
