import { useState } from "react";
import { Hit, searchCards } from "../../lib/api";

export default function SearchPage() {
  const [query, setQuery] = useState("");
  const [results, setResults] = useState<Hit[]>([]);
  const [loading, setLoading] = useState(false);

  const handleSearch = async () => {
    setLoading(true);
    try {
      const response = await searchCards(query);
      console.log("Hits: ", response);
      setResults(response);
    } catch (err) {
      console.error("Search failed:", err);
      setResults([]);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-6 max-w-4xl mx-auto">
      <h1 className="text-2xl font-bold mb-4">
        Magic: The Gathering Semantic Search
      </h1>

      <div className="flex gap-2 mb-6">
        <input
          type="text"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Search cards by description or sentiment..."
          className="flex-grow border rounded px-4 py-2"
        />
        <button
          onClick={handleSearch}
          disabled={loading}
          className="bg-blue-600 text-white px-4 py-2 rounded disabled:opacity-50"
        >
          {loading ? "Searching..." : "Search"}
        </button>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
        {results.map((hit) => (
          <div
            key={hit.card.id}
            className="border rounded p-2 shadow hover:shadow-lg transition"
          >
            <img
              src={hit.card.editions[0].imageUri}
              alt={hit.card.editions[0].name}
              className="w-full h-auto rounded mb-2"
            />
            <h2 className="text-lg font-semibold text-center">
              {hit.card.editions[0].name}
            </h2>
          </div>
        ))}
      </div>
    </div>
  );
}
