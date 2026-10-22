-- Native UUID generation support (gen_random_uuid() / uuid_generate_v4())
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Trigram indexing for text search and fuzzy matching
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Diacritic-insensitive text search support
CREATE EXTENSION IF NOT EXISTS "unaccent";

DO $$
BEGIN
    RAISE NOTICE '>>> Inicialización completada: Extensiones uuid-ossp, pg_trgm y unaccent activadas exitosamente.';
END $$;
