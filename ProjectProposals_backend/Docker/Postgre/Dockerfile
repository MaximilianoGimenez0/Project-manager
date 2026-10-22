ARG PG_VERSION=18-alpine
FROM postgres:${PG_VERSION}

LABEL maintainer="maxxi"
LABEL description="PostgreSQL 18 optimizado con extensiones iniciales y monitoreo de salud"

ENV POSTGRES_USER=maxxi \
    POSTGRES_DB=projectapproval \
    PGDATA=/var/lib/postgresql/data/pgdata \
    LANG=C.UTF-8 \
    TZ=UTC

RUN mkdir -p /var/lib/postgresql/data/pgdata && \
    chown -R postgres:postgres /var/lib/postgresql/data

COPY ./init-scripts/ /docker-entrypoint-initdb.d/

VOLUME ["/var/lib/postgresql/data"]

# Port 5432 is container-internal; compose binds host port 5532 to prevent local PostgreSQL conflicts
EXPOSE 5432

HEALTHCHECK --interval=10s --timeout=5s --start-period=15s --retries=5 \
    CMD pg_isready -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -h localhost || exit 1

CMD ["postgres", "-c", "max_connections=100", "-c", "shared_buffers=256MB", "-c", "work_mem=16MB", "-c", "timezone=UTC"]
