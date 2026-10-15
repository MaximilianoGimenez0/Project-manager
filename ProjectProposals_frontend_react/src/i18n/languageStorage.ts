import type { Language } from './types';

const STORAGE_KEY = 'app_language';
const DEFAULT_LANGUAGE: Language = 'es';

export const getStoredLanguage = (): Language => {
  try {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (stored === 'es' || stored === 'en' || stored === 'pt') {
      return stored as Language;
    }
  } catch (error) {
    console.error('Failed to access localStorage for language', error);
  }
  return DEFAULT_LANGUAGE;
};

export const setStoredLanguage = (lang: Language): void => {
  try {
    localStorage.setItem(STORAGE_KEY, lang);
  } catch (error) {
    console.error('Failed to save language to localStorage', error);
  }
};
