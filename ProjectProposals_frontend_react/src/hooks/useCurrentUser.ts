import { useState, useEffect, useCallback } from "react";
import { logoutAsync } from "@/services/authService";
import type { User } from "@/models";
import {
    getStoredUser,
    setStoredUser,
    clearAuthSession,
    hasValidSession,
    USER_CHANGED_EVENT,
} from "@/utils/tokenStorage";

export function useCurrentUser() {
    const [currentUser, setCurrentUserState] = useState<User | null>(() => getStoredUser());
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(() => hasValidSession() && getStoredUser() !== null);

    useEffect(() => {
        function handleUserChanged() {
            const user = getStoredUser();
            setCurrentUserState(user);
            setIsAuthenticated(hasValidSession() && user !== null);
        }

        window.addEventListener(USER_CHANGED_EVENT, handleUserChanged);
        return () => {
            window.removeEventListener(USER_CHANGED_EVENT, handleUserChanged);
        };
    }, []);

    const setCurrentUser = useCallback((user: User | null) => {
        if (user) {
            setStoredUser(user);
            setCurrentUserState(user);
            setIsAuthenticated(true);
        } else {
            clearAuthSession();
            setCurrentUserState(null);
            setIsAuthenticated(false);
        }
    }, []);

    const logout = useCallback(async () => {
        await logoutAsync();
        setCurrentUserState(null);
        setIsAuthenticated(false);
    }, []);

    return {
        currentUser,
        setCurrentUser,
        hasUser: currentUser !== null,
        isAuthenticated,
        logout,
    };
}
