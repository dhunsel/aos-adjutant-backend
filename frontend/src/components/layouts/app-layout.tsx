import { cn } from "@/lib/utils";
import { Link, NavLink, Outlet, useMatch } from "react-router";
import { Anvil, Database, Play, ListPlus, Settings, Search, ChevronDown } from "lucide-react";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarProvider,
  SidebarSeparator,
} from "../ui/sidebar";
import { InputGroup, InputGroupAddon, InputGroupInput } from "../ui/input-group";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "../ui/dialog";
import GithubLogo from "@/assets/GitHub_Invertocat_White.svg?react";

type SidebarNavButtonProps = {
  to: string;
  disabled?: boolean;
} & Omit<React.ComponentProps<typeof SidebarMenuButton>, "render" | "isActive">;

const SidebarNavButton = ({
  to,
  disabled,
  className,
  children,
  ...props
}: SidebarNavButtonProps) => {
  const isActive = !!useMatch({ path: to, end: false });

  if (disabled)
    return (
      <SidebarMenuButton
        size="xl"
        aria-disabled
        className={cn("cursor-not-allowed aria-disabled:pointer-events-auto", className)}
        {...props}
      >
        {children}
      </SidebarMenuButton>
    );

  return (
    <SidebarMenuButton size="xl" render={<NavLink to={to} />} isActive={isActive} {...props}>
      {children}
    </SidebarMenuButton>
  );
};

export function AppLayout() {
  return (
    <div>
      <SidebarProvider open={false}>
        <Sidebar collapsible="icon">
          <SidebarHeader>
            <Link
              to="/"
              className="flex size-12 items-center justify-center rounded-lg border-2 border-border bg-primary text-primary-foreground"
            >
              <Anvil className="size-8" />
            </Link>
          </SidebarHeader>
          <SidebarSeparator />
          <SidebarContent>
            <SidebarGroup>
              <SidebarMenu>
                <SidebarMenuItem>
                  <SidebarNavButton tooltip="Dashboard" to="/factions">
                    <Database />
                  </SidebarNavButton>
                </SidebarMenuItem>
                <SidebarMenuItem>
                  <SidebarNavButton disabled tooltip="List Builder (TBD)" to="/list-builder">
                    <ListPlus />
                  </SidebarNavButton>
                </SidebarMenuItem>
                <SidebarMenuItem>
                  <SidebarNavButton disabled tooltip="Play Mode (TBD)" to="/battle">
                    <Play />
                  </SidebarNavButton>
                </SidebarMenuItem>
              </SidebarMenu>
            </SidebarGroup>
          </SidebarContent>
          <SidebarFooter>
            <SidebarMenu>
              <SidebarMenuItem>
                <SidebarMenuButton size={"xl"} tooltip={"Settings"}>
                  <Settings />
                </SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
          </SidebarFooter>
        </Sidebar>
        <div className="flex min-h-screen flex-1 flex-col">
          <header className="flex w-full items-center justify-between gap-1 border-b border-border bg-sidebar px-5 py-2">
            <InputGroup className="max-w-xs">
              <InputGroupAddon>
                <Search />
              </InputGroupAddon>
              <InputGroupInput type="search" placeholder="Search factions, units, abilities, ..." />
            </InputGroup>
            <div className="flex max-w-xs items-center gap-3">
              <span className="size-10 shrink-0 rounded-4xl border-3 border-sidebar-border bg-sidebar-ring p-1 text-center font-bold text-primary-foreground">
                PU
              </span>
              <span className="truncate text-muted-foreground">Placeholder User</span>
              <ChevronDown className="size-4 shrink-0" />
            </div>
          </header>
          <main className="flex-1">
            <div className="mx-auto max-w-7xl">
              <Outlet />
            </div>
          </main>
          <footer className="flex items-center justify-end gap-3 border-t border-border bg-sidebar py-1 pr-3 text-xs text-muted-foreground">
            <Dialog>
              <DialogTrigger className="cursor-pointer hover:text-sidebar-foreground">
                Disclaimer
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Disclaimer</DialogTitle>
                  <DialogDescription>
                    AoS Adjutant is an unofficial application and not affiliated with Games
                    Workshop. Warhammer, Age of Sigmar, and associated brands are {"\u00A9"} Games
                    Workshop Ltd.
                  </DialogDescription>
                </DialogHeader>
              </DialogContent>
            </Dialog>
            <span aria-hidden="true">{"\u00B7"}</span>
            <a
              className="shrink-0"
              href="https://github.com/dhunsel/aos-adjutant"
              target="_blank"
              rel="noopener noreferrer"
            >
              <GithubLogo aria-label="Github Repository" className="size-4" />
            </a>
            <span aria-hidden="true">{"\u00B7"}</span>
            <span className="font-mono">v{__APP_VERSION__}</span>
          </footer>
        </div>
      </SidebarProvider>
    </div>
  );
}
