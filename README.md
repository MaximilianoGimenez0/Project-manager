# Project Proposals Management & Governance Ecosystem

> Sistema integral de gestión de proyectos institucionales. Integra un SPA con React 19, un motor transaccional de alta fidelidad en .NET 8 (Clean Architecture, CQRS) y persistencia relacional en PostgreSQL 18.

[![React](https://img.shields.io/badge/React-19.2.7-20232a?style=flat&logo=react)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-6.0.2-3178C6?style=flat&logo=typescript&logoColor=white)](https://www.typescriptlang.org/)
[![Vite](https://img.shields.io/badge/Vite-8.1.1-646CFF?style=flat&logo=vite&logoColor=white)](https://vitejs.dev/)
[![React Router](https://img.shields.io/badge/React_Router-7.18.1-CA4245?style=flat&logo=reactrouter&logoColor=white)](https://reactrouter.com/)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat&logo=csharp&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/)
[![Entity Framework Core](https://img.shields.io/badge/EF_Core-9.0.5-512BD4?style=flat&logo=dotnet&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-18_Alpine-4169E1?style=flat&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![MediatR](https://img.shields.io/badge/MediatR-14.2.0-FF6600?style=flat&logo=nuget&logoColor=white)](https://github.com/jbogard/MediatR)
[![Docker](https://img.shields.io/badge/Docker-Compose_v2-2496ED?style=flat&logo=docker&logoColor=white)](https://www.docker.com/)
[![Firebase](https://img.shields.io/badge/Firebase_Hosting-Edge-FFCA28?style=flat&logo=firebase&logoColor=black)](https://firebase.google.com/)

[Backend (.NET 8 Clean Architecture)](./ProjectProposals_backend) · [Frontend (React 19 SPA)](./ProjectProposals_frontend_react) · [Catálogo de Endpoints de la API](./ProjectProposals_backend/API_DOCUMENTATION.md) · [Live demo](https://project-manager.maxigimenez.com.ar/)

---

## Visión General & Propósito Institucional

**Project Proposals Ecosystem** resuelve esta problemática centralizando todo el ciclo de vida de una propuesta:
1. **Formulación Estandarizada:** Los miembros de la organización presentan proyectos con presupuesto estimado, plazo, área de impacto y fundamentación técnica.
2. **Generación Determinista de Aprobadores:** El motor de reglas interno calcula automáticamente la cadena de aprobación requerida en base al monto financiero, área y tipología, deduplicando roles y ordenándolos jerárquicamente.
3. **Gobernanza y Dictamen Colegiado:** Los miembros autorizados tienen acceso a la aprobación de iniciativas pendientes en un dashboard exclusivo, con la funcionalidad de emitir dictámenes vinculantes (*Aprobado*, *Rechazado* u *Observado*) con total trazabilidad temporal y firmas auditadas.

---

## Componentes del Ecosistema

| Módulo | Directorio | Tecnología Principal | Rol en la Solución |
| :--- | :--- | :--- | :--- |
| **Backend Engine & API** | [`/ProjectProposals_backend`](./ProjectProposals_backend) | .NET 8.0 (`net8.0`), C# 12, EF Core 9.0.5, PostgreSQL 18-alpine, MediatR 14.2.0, Docker Compose v2 | Núcleo transaccional, motor de reglas (`ApprovalStepEngine`), persistencia relacional y seguridad JWT con rotación de Refresh Tokens. |
| **Frontend Client SPA** | [`/ProjectProposals_frontend_react`](./ProjectProposals_frontend_react) | React 19.2.7 (lock 19.2.8), TypeScript 6.0.2, Vite 8.1.1, React Router 7.18.1, CSS Modules, Firebase | Interfaz de usuario de alta fidelidad, tablero de iniciativas pendientes, stepper de gobernanza, autenticación RBAC con perfiles demo e i18n nativo. |

---

## Flujo de Datos & Gobernanza Integral

El siguiente diagrama detalla la arquitectura de extremo a extremo, mostrando cómo la Single Page Application interactúa con el backend .NET 8 y la base de datos PostgreSQL:

```mermaid
flowchart TD
    subgraph USERS ["Roles de Usuario (RBAC)"]
        U1["Proponente / Solicitante<br/><code>Formulación de Iniciativas</code>"]
        U2["Evaluador / Aprobador<br/><code>Líder de Área | Comité Técnico | Gerente | Director</code>"]
    end

    subgraph FRONTEND ["Frontend SPA (React 19 + TypeScript + Vite)"]
        F1["Vistas: Login | Create | Pending | Detail | Search"]
        F2["Custom Hooks<br/><code>useProjectDetail | useMakeDecision | usePendingProjects</code>"]
        F3["Single-Flight Token Lock<br/><code>apiClient.ts (Interceptor 401 Concurrente)</code>"]
        F4["alertManager (DOM CustomEvent Toast Bus)"]
        F1 <--> F2
        F2 --> F3
        F3 -.-> F4
    end

    subgraph BACKEND_GATEWAY ["Backend Gateway & Seguridad (ASP.NET Core 8)"]
        B1["ForwardedHeaders & CORS<br/><code>Soporte Reverse Proxy (Nginx/Caddy)</code>"]
        B2["Authentication & Authorization<br/><code>JWT Bearer Validation (HMAC-SHA256)</code>"]
        B3["ExceptionHandlingMiddleware<br/><code>Respuestas JSON estandarizadas (400, 404, 409, 500)</code>"]
        B4["REST API Controllers<br/><code>Auth | Project | Catalogs</code>"]
        B1 --> B2 --> B3 --> B4
    end

    subgraph APPLICATION_CORE ["Casos de Uso & Lógica (Clean Architecture - MediatR)"]
        M1{"MediatR ISender"}
        C1["CreateProjectProposalCommandHandler"]
        C2["DecideApprovalStepCommandHandler"]
        Q1["Queries: GetPendingByRole, GetFiltered, GetById"]
        ENG["ApprovalStepEngine<br/><code>1. Filtro por Presupuesto [Min, Max]<br/>2. Especificidad S(r) = Area + Type<br/>3. Deduplicación de Roles<br/>4. Orden por Jerarquía H(r)</code>"]
        
        B4 --> M1
        M1 -->|"Comando Crear"| C1
        M1 -->|"Comando Dictaminar"| C2
        M1 -->|"Consultas"| Q1
        C1 --> ENG
    end

    subgraph DATABASE ["Persistencia Relacional (PostgreSQL 18 Alpine)"]
        DB1["ProjectApprovalDbContext<br/><code>Entity Framework Core 9 (Transacciones Atómicas)</code>"]
        DB2[("PostgreSQL Engine<br/><code>Proposals | ApprovalSteps | Rules | Roles | Users | RefreshTokens</code>")]
        C1 --> DB1
        C2 --> DB1
        Q1 --> DB1
        DB1 <--> DB2
    end

    U1 -->|"1. Formula Propuesta"| F1
    U2 -->|"4. Dictamina Paso Activo"| F1
    F3 -->|"HTTP JSON + Bearer Token"| B1
    DB2 -.->|"JSON DTO Response"| F1
```

---

### Ciclos Operativos del Sistema

```mermaid
sequenceDiagram
    autonumber
    actor Submitter as Solicitante
    actor Approver as Evaluador (Rol Activo)
    participant Front as Frontend (React 19)
    participant API as API Backend (.NET 8)
    participant Engine as ApprovalStepEngine
    participant DB as PostgreSQL 18

    Note over Submitter,DB: Ciclo 1: Formulación de Propuesta & Cálculo de Pasos
    Submitter->>Front: Completa título, justificación, monto y área
    Front->>API: POST /api/project (Payload JSON + JWT Bearer)
    API->>Engine: Evalúa reglas activas contra monto y especificidad S(r)
    Engine->>Engine: Deduplica roles y ordena por HierarchyLevel (10..30)
    Engine-->>API: Pasos secuenciales generados [StepOrder: 1..N, Status: Pending]
    API->>DB: Persiste Propuesta + Pasos vinculados (Transacción ACID)
    API-->>Front: 201 Created (ProjectResponse con grafo de pasos)

    Note over Approver,DB: Ciclo 2: Consulta de Pendientes & Dictamen Colegiado
    Approver->>Front: Accede a 'Proyectos Pendientes'
    Front->>API: GET /api/project/rolepending/{roleId}
    API->>DB: Filtra iniciativas con paso activo esperando al rol
    DB-->>API: Lista de iniciativas asignadas
    API-->>Front: 200 OK (Iniciativas prioritarias)
    Approver->>Front: Emite dictamen vinculante en DecisionModal (Aprobar/Rechazar)
    Front->>API: PATCH /api/project/{id}/decision (Acción + Observaciones)
    API->>DB: Valida que el paso corresponda al usuario y rol
    alt Aprobación (Approve)
        API->>DB: Marca paso actual como 'Approved'. ¿Hay más pasos? Avanza a paso i+1 o cierra en 'Approved'
    else Rechazo (Reject)
        API->>DB: Marca paso como 'Rejected' y cancela en cascada pasos posteriores (Propuesta: Rejected)
    end
    API-->>Front: 200 OK (Detalle actualizado y confirmación auditada)

    Note over Front,API: Ciclo 3: Manejo Concurrente de Sesión (Single-Flight Refresh)
    Front->>API: Petición paralela con Access Token expirado (401 Unauthorized)
    Front->>Front: Activa cerrojo Single-Flight (refreshPromise compartido)
    Front->>API: POST /api/auth/refresh-token (RefreshToken rotativo)
    API->>DB: Invalida token anterior, detecta posible reúso y guarda nuevo token
    API-->>Front: 200 OK (Nuevo AccessToken + Nuevo RefreshToken)
    Front->>API: Reintenta peticiones en vuelo transparentemente
```

---

## Puesta en Marcha

### Requisitos Previos
* **Docker Desktop / Docker Compose v2:** Para ejecutar el backend y la base de datos PostgreSQL de forma aislada.
* **Node.js:** Versión `20.x` LTS o superior (compatible con Node 22+).
* **Gestor de Paquetes:** `npm` (10+), `pnpm` o `yarn`.
* **Git:** Para control de versiones.
* *(Opcional)* **.NET 8 SDK:** Versión `8.0.100` o superior, requerida únicamente si se desea ejecutar el backend de forma nativa sin Docker.

---

### Configuración de Variables de Entorno

Antes de iniciar, genere los archivos de entorno locales a partir de sus respectivas plantillas:

```bash
# 1. Configurar variables del Backend
cp ProjectProposals_backend/.env.example ProjectProposals_backend/.env

# 2. Configurar variables del Frontend
cp ProjectProposals_frontend_react/.env.example ProjectProposals_frontend_react/.env
```

Ajuste las variables esenciales:
* **Backend (`ProjectProposals_backend/.env`):** Define `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB` y `JWT_SECRET_KEY`.
* **Frontend (`ProjectProposals_frontend_react/.env`):** Define `VITE_API_BASE_URL` apuntando a la URL del backend activo (ej. `http://localhost:8080/api` para Docker o `http://localhost:5242/api` para .NET CLI).

---

### Opción 1: Entorno Contenerizado (Docker Compose + Vite) — *Recomendada*

Esta modalidad aprovisiona la base de datos PostgreSQL y la API compilada dentro de la red interna de Docker, mientras el frontend se ejecuta en Node.js local con recarga en caliente (*Hot Module Replacement*).

#### Paso 1: Levantar Backend & Base de Datos con Docker
```bash
# 1. Navegar al backend
cd ProjectProposals_backend

# 2. Levantar servicios en segundo plano
docker compose up -d --build

# 3. Comprobar salud de los contenedores
docker compose ps
```
*La API quedará expuesta en `http://localhost:8080` (Swagger: `http://localhost:8080/swagger`) y PostgreSQL en el puerto `5532`.*

#### Paso 2: Iniciar el Frontend SPA
```bash
# 1. En una nueva terminal, navegar al directorio del frontend
cd ProjectProposals_frontend_react

# 2. Configurar la URL de conexión en .env
# VITE_API_BASE_URL=http://localhost:8080/api

# 3. Instalar dependencias e iniciar el servidor de desarrollo
npm install
npm run dev
```

La aplicación web estará accesible de inmediato en [http://localhost:5173](http://localhost:5173).

---

### Opción 2: Entorno Nativo (.NET CLI + Vite + PostgreSQL)

Para desarrolladores que requieran depuración paso a paso en Visual Studio o VS Code sobre el SDK nativo:

```bash
# 1. Asegurar una instancia de PostgreSQL activa y configurar ConnectionStrings en appsettings.json

# 2. Terminal 1: Compilar e iniciar la API .NET 8
cd ProjectProposals_backend
dotnet restore ProjectApproval.sln
dotnet run --project ProjectApproval.Api

# 3. Terminal 2: Iniciar el cliente React
cd ProjectProposals_frontend_react
# Configurar VITE_API_BASE_URL=http://localhost:5242/api en .env
npm install
npm run dev
```

---

## Topología de Servicios y Puertos Locales

| Servicio | Entorno | URL / Puerto Local | Credenciales por Defecto |
| :--- | :--- | :--- | :--- |
| **Frontend SPA** | React 19 (Vite) | `http://localhost:5173` | Ver tabla de perfiles demo |
| **Backend API (Docker)** | ASP.NET Core 8 | `http://localhost:8080` | Endpoint REST Bearer |
| **Swagger UI (Docker)** | OpenAPI 3.0 | `http://localhost:8080/swagger` | Interactivo con botón Authorize |
| **Backend API (Nativo)** | Kestrel (.NET CLI) | `http://localhost:5242` / `https://localhost:7084` | Endpoint REST Bearer |
| **PostgreSQL 18** | Contenedor Alpine | `localhost:5532` | User: `maxxi`, Pass: `MiClaveLocal123`, DB: `projectapproval` |

---

## Perfiles Demo & Credenciales de Prueba

Ambos proyectos incluyen perfiles precargados para testear todos los niveles del flujo de aprobación colegiado. En la pantalla de inicio de sesión (`/login`) se pueden seleccionar con un solo clic:

> **Contraseña unificada de prueba:** `Password123!`

| ID | Nombre | Correo Electrónico | Rol Institucional | Nivel Jerárquico $H(r)$ | Responsabilidad en el Dictamen |
| :---: | :--- | :--- | :--- | :---: | :--- |
| **1** | José Ferreyra | `jferreyra@unaj.com` | Gerente | 20 (Nivel 3) | Evaluación de viabilidad operativa y asignación presupuestaria intermedia. |
| **2** | Ana Lucero | `alucero@unaj.com` | Líder de Área | 10 (Nivel 1) | Primer dictamen de pertinencia temática en el área de origen. |
| **3** | Gonzalo Molinas | `gmolinas@unaj.com` | Gerente | 20 (Nivel 3) | Revisión de partidas y control gerencial. |
| **4** | Lucas Olivera | `lolivera@unaj.com` | Director | 30 (Nivel 4) | Dictamen final institucional vinculante para iniciativas de alto impacto. |
| **5** | Danilo Fagundez | `dfagundez@unaj.com` | Comité Técnico | 15 (Nivel 2) | Dictamen técnico sobre arquitectura, factibilidad e infraestructura. |
| **6** | Gabriel Galli | `ggalli@unaj.com` | Comité Técnico | 15 (Nivel 2) | Evaluación técnica y estándares de ingeniería. |
| **7** | Mariana Torres | `mtorres@unaj.com` | Líder de Área | 10 (Nivel 1) | Validación inicial de propuestas sectoriales. |
| **8** | Carlos Mendoza | `cmendoza@unaj.com` | Director | 30 (Nivel 4) | Aprobación estratégica en la máxima jerarquía. |

---

## Comparativa del Stack Tecnológico

| Capa del Sistema | Frontend Client (`ProjectProposals_frontend_react`) | Backend Engine (`ProjectProposals_backend`) |
| :--- | :--- | :--- |
| **Lenguaje & Core** | TypeScript `~6.0.2` (lock `6.0.3`), React `^19.2.7` (lock `19.2.8`) | C# 12.0, .NET SDK/Runtime `8.0` (`net8.0`) |
| **Arquitectura** | Component-Driven, Custom Hooks, Data Mappers | Clean Architecture, DDD táctico, CQRS con MediatR `14.2.0` |
| **Red & Integración** | Native Fetch Wrapper con Single-Flight Refresh Lock | ASP.NET Core Web API `8.0.x`, ForwardedHeadersOptions |
| **Persistencia & Datos** | `localStorage` (Tokens de sesión y preferencia de idioma) | PostgreSQL `18-alpine`, Entity Framework Core `9.0.5`, Npgsql `9.0.4` |
| **Seguridad & Auth** | Interceptor 401, Rutas protegidas (`ProtectedRoute`) | JwtBearer `8.0.13` (HMAC-SHA256, 15 min), Refresh Tokens (7 días), BCrypt.Net `4.0.3` |
| **Diseño & UI** | Vanilla CSS Modules (`*.module.css`), Variables HSL, React Icons `^5.7.0` | Swagger UI / Swashbuckle OpenAPI 3.0 `6.6.2` |
| **Internacionalización** | Sistema nativo trilingüe (`es`, `en`, `pt`) con `NestedKeyOf` | N/A (Respuestas tipadas estandarizadas) |
| **Calidad & Testing** | ESLint `^10.6.0` (lock `10.8.0`), TypeScript-ESLint `^8.62.0` (lock `8.65.0`) | xUnit `2.5.3`, Moq `4.21.0`, FluentAssertions `8.11.0` |
| **Despliegue & DevOps** | Firebase Hosting (Edge CDN con SSL administrado) | Docker Multi-stage, Docker Compose v2 |

---

## Autor & Contacto

* **Maximiliano Giménez**
* **Portfolio:** [maxigimenez.com.ar](https://maxigimenez.com.ar)
* **GitHub:** [@MaximilianoGimenez0](https://github.com/MaximilianoGimenez0)
* **LinkedIn:** [Maximiliano Giménez](https://www.linkedin.com/in/)
