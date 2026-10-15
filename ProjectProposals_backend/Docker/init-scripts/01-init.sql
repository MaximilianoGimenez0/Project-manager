-- ==============================================================================
-- Script de Inicialización Automática de PostgreSQL
-- Este archivo es ejecutado por el entrypoint oficial de PostgreSQL
-- ÚNICAMENTE durante la primera creación del contenedor (cuando PGDATA está vacío).
-- ==============================================================================

-- 1. uuid-ossp: Soporte para generación de UUIDs nativos (gen_random_uuid() / uuid_generate_v4())
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- 2. pg_trgm: Índices trigram para búsquedas de texto eficientes y similitud difusa (fuzzy search)
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- 3. unaccent: Permite realizar búsquedas ignorando acentos y caracteres diacríticos
CREATE EXTENSION IF NOT EXISTS "unaccent";

-- Mensaje de confirmación emitido a los logs estándar del contenedor
DO $$
BEGIN
    RAISE NOTICE '>>> Inicialización completada: Extensiones uuid-ossp, pg_trgm y unaccent activadas exitosamente.';
END $$;
