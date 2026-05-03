import { useHideOnScroll } from "@/hooks/use-hide-on-scroll";
import { cn } from "@/lib/utils";
import { SidebarTrigger } from "../ui/sidebar";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "../ui/dialog";
import { Button } from "../ui/button";
import { ChevronDown, Search } from "lucide-react";
import { SearchInput } from "../ui/search-input";

const searchPlaceholder = "Search factions, units, abilities, ...";

export function AppHeader() {
  const isHeaderHidden = useHideOnScroll();

  return (
    <header
      className={cn(
        isHeaderHidden ? "-translate-y-full" : "translate-y-0",
        "sticky top-0 z-40 flex w-full items-center justify-between gap-1 border-b border-border bg-sidebar py-2 pl-2 transition-transform duration-200 md:pl-4",
      )}
    >
      <div className="flex items-center gap-5">
        <SidebarTrigger size="icon-lg" className="md:hidden" />
        <Dialog>
          <DialogTrigger
            className="cursor-pointer md:hidden"
            render={
              <Button variant="ghost" size="icon-lg">
                <Search />
              </Button>
            }
          />
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Search</DialogTitle>
              <SearchInput placeholder={searchPlaceholder} />
            </DialogHeader>
          </DialogContent>
        </Dialog>
        <SearchInput className="hidden max-w-xs md:flex" placeholder={searchPlaceholder} />
      </div>
      <div className="flex max-w-xs items-center gap-3 pr-2">
        <span className="inline-flex size-10 shrink-0 items-center justify-center rounded-3xl border-2 border-sidebar-border bg-sidebar-ring font-bold text-primary-foreground">
          PU
        </span>
        <span className="hidden truncate text-foreground md:block">Placeholder User</span>
        <ChevronDown className="hidden size-4 shrink-0 md:block" />
      </div>
    </header>
  );
}
