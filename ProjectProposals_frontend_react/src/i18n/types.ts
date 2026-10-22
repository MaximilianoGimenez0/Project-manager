export type Language = 'es' | 'en' | 'pt';

// Spanish locale defines the canonical schema structure for all translations
import type { es } from './locales/es';

export type TranslationSchema = typeof es;

// Flattens nested object keys into dot-separated paths ("a.b.c")
export type NestedKeyOf<ObjectType extends object> = {
  [Key in keyof ObjectType & (string | number)]: ObjectType[Key] extends object
  ? `${Key}.${NestedKeyOf<ObjectType[Key]>}`
  : `${Key}`;
}[keyof ObjectType & (string | number)];

export type TranslationKey = NestedKeyOf<TranslationSchema>;

export type TranslationDictionary = {
  [key: string]: string | TranslationDictionary;
};
