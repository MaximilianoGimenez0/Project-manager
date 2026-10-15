import { apiClient } from "@/api/client";
import { API_ENDPOINTS } from "@/api/endpoints";
import type { GenericResponse } from "@/api/responses/GenericResponse";
import { CACHE_KEYS } from "@/config";
import { mapStatus } from "@/mappers/StatusMapper";
import type { Status } from "@/models";
import { getStoredItem, setStoredItem } from "@/utils/storage";

export async function getStatusAsync(): Promise<Status[]> {
    const cached = getStoredItem<Status[] | null>(CACHE_KEYS.STATUSES, null);
    if (cached) {
        return cached;
    }

    const response = await apiClient<GenericResponse[]>(
        API_ENDPOINTS.APPROVAL_STATUSES
    );
    const statuses = response.map(mapStatus);
    setStoredItem(CACHE_KEYS.STATUSES, statuses);
    return statuses;
}
