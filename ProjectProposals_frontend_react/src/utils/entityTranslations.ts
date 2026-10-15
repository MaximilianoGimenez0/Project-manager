import type { TranslationKey } from "@/i18n/types";
import type { Area, Role, Status, Type } from "@/models";

type EntityInput<T> = T | number | string | null | undefined;

function extractEntityId(entity: unknown): number | null {
    if (entity === null || entity === undefined) return null;
    if (typeof entity === "number" && Number.isFinite(entity)) return entity;
    if (typeof entity === "object" && "id" in entity) {
        const id = (entity as { id: unknown }).id;
        if (typeof id === "number" && Number.isFinite(id)) return id;
        const parsed = Number(id);
        if (!isNaN(parsed) && parsed > 0) return parsed;
    }
    if (typeof entity === "string") {
        const parsed = Number(entity);
        if (!isNaN(parsed) && parsed > 0) return parsed;
    }
    return null;
}

function extractFallbackName(entity: unknown): string {
    if (typeof entity === "string") return entity;
    if (typeof entity === "object" && entity !== null && "name" in entity) {
        const name = (entity as { name: unknown }).name;
        if (typeof name === "string") return name;
    }
    return "";
}

export function formatRole(
    role: EntityInput<Role>,
    t: (key: TranslationKey) => string
): string {
    const id = extractEntityId(role);
    if (id) {
        return t(`entities.roles.${id}` as TranslationKey);
    }
    return extractFallbackName(role);
}

export function formatStatus(
    status: EntityInput<Status>,
    t: (key: TranslationKey) => string
): string {
    const id = extractEntityId(status);
    if (id) {
        return t(`entities.statuses.${id}` as TranslationKey);
    }
    return extractFallbackName(status);
}

export function formatArea(
    area: EntityInput<Area>,
    t: (key: TranslationKey) => string
): string {
    const id = extractEntityId(area);
    if (id) {
        return t(`entities.areas.${id}` as TranslationKey);
    }
    return extractFallbackName(area);
}

export function formatType(
    type: EntityInput<Type>,
    t: (key: TranslationKey) => string
): string {
    const id = extractEntityId(type);
    if (id) {
        return t(`entities.types.${id}` as TranslationKey);
    }
    return extractFallbackName(type);
}
