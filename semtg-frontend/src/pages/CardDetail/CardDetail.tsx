import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { CardDetailResult, getCard } from "../../lib/api";
import styles from "./styles.module.css";

export default function CardDetail() {
  const { id } = useParams();
  const [card, setCard] = useState<CardDetailResult | null>(null);

  useEffect(() => {
    if (!id) return;

    const loadCard = async () => {
      try {
        const result = await getCard(id);
        setCard(result);
      } catch (err) {
        console.error("Failed to load card:", err);
        setCard(null);
      }
    };

    loadCard();
  }, [id]);

  if (!id) return <p>Invalid card ID.</p>;
  if (!card) return <p>Loading card...</p>;

  const lastEdition = card.editions[card.editions.length - 1];

  return (
    <div className={styles.cardDetailContainer}>
      {lastEdition && (
        <div className={styles.edition}>
          <img
            src={lastEdition.imageUris.png}
            alt={lastEdition.name}
            className={styles.cardImage}
          />
          <div className={styles.cardDetailText}>
            <h2 className={styles.editionName}>{lastEdition.name}</h2>
            <p>
              <strong>Type:</strong> {lastEdition.typeLine}
            </p>
            <p>
              <strong>Oracle Text:</strong> {lastEdition.oracleText}
            </p>
          </div>
        </div>
      )}
    </div>
  );
}
