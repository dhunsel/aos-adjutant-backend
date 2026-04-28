import { cn } from "@/lib/utils";
import { NavLink, Outlet } from "react-router";

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  cn(
    "px-2 flex items-center border-b-2 border-transparent text-lg text-muted-foreground hover:text-card-foreground",
    isActive && "border-primary text-card-foreground",
  );

export function AppLayout() {
  return (
    <>
      <header className="bg-card">
        <nav className="flex items-stretch border-b-2 border-border">
          <NavLink
            to="/"
            className="border-r-2 border-border px-3 pt-3 pb-2 font-heading text-2xl tracking-wide text-primary uppercase"
            end
          >
            AoS Adjutant
          </NavLink>
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
      <main>
        <Outlet />
      </main>
    </>
  );
}
