import type { ComponentType } from "react";

export interface DasboardRouteHandle {
  sidebar?: ComponentType;
}

export function hasSidebar(handle: unknown): handle is Required<DasboardRouteHandle> {
  return (
    typeof handle === "object" &&
    handle !== null &&
    "sidebar" in handle &&
    typeof handle.sidebar === "function"
  );
}
