import { Link } from "react-router-dom";
import { FiHome } from "react-icons/fi";
import { useTranslation } from "@/hooks";
import styles from "./NotFound.module.css";

export default function NotFound() {
  const { t } = useTranslation();

  return (
    <div className={styles.container}>
      <h1 className={styles.errorCode}>{t("pages.notFound.code")}</h1>
      <h2 className={styles.title}>{t("pages.notFound.title")}</h2>
      <p className={styles.description}>
        {t("pages.notFound.description")}
      </p>
      <Link to="/" className={styles.homeButton}>
        <FiHome aria-hidden="true" /> {t("pages.notFound.backHome")}
      </Link>
    </div>
  );
}
