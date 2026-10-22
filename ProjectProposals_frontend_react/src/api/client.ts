import { API_BASE_URL } from "@/config";
import { ApiError } from "@/errors/ApiError";
import type { ApiErrorResponse } from "@/api/responses/ApiErrorResponse";
import {
    getAccessToken,
    getRefreshToken,
    setTokens,
    clearAuthSession,
} from "@/utils/tokenStorage";

interface ApiRequestOptions extends RequestInit {
    _retry?: boolean;
}

let refreshPromise: Promise<string | null> | null = null;

async function executeRefreshToken(baseUrl: string): Promise<string | null> {
    const currentRefreshToken = getRefreshToken();
    if (!currentRefreshToken) {
        return null;
    }

    try {
        const response = await fetch(`${baseUrl}/auth/refresh-token`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ refreshToken: currentRefreshToken }),
        });

        if (!response.ok) {
            return null;
        }

        const data = await response.json();
        if (data.accessToken && data.refreshToken) {
            setTokens(data.accessToken, data.refreshToken);
            return data.accessToken as string;
        }

        return null;
    } catch {
        return null;
    }
}

export async function apiClient<T>(
    endpoint: string,
    options?: ApiRequestOptions
): Promise<T> {
    const baseUrl = API_BASE_URL.replace(/\/+$/, "");
    const cleanEndpoint = endpoint.startsWith("/") ? endpoint : `/${endpoint}`;

    const headers: Record<string, string> = {
        "Content-Type": "application/json",
        ...(options?.headers as Record<string, string> | undefined),
    };

    const token = getAccessToken();
    if (token && !headers["Authorization"]) {
        headers["Authorization"] = `Bearer ${token}`;
    }

    const response = await fetch(`${baseUrl}${cleanEndpoint}`, {
        ...options,
        headers,
    });

    const isAuthEndpoint =
        cleanEndpoint.includes("/auth/login") ||
        cleanEndpoint.includes("/auth/refresh-token");

    if (response.status === 401 && !options?._retry && !isAuthEndpoint) {
        if (!refreshPromise) {
            refreshPromise = executeRefreshToken(baseUrl).finally(() => {
                refreshPromise = null;
            });
        }

        const newAccessToken = await refreshPromise;

        if (newAccessToken) {
            return apiClient<T>(cleanEndpoint, {
                ...options,
                _retry: true,
                headers: {
                    ...headers,
                    Authorization: `Bearer ${newAccessToken}`,
                },
            });
        }

        clearAuthSession();
        throw new ApiError("Session expired. Please log in again.", 401);
    }

    if (!response.ok) {
        let errorMessage = `HTTP Error ${response.status}`;
        try {
            const errorData: ApiErrorResponse = await response.json();
            if (errorData.message) {
                errorMessage = errorData.message;
            }
        } catch {
            errorMessage = response.statusText || errorMessage;
        }

        throw new ApiError(errorMessage, response.status);
    }

    if (response.status === 204) {
        return undefined as T;
    }

    const contentType = response.headers.get("content-type");
    if (contentType && contentType.includes("application/json")) {
        return response.json() as Promise<T>;
    }

    return undefined as T;
}