import { useState, useEffect, useCallback } from "react";
import type { ReactNode } from "react";
import type { Language, TranslationKey } from "./types";
import { LanguageContext } from "./context";
import { getStoredLanguage, setStoredLanguage } from "./languageStorage";
import { es } from "./locales/es";
import { en } from "./locales/en";
import { pt } from "./locales/pt";

const dictionaries = {
  es,
  en,
  pt,
};

interface LanguageProviderProps {
  children: ReactNode;
}

export function LanguageProvider({ children }: LanguageProviderProps) {
  const [language, setLanguageState] = useState<Language>(getStoredLanguage);

  useEffect(() => {
    setStoredLanguage(language);
    document.documentElement.lang = language;
  }, [language]);

  const setLanguage = useCallback((lang: Language) => {
    setLanguageState(lang);
  }, []);

  const getNestedTranslation = (
    lang: Language,
    key: string
  ): string | undefined => {
    const keys = key.split(".");
    let current: unknown = dictionaries[lang];
    for (const k of keys) {
      if (current === undefined || current === null || typeof current !== "object") {
        return undefined;
      }
      current = (current as Record<string, unknown>)[k];
    }
    return typeof current === "string" ? current : undefined;
  };

  const t = useCallback((key: TranslationKey, params?: Record<string, string | number>): string => {
    let translation = getNestedTranslation(language, key);

    if (translation === undefined && language === "pt") {
      translation = getNestedTranslation("en", key);
    }
    if (translation === undefined && (language === "pt" || language === "en")) {
      translation = getNestedTranslation("es", key);
    }

    if (translation === undefined) {
      return key;
    }

    if (params) {
      return Object.entries(params).reduce((str, [paramKey, paramValue]) => {
        return str.replace(new RegExp(`{${paramKey}}`, "g"), String(paramValue));
      }, translation);
    }

    return translation;
  }, [language]);

  return (
    <LanguageContext.Provider value={{ language, setLanguage, t }}>
      {children}
    </LanguageContext.Provider>
  );
}
