import { FaGithub, FaWhatsapp, FaBook, FaLinkedin } from "react-icons/fa";
import { COPYRIGHT_YEAR } from "@/config";
import { useTranslation } from "@/hooks";
import styles from "./Footer.module.css";

export default function Footer() {
  const { t } = useTranslation();
  return (
    <footer className={styles.footer}>
      <div className={styles.footerContainer}>
        <div className={styles.footerBrand}>
          <div className={styles.footerLogo}>
            <FaBook className={styles.footerIcon} />
            <h3>{t("footer.brand")}<span> {t("footer.brandHighlight")}</span></h3>
          </div>
          <p className={styles.footerSlogan}>{t("footer.slogan")}</p>
        </div>

        <div className={styles.footerLinksSection}>
          <h4>{t("footer.developer")}</h4>
          <p className={styles.authorName}>{t("footer.authorName")}</p>
          <div className={styles.footerSocials}>
            <a href="https://github.com/MaximilianoGimenez0" target="_blank" rel="noreferrer" aria-label="GitHub">
              <FaGithub />
            </a>
            <a href="https://wa.me/5491140675852" target="_blank" rel="noreferrer" aria-label="WhatsApp">
              <FaWhatsapp />
            </a>
            <a href="https://www.linkedin.com/in/maximilianogimenez0/" target="_blank" rel="noreferrer" aria-label="LinkedIn">
              <FaLinkedin />
            </a>
          </div>
        </div>
      </div>

      <div className={styles.footerBottom}>
        <p>&copy; {COPYRIGHT_YEAR} {t("footer.copyright")}</p>
      </div>
    </footer>
  );
}
