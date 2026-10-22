import styles from "./About.module.css";
import { APP_NAME } from "@/config";
import { FiTarget, FiTool, FiTrendingUp, FiCheckCircle } from "react-icons/fi";
import { useTranslation } from "@/hooks";

export default function About() {
  const { t } = useTranslation();

  return (
    <div className={styles.pageContainer}>
      <header className={styles.header}>
        <div className={styles.headerContent}>
          <h1 className={styles.title}>{t("pages.about.header.title", { appName: APP_NAME })}</h1>
          <p className={styles.subtitle}>
            {t("pages.about.header.subtitle")}
          </p>
        </div>
      </header>

      <div className={styles.contentContainer}>
        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>
            <FiTarget className={styles.icon} /> {t("pages.about.challenge.title")}
          </h2>
          <div className={styles.card}>
            <p className={styles.text}>
              {t("pages.about.challenge.desc")}
            </p>
          </div>
        </section>

        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>
            <FiCheckCircle className={styles.icon} /> {t("pages.about.solution.title")}
          </h2>
          <div className={styles.card}>
            <p className={styles.text}>
              {t("pages.about.solution.desc", { appName: APP_NAME })}
            </p>
            <ul className={styles.featureList}>
              <li><strong>{t("pages.about.solution.hub")}</strong> {t("pages.about.solution.hubDesc")}</li>
              <li><strong>{t("pages.about.solution.roles")}</strong> {t("pages.about.solution.rolesDesc")}</li>
              <li><strong>{t("pages.about.solution.audit")}</strong> {t("pages.about.solution.auditDesc")}</li>
            </ul>
          </div>
        </section>

        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>
            <FiTool className={styles.icon} /> {t("pages.about.tech.title")}
          </h2>
          <div className={styles.card}>
            <p className={styles.text}>
              {t("pages.about.tech.desc")}
            </p>
            <div className={styles.techGrid}>
              <div className={styles.techItem}>
                <span className={styles.techName}>React 19</span>
                <span className={styles.techDesc}>{t("pages.about.tech.react")}</span>
              </div>
              <div className={styles.techItem}>
                <span className={styles.techName}>Vite</span>
                <span className={styles.techDesc}>{t("pages.about.tech.vite")}</span>
              </div>
              <div className={styles.techItem}>
                <span className={styles.techName}>TypeScript</span>
                <span className={styles.techDesc}>{t("pages.about.tech.ts")}</span>
              </div>
              <div className={styles.techItem}>
                <span className={styles.techName}>CSS Modules</span>
                <span className={styles.techDesc}>{t("pages.about.tech.css")}</span>
              </div>
            </div>
          </div>
        </section>

        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>
            <FiTrendingUp className={styles.icon} /> {t("pages.about.roadmap.title")}
          </h2>
          <div className={styles.card}>
            <p className={styles.text}>
              {t("pages.about.roadmap.desc", { appName: APP_NAME })}
            </p>
            <ul className={styles.featureList}>
              <li>{t("pages.about.roadmap.collab")}</li>
              <li>{t("pages.about.roadmap.analytics")}</li>
              <li>{t("pages.about.roadmap.integrations")}</li>
              <li>{t("pages.about.roadmap.custom")}</li>
            </ul>
          </div>
        </section>
      </div>
    </div>
  );
}
