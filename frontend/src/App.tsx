import { Button } from "./components/ui/button";
import { useFactions } from "./features/factions/faction.queries";

function App() {
  const factionsQuery = useFactions({
    page: 1,
    pageSize: 1,
    sortBy: "Name",
    sortDirection: "Desc",
  });

  if (factionsQuery.isLoading) {
    return (
      <>
        <h1>Loading data</h1>
      </>
    );
  }

  const factions = factionsQuery.data;

  if (!factions) {
    return <h1>No data</h1>;
  }

  return (
    <>
      <h1 className="bg-amber-700 text-3xl font-bold">Aos Adjutant</h1>
      <Button>Test button</Button>
      <div>Faction 1: {factions.items[0]?.name}</div>
      <div>Faction 2: {factions.items[1]?.name}</div>
      <div>Faction 3: {factions.items[2]?.name}</div>
    </>
  );
}

export default App;
