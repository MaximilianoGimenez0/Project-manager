import { apiClient } from "@/api/client";
import { API_ENDPOINTS } from "@/api/endpoints";
import type { UsersResponse } from "@/api/responses/UsersResponse";
import { CACHE_KEYS } from "@/config";
import { mapUser } from "@/mappers/UserMapper";
import type { User } from "@/models";
import { getStoredItem, setStoredItem } from "@/utils/storage";

export async function getUsersAsync(): Promise<User[]> {
    const cached = getStoredItem<User[] | null>(CACHE_KEYS.USERS, null);
    if (cached) {
        return cached;
    }

    const response = await apiClient<UsersResponse[]>(API_ENDPOINTS.USERS);
    const users = response.map(mapUser);
    setStoredItem(CACHE_KEYS.USERS, users);
    return users;
}
