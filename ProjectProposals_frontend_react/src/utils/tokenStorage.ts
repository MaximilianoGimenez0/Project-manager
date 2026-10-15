import type { User } from "@/models";

const ACCESS_TOKEN_KEY = "accessToken";
const REFRESH_TOKEN_KEY = "refreshToken";
const CURRENT_USER_KEY = "currentUser";
export const USER_CHANGED_EVENT = "userChanged";

export function getAccessToken(): string | null {
    try {
        return localStorage.getItem(ACCESS_TOKEN_KEY);
    } catch {
        return null;
    }
}

export function getRefreshToken(): string | null {
    try {
        return localStorage.getItem(REFRESH_TOKEN_KEY);
    } catch {
        return null;
    }
}

export function setTokens(accessToken: string, refreshToken: string): void {
    try {
        localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
        localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
    } catch {
        // Storage might be unavailable
    }
}

export function clearTokens(): void {
    try {
        localStorage.removeItem(ACCESS_TOKEN_KEY);
        localStorage.removeItem(REFRESH_TOKEN_KEY);
    } catch {
        // Safe ignore
    }
}

export function getStoredUser(): User | null {
    try {
        const item = localStorage.getItem(CURRENT_USER_KEY) ?? sessionStorage.getItem(CURRENT_USER_KEY);
        if (!item || item === "undefined") {
            return null;
        }
        return JSON.parse(item) as User;
    } catch {
        return null;
    }
}

export function setStoredUser(user: User): void {
    try {
        const serialized = JSON.stringify(user);
        localStorage.setItem(CURRENT_USER_KEY, serialized);
        sessionStorage.setItem(CURRENT_USER_KEY, serialized);
    } catch {
        // Safe ignore
    }
}

export function clearAuthSession(): void {
    clearTokens();
    try {
        localStorage.removeItem(CURRENT_USER_KEY);
        sessionStorage.removeItem(CURRENT_USER_KEY);
    } catch {
        // Safe ignore
    }
    window.dispatchEvent(new Event(USER_CHANGED_EVENT));
}

export function setAuthSession(accessToken: string, refreshToken: string, user: User): void {
    setTokens(accessToken, refreshToken);
    setStoredUser(user);
    window.dispatchEvent(new Event(USER_CHANGED_EVENT));
}

export function hasValidSession(): boolean {
    return Boolean(getAccessToken());
}
