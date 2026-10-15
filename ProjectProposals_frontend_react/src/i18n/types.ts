export type Language = 'es' | 'en' | 'pt';

// This will be used as the single source of truth for the shape of all translations.
// We import from the Spanish translation as the default schema.
import type { es } from './locales/es';

export type TranslationSchema = typeof es;

// Magic type to flatten object keys into dot notation "a.b.c"
export type NestedKeyOf<ObjectType extends object> = {
  [Key in keyof ObjectType & (string | number)]: ObjectType[Key] extends object
  ? `${Key}.${NestedKeyOf<ObjectType[Key]>}`
  : `${Key}`;
}[keyof ObjectType & (string | number)];

export type TranslationKey = NestedKeyOf<TranslationSchema>;

export type TranslationDictionary = {
  [key: string]: string | TranslationDictionary;
};
