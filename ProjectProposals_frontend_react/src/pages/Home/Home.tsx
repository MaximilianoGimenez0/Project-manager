import { Link } from "react-router-dom";
import {
  FiArrowRight,
  FiCheckCircle,
  FiFileText,
  FiUsers,
  FiSearch,
  FiLayers,
} from "react-icons/fi";

import { APP_NAME } from "@/config";

import { useTranslation } from "@/hooks";

import styles from "./Home.module.css";

export default function Home() {
  const { t } = useTranslation();

  return (
    <div className={styles.pageContainer}>
      {/* Hero Section */}
      <section className={styles.heroSection}>
        <div className={styles.heroContent}>
          <h1 className={styles.heroTitle}>
            {t("pages.home.hero.title1")} <br /> {t("pages.home.hero.title2")}
          </h1>
          <p className={styles.heroSubtitle}>
            {t("pages.home.hero.subtitle", { appName: APP_NAME })}
          </p>
          <div className={styles.heroActions}>
            <Link to="/create-project" className={styles.primaryButton}>
              {t("pages.home.hero.btnCreate")} <FiArrowRight />
            </Link>
            <Link to="/search" className={styles.secondaryButton}>
              {t("pages.home.hero.btnExplore")}
            </Link>
          </div>
        </div>
        <div className={styles.heroVisual}>
          <div className={styles.heroVisualCard}>
            <div className={styles.previewHeader}>
              <span className={styles.previewBadge}>
                {t("pages.home.hero.preview.badge")}
              </span>
              <span className={styles.previewId}>
                {t("pages.home.hero.preview.code")}
              </span>
            </div>
            <div className={styles.previewBody}>
              <h3 className={styles.previewTitle}>
                {t("pages.home.hero.preview.title")}
              </h3>
              <p className={styles.previewDesc}>
                {t("pages.home.hero.preview.desc")}
              </p>

              <div className={styles.previewMetrics}>
                <div className={styles.previewMetric}>
                  <span className={styles.metricLabel}>
                    {t("pages.home.hero.preview.budgetLabel")}
                  </span>
                  <span className={styles.metricValue}>
                    {t("pages.home.hero.preview.budgetValue")}
                  </span>
                </div>
                <div className={styles.previewMetric}>
                  <span className={styles.metricLabel}>
                    {t("pages.home.hero.preview.durationLabel")}
                  </span>
                  <span className={styles.metricValue}>
                    {t("pages.home.hero.preview.durationValue")}
                  </span>
                </div>
                <div className={styles.previewMetric}>
                  <span className={styles.metricLabel}>
                    {t("pages.home.hero.preview.areaLabel")}
                  </span>
                  <span className={styles.metricValue}>
                    {t("pages.home.hero.preview.areaValue")}
                  </span>
                </div>
              </div>

              <div className={styles.previewSteps}>
                <div className={styles.previewStep}>
                  <span className={`${styles.stepDot} ${styles.stepDotCompleted}`}>✓</span>
                  <span className={styles.stepName}>
                    {t("pages.home.hero.preview.step1")}
                  </span>
                </div>
                <div className={styles.previewStep}>
                  <span className={`${styles.stepDot} ${styles.stepDotActive}`}>2</span>
                  <span className={styles.stepName}>
                    {t("pages.home.hero.preview.step2")}
                  </span>
                </div>
                <div className={styles.previewStep}>
                  <span className={styles.stepDot}>3</span>
                  <span className={styles.stepName}>
                    {t("pages.home.hero.preview.step3")}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Product Overview */}
      <section className={styles.overviewSection}>
        <div className={styles.sectionHeader}>
          <h2 className={styles.sectionTitle}>{t("pages.home.overview.title", { appName: APP_NAME })}</h2>
          <p className={styles.sectionSubtitle}>
            {t("pages.home.overview.subtitle")}
          </p>
        </div>
      </section>

      {/* Features Section */}
      <section className={styles.featuresSection}>
        <div className={styles.featuresGrid}>
          <div className={styles.featureCard}>
            <div className={styles.featureIcon}><FiFileText /></div>
            <h3 className={styles.featureTitle}>{t("pages.home.features.management.title")}</h3>
            <p className={styles.featureDescription}>{t("pages.home.features.management.desc")}</p>
          </div>
          <div className={styles.featureCard}>
            <div className={styles.featureIcon}><FiCheckCircle /></div>
            <h3 className={styles.featureTitle}>{t("pages.home.features.workflows.title")}</h3>
            <p className={styles.featureDescription}>{t("pages.home.features.workflows.desc")}</p>
          </div>
          <div className={styles.featureCard}>
            <div className={styles.featureIcon}><FiSearch /></div>
            <h3 className={styles.featureTitle}>{t("pages.home.features.search.title")}</h3>
            <p className={styles.featureDescription}>{t("pages.home.features.search.desc")}</p>
          </div>
          <div className={styles.featureCard}>
            <div className={styles.featureIcon}><FiUsers /></div>
            <h3 className={styles.featureTitle}>{t("pages.home.features.roles.title")}</h3>
            <p className={styles.featureDescription}>{t("pages.home.features.roles.desc")}</p>
          </div>
        </div>
      </section>

      {/* Workflow Section */}
      <section className={styles.workflowSection}>
        <h2 className={styles.sectionTitleCenter}>{t("pages.home.howItWorks.title")}</h2>
        <div className={styles.workflowSteps}>
          <div className={styles.step}>
            <div className={styles.stepNumber}>1</div>
            <h3 className={styles.stepTitle}>{t("pages.home.howItWorks.step1.title")}</h3>
            <p className={styles.stepDesc}>{t("pages.home.howItWorks.step1.desc")}</p>
          </div>
          <div className={styles.stepDivider}></div>
          <div className={styles.step}>
            <div className={styles.stepNumber}>2</div>
            <h3 className={styles.stepTitle}>{t("pages.home.howItWorks.step2.title")}</h3>
            <p className={styles.stepDesc}>{t("pages.home.howItWorks.step2.desc")}</p>
          </div>
          <div className={styles.stepDivider}></div>
          <div className={styles.step}>
            <div className={styles.stepNumber}>3</div>
            <h3 className={styles.stepTitle}>{t("pages.home.howItWorks.step3.title")}</h3>
            <p className={styles.stepDesc}>{t("pages.home.howItWorks.step3.desc")}</p>
          </div>
          <div className={styles.stepDivider}></div>
          <div className={styles.step}>
            <div className={styles.stepNumber}>4</div>
            <h3 className={styles.stepTitle}>{t("pages.home.howItWorks.step4.title")}</h3>
            <p className={styles.stepDesc}>{t("pages.home.howItWorks.step4.desc")}</p>
          </div>
          <div className={styles.stepDivider}></div>
          <div className={styles.step}>
            <div className={styles.stepNumber}>5</div>
            <h3 className={styles.stepTitle}>{t("pages.home.howItWorks.step5.title")}</h3>
            <p className={styles.stepDesc}>{t("pages.home.howItWorks.step5.desc")}</p>
          </div>
        </div>
      </section>

      {/* Tech Info Section */}
      <section className={styles.techSection}>
        <div className={styles.techContent}>
          <div className={styles.techIcon}><FiLayers /></div>
          <div>
            <h2 className={styles.techTitle}>{t("pages.home.tech.title")}</h2>
            <p className={styles.techDescription}>
              {t("pages.home.tech.desc")}
            </p>
          </div>
        </div>
      </section>
    </div>
  );
}