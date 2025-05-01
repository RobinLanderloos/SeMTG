import { useEffect, useState } from "react";
import { SearchResult, searchCards } from "../../lib/api";
import SearchResults from "./SearchResults";
import { useSearchParams, useNavigate } from "react-router-dom";
import styles from "./styles.module.css";

export default function SearchPage() {
  const [searchParams] = useSearchParams();
  const [searchResult, setSearchResult] = useState<SearchResult>();
  const [searching, setSearching] = useState(false);
  const [queryInput, setQueryInput] = useState("");
  const query = searchParams.get("query");
  const navigate = useNavigate();

  useEffect(() => {
    if (query === null) {
      return;
    }

    setQueryInput(query);

    const handleSearch = async (query: string) => {
      try {
        setSearchResult(undefined);
        setSearching(true);
        const response = await searchCards(query);
        setSearchResult(response);
      } catch (err) {
        console.error("Search failed:", err);
        setSearchResult(undefined);
      } finally {
        setSearching(false);
      }
    };

    if (!query) {
      setSearchResult(undefined);
      return;
    }

    handleSearch(query);
  }, [query]);

  const handleSearch = () => {
    navigate(`/search?query=${encodeURIComponent(queryInput)}`);
  };

  return (
    <div>
      <div>
        <div className={styles.searchInputContainer}>
          <input
            type="text"
            className={styles.searchInput}
            value={queryInput}
            onChange={(e) => setQueryInput(e.target.value)}
            placeholder="Search cards by description or sentiment..."
          />
          <button onClick={handleSearch}>Search</button>
        </div>

        {searching && <p>Searching...</p>}
        {!searching && searchResult?.cards?.length === 0 && (
          <p>We couldn't find anything for that query</p>
        )}

        {searchResult?.cards && <SearchResults cards={searchResult.cards} />}
      </div>
    </div>
  );
}
