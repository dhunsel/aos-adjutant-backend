import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "../ui/dialog";
import GithubLogo from "@/assets/GitHub_Invertocat_White.svg?react";

export function AppFooter() {
  return (
    <footer className="flex items-center justify-center gap-3 border-t border-border bg-sidebar py-1 pr-3 text-muted-foreground md:justify-end md:text-xs">
      <Dialog>
        <DialogTrigger className="cursor-pointer hover:text-sidebar-foreground">
          Disclaimer
        </DialogTrigger>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Disclaimer</DialogTitle>
            <DialogDescription>
              AoS Adjutant is an unofficial application and not affiliated with Games Workshop.
              Warhammer, Age of Sigmar, and associated brands are {"\u00A9"} Games Workshop Ltd.
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
        aria-label="Github Repository"
      >
        <GithubLogo className="size-6 md:size-4" />
      </a>
      <span aria-hidden="true">{"\u00B7"}</span>
      <span className="font-mono">v{__APP_VERSION__}</span>
    </footer>
  );
}
