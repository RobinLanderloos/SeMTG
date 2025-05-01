import { Link } from "react-router-dom";
import { SearchResultCard } from "../../lib/api";
import styles from "./styles.module.css";

export default function SearchResults({
  cards,
}: Readonly<{ cards: SearchResultCard[] }>) {
  return (
    <div className={styles.searchResultsContainer}>
      {cards.map((card) => (
        <div key={card.id} className={styles.searchResult}>
          <Link to={`/card/${card.id}`}>
            <img
              src={card.imageUris.png}
              alt={card.name}
              className={styles.searchResultImage}
            />
          </Link>
        </div>
      ))}
    </div>
  );
}
