import { cn } from "@/lib/utils";
import { Link, NavLink, Outlet } from "react-router";
import { Dices, Database, Play, ListPlus, Settings } from "lucide-react";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuBadge,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarProvider,
  SidebarRail,
  SidebarSeparator,
  SidebarTrigger,
} from "../ui/sidebar";
import { Anvil } from "lucide-react";
//import { Astroid } from "lucide-react";

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  cn(
    "px-2 flex items-center border-b-2 border-transparent text-lg text-muted-foreground hover:text-card-foreground",
    isActive && "border-primary text-card-foreground",
  );

const Logo = () => (
  <NavLink
    to="/"
    className="flex items-center gap-3 border-r-2 border-border py-2 pr-15 pl-6 leading-none text-primary"
    end
  >
    <Dices size={32} />
    <div className="item-start flex flex-col font-heading leading-none tracking-wide text-primary uppercase">
      <span className="text-2xl">AoS</span>
      <span className="text-xs tracking-widest text-muted-foreground">Adjutant</span>
    </div>
  </NavLink>
);

const headerOld = (
  <header className="bg-card">
    <nav className="flex items-stretch border-b-2 border-border">
      <Logo />
      <ul className="flex items-stretch gap-3 pl-5 font-semibold">
        <li className="flex">
          <NavLink to="/factions" className={navLinkClass}>
            Factions
          </NavLink>
        </li>
        <li className="flex">
          <NavLink to="/abilities" className={navLinkClass}>
            Abilities
          </NavLink>
        </li>
      </ul>
    </nav>
  </header>
);

const footerOld = (
  <footer className="flex flex-col gap-1 border-t-2 border-border bg-card py-2 text-xs text-muted-foreground">
    <div className="flex items-center justify-center gap-3">
      <a
        className="hover:text-card-foreground"
        href="https://github.com/dhunsel/aos-adjutant"
        target="_blank"
        rel="noopener noreferrer"
      >
        Github
      </a>
      <span aria-hidden="true">{"\u00B7"}</span>
      <span>AoS Adjutant - {__APP_VERSION__}</span>
    </div>
    <small className="text-center">
      AoS Adjutant is an unofficial application and not affiliated with Games Workshop. Warhammer,
      Age of Sigmar, and associated brands are © Games Workshop Ltd.
    </small>
  </footer>
);

export function AppLayout() {
  return (
    <div className="flex min-h-screen flex-col">
      <SidebarProvider open={false}>
        <Sidebar collapsible="icon">
          <SidebarHeader>
            <Link
              to="/"
              className="flex size-12 items-center justify-center rounded-lg border-2 border-border bg-primary text-primary-foreground [&_svg]:size-8"
            >
              <Anvil />
            </Link>
          </SidebarHeader>
          <SidebarSeparator />
          <SidebarContent>
            <SidebarGroup>
              <SidebarMenu>
                <SidebarMenuItem>
                  <SidebarMenuButton isActive={true} size={"xl"} tooltip={"Dashboard"}>
                    <Database />
                  </SidebarMenuButton>
                </SidebarMenuItem>
                <SidebarMenuItem>
                  <SidebarMenuButton
                    size={"xl"}
                    aria-disabled={true}
                    className="cursor-not-allowed aria-disabled:pointer-events-auto"
                    tooltip={"List Builder (TBD)"}
                  >
                    <ListPlus />
                  </SidebarMenuButton>
                </SidebarMenuItem>
                <SidebarMenuItem>
                  <SidebarMenuButton
                    size={"xl"}
                    aria-disabled={true}
                    className="cursor-not-allowed aria-disabled:pointer-events-auto"
                    tooltip={"Play Mode (TBD)"}
                  >
                    <Play />
                  </SidebarMenuButton>
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
        <main className="flex-1">
          <div className="mx-auto max-w-7xl">
            <Outlet />
          </div>
        </main>
      </SidebarProvider>
    </div>
  );
}
