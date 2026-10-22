import { apiClient } from "@/api/client";
import { API_ENDPOINTS } from "@/api/endpoints";
import type { LoginRequest } from "@/api/requests/LoginRequest";
import type { AuthResponse } from "@/api/responses/AuthResponse";
import type { UsersResponse } from "@/api/responses/UsersResponse";
import { mapUser } from "@/mappers/UserMapper";
import type { User } from "@/models";
import {
    getRefreshToken,
    setAuthSession,
    setTokens,
    setStoredUser,
    clearAuthSession,
} from "@/utils/tokenStorage";

export async function loginAsync(credentials: LoginRequest): Promise<User> {
    const response = await apiClient<AuthResponse>(API_ENDPOINTS.AUTH_LOGIN, {
        method: "POST",
        body: JSON.stringify(credentials),
    });

    const user = mapUser(response.user);
    setAuthSession(response.accessToken, response.refreshToken, user);
    return user;
}

export async function refreshTokenAsync(refreshToken: string): Promise<AuthResponse> {
    const response = await apiClient<AuthResponse>(API_ENDPOINTS.AUTH_REFRESH_TOKEN, {
        method: "POST",
        body: JSON.stringify({ refreshToken }),
    });

    setTokens(response.accessToken, response.refreshToken);
    return response;
}

export async function revokeTokenAsync(refreshToken: string): Promise<void> {
    await apiClient<{ message: string }>(API_ENDPOINTS.AUTH_REVOKE_TOKEN, {
        method: "POST",
        body: JSON.stringify({ refreshToken }),
    });
}

export async function logoutAsync(): Promise<void> {
    const refreshToken = getRefreshToken();
    if (refreshToken) {
        try {
            await revokeTokenAsync(refreshToken);
        } catch {
            // Clear local session even if server-side token revocation fails
        }
    }
    clearAuthSession();
}

export async function getCurrentUserAsync(): Promise<User> {
    const response = await apiClient<UsersResponse>(API_ENDPOINTS.AUTH_ME);
    const user = mapUser(response);
    setStoredUser(user);
    return user;
}
