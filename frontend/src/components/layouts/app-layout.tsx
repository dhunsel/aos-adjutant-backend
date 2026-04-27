import { NavLink, Outlet } from "react-router";

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  isActive ? "bg-accent text-accent-foreground" : "text-muted-foreground hover:text-foreground";

export function AppLayout() {
  return (
    <>
      <header>
        <nav>
          <NavLink to="/" className={navLinkClass} end>
            Aos Adjutant
          </NavLink>
          <ul>
            <li>
              <NavLink to="/factions" className={navLinkClass}>
                Factions
              </NavLink>
            </li>
            <li>
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
