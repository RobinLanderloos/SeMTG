import { useEffect } from "react";
import styles from "./styles.module.css";
import { useSearchParams } from "react-router-dom";

export default function TitleBar() {
  const [searchParams] = useSearchParams();
  const query = searchParams.get("query");

  useEffect(() => {}, [query]);

  return (
    <div className={styles.titleBar}>
      <div className={styles.siteName}>SeMTG</div>
    </div>
  );
}
