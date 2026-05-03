import {
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuItem,
} from "@/components/ui/sidebar";
import { Book, Cloud, Component, Crown, Gem, Sparkles, Swords, Users } from "lucide-react";
import { useParams } from "react-router";
import { useFaction } from "../factions/faction.queries";
import { Skeleton } from "@/components/ui/skeleton";
import { SidebarNavButton } from "@/components/ui/sidebar-nav-button";
import type { ComponentProps } from "react";

export function FactionSidebarGroup({
  size,
}: {
  size?: ComponentProps<typeof SidebarNavButton>["size"];
}) {
  const params = useParams();
  const factionId = Number(params["factionId"]);
  const faction = useFaction(factionId);

  return (
    <SidebarGroup>
      <SidebarGroupLabel className="font-heading">
        {faction.isLoading ? <Skeleton className="h-4 w-50" /> : faction.data?.name}
      </SidebarGroupLabel>
      <SidebarMenu>
        <SidebarNavButton size={size} to={`factions/${factionId.toString()}/battle-traits`}>
          <Swords />
          Battle Traits
        </SidebarNavButton>
        <SidebarMenuItem>
          <SidebarNavButton size={size} to={`factions/${factionId.toString()}/battle-formations`}>
            <Component />
            Battle Formations
          </SidebarNavButton>
        </SidebarMenuItem>
        <SidebarMenuItem>
          <SidebarNavButton size={size} to={`factions/${factionId.toString()}/units`}>
            <Users />
            Units
          </SidebarNavButton>
        </SidebarMenuItem>
        <SidebarMenuItem>
          <SidebarNavButton size={size} to={`factions/${factionId.toString()}/heroic-traits`}>
            <Crown />
            Heroic Traits
          </SidebarNavButton>
        </SidebarMenuItem>
        <SidebarMenuItem>
          <SidebarNavButton size={size} to={`factions/${factionId.toString()}/artefacts`}>
            <Gem />
            Artefacts of Power
          </SidebarNavButton>
        </SidebarMenuItem>
        <SidebarMenuItem>
          <SidebarNavButton size={size} to={`factions/${factionId.toString()}/spell-lores`}>
            <Sparkles />
            Spell Lores
          </SidebarNavButton>
        </SidebarMenuItem>
        <SidebarMenuItem>
          <SidebarNavButton size={size} to={`factions/${factionId.toString()}/prayer-lores`}>
            <Book />
            Prayer Lores
          </SidebarNavButton>
        </SidebarMenuItem>
        <SidebarMenuItem>
          <SidebarNavButton size={size} to={`factions/${factionId.toString()}/manifestations`}>
            <Cloud />
            Manifestation Lores
          </SidebarNavButton>
        </SidebarMenuItem>
      </SidebarMenu>
    </SidebarGroup>
  );
}
