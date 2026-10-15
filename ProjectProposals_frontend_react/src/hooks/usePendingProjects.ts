import { useState, useEffect, useCallback } from "react";
import type { Project } from "@/models";
import { getRolePendingProjects } from "@/services";
import { useCurrentUser } from "./useCurrentUser";

export function usePendingProjects() {
    const { currentUser } = useCurrentUser();
    const [projects, setProjects] = useState<Project[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    const fetchProjects = useCallback(async () => {
        if (!currentUser?.role?.id) {
            setProjects([]);
            setIsLoading(false);
            return;
        }

        try {
            setIsLoading(true);
            setError(null);
            const pendingProjects = await getRolePendingProjects(currentUser.role.id);
            setProjects(pendingProjects);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Error fetching pending projects");
            setProjects([]);
        } finally {
            setIsLoading(false);
        }
    }, [currentUser]);

    useEffect(() => {
        // eslint-disable-next-line react-hooks/set-state-in-effect
        void fetchProjects();
    }, [fetchProjects]);

    return {
        projects,
        isLoading,
        error,
        refresh: fetchProjects,
    };
}
