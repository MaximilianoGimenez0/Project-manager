import { useEffect, useState, useCallback } from "react";
import type { ProjectFilterParams } from "@/api";
import type { ProjectShortResponse } from "@/api";
import type { Status, User } from "@/models";
import { getFilteredProjectsAsync, getStatusAsync, getUsersAsync } from "@/services";

export function useProjectsFilter() {
    const [projects, setProjects] = useState<ProjectShortResponse[]>([]);
    const [statuses, setStatuses] = useState<Status[]>([]);
    const [users, setUsers] = useState<User[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        async function loadInitialData() {
            try {
                const [statusesData, usersData] = await Promise.all([
                    getStatusAsync(),
                    getUsersAsync(),
                ]);
                setStatuses(statusesData);
                setUsers(usersData);
            } catch (err) {
                setError(err instanceof Error ? err.message : "Failed to load filter options");
            }
        }

        void loadInitialData();
    }, []);

    const search = useCallback(async (filters: ProjectFilterParams) => {
        try {
            setIsLoading(true);
            setError(null);
            const data = await getFilteredProjectsAsync(filters);
            setProjects(data);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to search projects");
            setProjects([]);
        } finally {
            setIsLoading(false);
        }
    }, []);

    return {
        projects,
        statuses,
        users,
        isLoading,
        error,
        search,
    };
}