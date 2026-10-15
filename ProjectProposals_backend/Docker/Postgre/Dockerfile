# ==============================================================================
# Dockerfile - PostgreSQL 18
# Basado en Alpine Linux para máxima ligereza, seguridad y rendimiento
# ==============================================================================

# 1. Imagen base: se permite sobrescribir la versión mediante ARG durante el build
ARG PG_VERSION=18-alpine
FROM postgres:${PG_VERSION}

# 2. Metadatos informativos de la imagen
LABEL maintainer="maxxi"
LABEL description="PostgreSQL 18 optimizado con extensiones iniciales y monitoreo de salud"

# 3. Variables de entorno iniciales para el motor:
# - POSTGRES_USER: Superusuario administrador por defecto
# - POSTGRES_DB: Nombre de la base de datos creada en el primer inicio
# - PGDATA: Subcarpeta dentro del volumen montado (aisla los datos de posibles metadatos del filesystem)
# - LANG / TZ: Configuración regional UTF-8 y zona horaria UTC
ENV POSTGRES_USER=maxxi \
    POSTGRES_DB=projectapproval \
    PGDATA=/var/lib/postgresql/data/pgdata \
    LANG=C.UTF-8 \
    TZ=UTC

# 4. Asegura la estructura de directorios y los permisos correctos para el usuario del sistema 'postgres'
RUN mkdir -p /var/lib/postgresql/data/pgdata && \
    chown -R postgres:postgres /var/lib/postgresql/data

# 5. Copia los scripts SQL de inicialización que se ejecutan automáticamente solo la primera vez que se crea la base
COPY ./init-scripts/ /docker-entrypoint-initdb.d/

# 6. Declara el punto de montaje persistente
VOLUME ["/var/lib/postgresql/data"]

# 7. Expone el puerto TCP estándar interno donde escucha el proceso de PostgreSQL
# NOTA: Internamente el motor siempre escucha en 5432. El mapeo externo a 5532 se configura en docker-compose / docker run.
EXPOSE 5432

# 8. Healthcheck: comprueba cada 10s si PostgreSQL está listo para responder consultas
HEALTHCHECK --interval=10s --timeout=5s --start-period=15s --retries=5 \
    CMD pg_isready -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -h localhost || exit 1

# 9. Comando de arranque con parámetros ajustados de memoria, conexiones y zona horaria
CMD ["postgres", "-c", "max_connections=100", "-c", "shared_buffers=256MB", "-c", "work_mem=16MB", "-c", "timezone=UTC"]
