import { apiClient } from "@/api/client";
import { API_ENDPOINTS } from "@/api/endpoints";
import type { GenericResponse } from "@/api/responses/GenericResponse";
import { CACHE_KEYS } from "@/config";
import { mapArea } from "@/mappers/AreaMapper";
import type { Area } from "@/models";
import { getStoredItem, setStoredItem } from "@/utils/storage";

export async function getAreasAsync(): Promise<Area[]> {
    const cached = getStoredItem<Area[] | null>(CACHE_KEYS.AREAS, null);
    if (cached) {
        return cached;
    }

    const response = await apiClient<GenericResponse[]>(API_ENDPOINTS.AREAS);
    const areas = response.map(mapArea);
    setStoredItem(CACHE_KEYS.AREAS, areas);
    return areas;
}
