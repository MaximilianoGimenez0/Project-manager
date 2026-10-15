export const APP_NAME = import.meta.env.VITE_APP_NAME ?? "Project Manager";
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "";

export const COPYRIGHT_YEAR = new Date().getFullYear();

export const TABLET_BREAKPOINT = 768;

export const CACHE_KEYS = {
    AREAS: "areas",
    STATUSES: "statuses",
    TYPES: "types",
    USERS: "users",
} as const;

export const ROLE_IDS = {
    AREA_LEADER: 1,
    MANAGER: 2,
    DIRECTOR: 3,
    TECHNICAL_COMMITTEE: 4,
} as const;

export const STATUS_IDS = {
    PENDING: 1,
    APPROVED: 2,
    REJECTED: 3,
    OBSERVED: 4,
} as const;

export const AREA_IDS = {
    FINANCE: 1,
    TECHNOLOGY: 2,
    HUMAN_RESOURCES: 3,
    OPERATIONS: 4,
} as const;

export const PROJECT_TYPE_IDS = {
    PROCESS_IMPROVEMENT: 1,
    INNOVATION_AND_DEVELOPMENT: 2,
    INFRASTRUCTURE: 3,
    INTERNAL_TRAINING: 4,
} as const;

export const PROJECT_STATUS = STATUS_IDS;