import type { SidebarNavButton } from "@/components/ui/sidebar-nav-button";
import type { ComponentProps, ComponentType } from "react";

// Sidebar component expects size property
// Might change in the future to e.g., pass size through context to remove SidebarNavButton dep
export interface DasboardRouteHandle {
  sidebar?: ComponentType<{ size?: ComponentProps<typeof SidebarNavButton>["size"] }>;
}

export function hasSidebar(handle: unknown): handle is Required<DasboardRouteHandle> {
  return (
    typeof handle === "object" &&
    handle !== null &&
    "sidebar" in handle &&
    typeof handle.sidebar === "function"
  );
}
