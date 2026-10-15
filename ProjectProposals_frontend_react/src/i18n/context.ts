import { createContext } from 'react';
import type { Language, TranslationKey } from './types';

export type LanguageContextType = {
  language: Language;
  setLanguage: (lang: Language) => void;
  t: (key: TranslationKey, params?: Record<string, string | number>) => string;
};

export const LanguageContext = createContext<LanguageContextType | undefined>(undefined);
