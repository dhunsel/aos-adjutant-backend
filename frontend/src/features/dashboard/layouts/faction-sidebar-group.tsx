import {
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar";
import { Book, Cloud, Component, Crown, Gem, Sparkles, Swords, Users } from "lucide-react";
import { useParams } from "react-router";
import { useFaction } from "../factions/faction.queries";
import { Skeleton } from "@/components/ui/skeleton";

export function FactionSidebarGroup() {
  const params = useParams();
  const factionId = Number(params["factionId"]);
  const faction = useFaction(factionId);

  return (
    <SidebarGroup>
      <SidebarGroupLabel className="font-heading">
        {faction.isLoading ? <Skeleton className="h-4 w-50" /> : faction.data?.name}
      </SidebarGroupLabel>
      <SidebarMenu>
        <SidebarMenuItem>
          <SidebarMenuButton>
            <Swords />
            Battle Traits
          </SidebarMenuButton>
        </SidebarMenuItem>
        <SidebarMenuItem>
          <SidebarMenuButton>
            <Component />
            Battle Formations
          </SidebarMenuButton>
        </SidebarMenuItem>
        <SidebarMenuItem>
          <SidebarMenuButton>
            <Users />
            Units
          </SidebarMenuButton>
        </SidebarMenuItem>
        <SidebarMenuItem>
          <SidebarMenuButton>
            <Crown />
            Heroic Traits
          </SidebarMenuButton>
        </SidebarMenuItem>
        <SidebarMenuItem>
          <SidebarMenuButton>
            <Gem />
            Artefacts of Power
          </SidebarMenuButton>
        </SidebarMenuItem>
        <SidebarMenuItem>
          <SidebarMenuButton>
            <Sparkles />
            Spell Lores
          </SidebarMenuButton>
        </SidebarMenuItem>
        <SidebarMenuItem>
          <SidebarMenuButton>
            <Book />
            Prayer Lores
          </SidebarMenuButton>
        </SidebarMenuItem>
        <SidebarMenuItem>
          <SidebarMenuButton>
            <Cloud />
            Manifestation Lores
          </SidebarMenuButton>
        </SidebarMenuItem>
      </SidebarMenu>
    </SidebarGroup>
  );
}
