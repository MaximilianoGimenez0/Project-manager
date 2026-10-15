import { FiSun, FiMoon } from "react-icons/fi";
import { useTheme, useTranslation } from "@/hooks";
import styles from "./ThemeToggle.module.css";

export default function ThemeToggle() {
  const { isDark, toggleTheme } = useTheme();
  const { t } = useTranslation();

  const label = isDark ? t("common.lightTheme") : t("common.darkTheme");

  return (
    <button
      type="button"
      className={styles.themeToggleBtn}
      onClick={toggleTheme}
      aria-label={label}
      title={label}
    >
      <span className={styles.iconWrapper} aria-hidden="true">
        {isDark ? (
          <FiSun className={styles.sunIcon} />
        ) : (
          <FiMoon className={styles.moonIcon} />
        )}
      </span>
    </button>
  );
}
