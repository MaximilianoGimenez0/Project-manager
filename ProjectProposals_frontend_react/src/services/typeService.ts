import { apiClient } from "@/api/client";
import { API_ENDPOINTS } from "@/api/endpoints";
import type { GenericResponse } from "@/api/responses/GenericResponse";
import { CACHE_KEYS } from "@/config";
import { mapType } from "@/mappers/TypeMapper";
import type { Type } from "@/models";
import { getStoredItem, setStoredItem } from "@/utils/storage";

export async function getTypesAsync(): Promise<Type[]> {
    const cached = getStoredItem<Type[] | null>(CACHE_KEYS.TYPES, null);
    if (cached) {
        return cached;
    }

    const response = await apiClient<GenericResponse[]>(API_ENDPOINTS.PROJECT_TYPES);
    const types = response.map(mapType);
    setStoredItem(CACHE_KEYS.TYPES, types);
    return types;
}
