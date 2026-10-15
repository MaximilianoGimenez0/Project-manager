import { useState, useRef, useEffect } from "react";
import { FiGlobe, FiChevronDown, FiCheck } from "react-icons/fi";
import { useTranslation } from "@/hooks";
import type { Language } from "@/i18n";
import styles from "./LanguageSelector.module.css";

const languages: { code: Language; label: string; flag: string }[] = [
  { code: 'es', label: 'Español', flag: '🇪🇸' },
  { code: 'en', label: 'English', flag: '🇺🇸' },
  { code: 'pt', label: 'Português', flag: '🇧🇷' },
];

export default function LanguageSelector() {
  const { language, setLanguage, t } = useTranslation();
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  useEffect(() => {
    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape" && isOpen) {
        setIsOpen(false);
      }
    }
    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, [isOpen]);

  function handleSelect(lang: Language) {
    setLanguage(lang);
    setIsOpen(false);
  }

  const currentLang = languages.find((l) => l.code === language) || languages[0];

  return (
    <div className={styles.container} ref={dropdownRef}>
      <button
        type="button"
        className={styles.button}
        onClick={() => setIsOpen(!isOpen)}
        aria-expanded={isOpen}
        aria-haspopup="listbox"
        aria-label={t('navbar.language')}
        title={t('navbar.language')}
      >
        <FiGlobe className={styles.icon} aria-hidden="true" />
        <span className={styles.code}>{currentLang.code.toUpperCase()}</span>
        <FiChevronDown
          className={`${styles.chevron} ${isOpen ? styles.chevronOpen : ""}`}
          aria-hidden="true"
        />
      </button>

      {isOpen && (
        <div className={styles.dropdownMenu}>
          <div className={styles.list} role="listbox" aria-label={t('navbar.language')}>
            {languages.map((lang) => (
              <button
                key={lang.code}
                type="button"
                role="option"
                aria-selected={language === lang.code}
                className={styles.option}
                onClick={() => handleSelect(lang.code)}
              >
                <span className={styles.optionFlag} aria-hidden="true">{lang.flag}</span>
                <span className={styles.optionLabel}>{lang.label}</span>
                {language === lang.code && (
                  <FiCheck className={styles.check} aria-hidden="true" />
                )}
              </button>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
