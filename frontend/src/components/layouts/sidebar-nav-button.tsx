import type { ComponentProps } from "react";
import { SidebarMenuButton, useSidebar } from "../ui/sidebar";
import { useMatch } from "react-router";
import { cn } from "@/lib/utils";
import { Link } from "react-router";

type SidebarNavButtonProps = {
  to: string;
  disabled?: boolean;
  closeSidebarOnClick?: boolean;
  showActive?: boolean;
} & Omit<ComponentProps<typeof SidebarMenuButton>, "render" | "isActive">;

export function SidebarNavButton({
  to,
  disabled,
  closeSidebarOnClick = true,
  showActive = true,
  className,
  onClick,
  children,
  ...props
}: SidebarNavButtonProps) {
  const isActive = !!useMatch({ path: to, end: false });
  const { isMobile, openMobile, toggleSidebar } = useSidebar();

  if (disabled)
    return (
      <SidebarMenuButton
        size="xl"
        aria-disabled={true}
        className={cn(
          "cursor-not-allowed font-heading text-nowrap aria-disabled:pointer-events-auto",
          className,
        )}
        // No onClick for disabled button
        {...props}
      >
        {children}
      </SidebarMenuButton>
    );

  return (
    <SidebarMenuButton
      size="xl"
      render={<Link to={to} aria-current={isActive ? "page" : undefined} />}
      isActive={showActive && isActive}
      className={cn("font-heading text-nowrap", className)}
      onClick={(e) => {
        onClick?.(e);
        if (e.defaultPrevented) return;
        if (closeSidebarOnClick && isMobile && openMobile) toggleSidebar();
      }}
      {...props}
    >
      {children}
    </SidebarMenuButton>
  );
}
