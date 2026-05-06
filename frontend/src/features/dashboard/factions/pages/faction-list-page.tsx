import { Spinner } from "@/components/ui/spinner";
import { useFactions } from "../faction.queries";
import type { FactionQuery, GrandAlliance } from "@/types/api.types";
import { Badge } from "@/components/ui/badge";
import { useState, type ComponentProps } from "react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";

const GrandAllianceBadge = ({
  grandAlliance,
  className,
  ...props
}: { grandAlliance: GrandAlliance } & ComponentProps<typeof Badge>) => {
  let color;
  switch (grandAlliance) {
    case "Order": {
      color = "bg-alliance-order text-foreground";
      break;
    }
    case "Chaos": {
      color = "bg-alliance-chaos text-foreground";
      break;
    }
    case "Death": {
      color = "bg-alliance-death text-foreground";
      break;
    }
    case "Destruction": {
      color = "bg-alliance-destruction text-foreground";
      break;
    }
  }

  return (
    <Badge className={cn(color, className)} {...props}>
      {grandAlliance}
    </Badge>
  );
};

export function FactionListPage() {
  const [filter, setFilter] = useState<FactionQuery>();
  const factions = useFactions(filter);

  if (factions.isLoading)
    return (
      <div className="pt-10">
        <Spinner className="mx-auto" />
      </div>
    );

  return (
    <div className="flex-col">
      <div className="flex items-center gap-2 bg-card pb-2">
        <span>Filter</span>
        <GrandAllianceBadge
          onClick={() => {
            setFilter({ GrandAlliance: "Order" });
          }}
          grandAlliance="Order"
        />
        <GrandAllianceBadge grandAlliance="Chaos" />
        <GrandAllianceBadge grandAlliance="Death" />
        <GrandAllianceBadge grandAlliance="Destruction" />
      </div>
      <div className="grid grid-cols-[repeat(auto-fill,minmax(250px,1fr))] gap-3">
        {factions.data?.items.map((f) => (
          <div
            key={f.factionId}
            className="flex flex-col gap-2 rounded-lg border border-border bg-card p-2 text-nowrap text-card-foreground"
          >
            <span className="font-heading text-xl">{f.name}</span>
            <GrandAllianceBadge grandAlliance={f.grandAlliance} />
          </div>
        ))}
      </div>
    </div>
  );
}
