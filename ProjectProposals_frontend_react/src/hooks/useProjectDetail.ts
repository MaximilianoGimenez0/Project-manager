import { useState, useEffect, useCallback } from "react";

import { STATUS_IDS } from "@/config";

import { useCurrentUser } from "@/hooks/useCurrentUser";

import { getProjectAsync } from "@/services";

import type { Project, ApprovalStep, Role } from "@/models";

export function useProjectDetail(projectId: string | undefined) {
    const { currentUser } = useCurrentUser();
    const [project, setProject] = useState<Project | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const fetchProject = useCallback(async () => {
        if (!projectId) {
            setError("Project ID is missing");
            setIsLoading(false);
            return;
        }

        try {
            setIsLoading(true);
            setError(null);
            const data = await getProjectAsync(projectId);
            setProject(data);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Error fetching project detail");
            setProject(null);
        } finally {
            setIsLoading(false);
        }
    }, [projectId]);

    useEffect(() => {
        // eslint-disable-next-line react-hooks/set-state-in-effect
        void fetchProject();
    }, [fetchProject]);

    // Sorted steps by stepOrder
    const steps: ApprovalStep[] = project?.steps
        ? [...project.steps].sort((a, b) => a.stepOrder - b.stepOrder)
        : [];

    // Find the current pending step waiting for evaluation
    const nextPendingStep: ApprovalStep | null =
        steps.find((step) => step.status?.id === STATUS_IDS.PENDING) ?? null;

    const nextApproverRole: Role | null = nextPendingStep?.approverRole ?? null;

    // Check if workflow is finalized (Approved or Rejected)
    const isWorkflowCompleted: boolean =
        project?.status?.id === STATUS_IDS.APPROVED ||
        project?.status?.id === STATUS_IDS.REJECTED;

    // Determine if the logged-in user has the role required to decide on this pending step
    const canCurrentUserDecide: boolean = Boolean(
        currentUser?.role?.id &&
        nextPendingStep?.approverRole?.id &&
        currentUser.role.id === nextPendingStep.approverRole.id &&
        !isWorkflowCompleted
    );

    return {
        project,
        steps,
        nextPendingStep,
        nextApproverRole,
        isWorkflowCompleted,
        canCurrentUserDecide,
        isLoading,
        error,
        refresh: fetchProject,
    };
}
