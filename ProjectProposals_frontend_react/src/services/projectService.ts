import { apiClient } from "@/api/client";
import { API_ENDPOINTS } from "@/api/endpoints";
import type { DecisionStepUpdateRequest } from "@/api/requests/DecisionStepUpdateRequest";
import type { ProjectFilterParams } from "@/api/params/ProjectFilterParams";
import type { ProjectProposalCreateRequest } from "@/api/requests/ProjectProposalCreateRequest";
import type { ProjectUpdateRequest } from "@/api/requests/ProjectUpdateRequest";
import type { ProjectResponse } from "@/api/responses/ProjectResponse";
import type { ProjectShortResponse } from "@/api/responses/ProjectShortResponse";
import { mapProject } from "@/mappers/ProjectMapper";
import type { Project } from "@/models";

export async function getProjectAsync(id: string): Promise<Project> {
    const response = await apiClient<ProjectResponse>(
        `${API_ENDPOINTS.PROJECTS}/${id}`
    );
    return mapProject(response);
}

export async function getRolePendingProjects(role: number): Promise<Project[]> {
    const response = await apiClient<ProjectResponse[]>(
        `${API_ENDPOINTS.PROJECTS}/rolepending/${role}`
    );
    return response.map(mapProject);
}

export async function updateProjectAsync(
    id: string,
    request: ProjectUpdateRequest
): Promise<Project> {
    const response = await apiClient<ProjectResponse>(
        `${API_ENDPOINTS.PROJECTS}/${id}`,
        {
            method: "PATCH",
            body: JSON.stringify(request),
        }
    );
    return mapProject(response);
}

export async function getAllProjectsAsync(): Promise<ProjectShortResponse[]> {
    return apiClient<ProjectShortResponse[]>(API_ENDPOINTS.PROJECTS);
}

export async function createProjectAsync(
    request: ProjectProposalCreateRequest
): Promise<Project> {
    const response = await apiClient<ProjectResponse>(
        API_ENDPOINTS.PROJECTS,
        {
            method: "POST",
            body: JSON.stringify(request),
        }
    );
    return mapProject(response);
}

export async function getFilteredProjectsAsync(
    filters: ProjectFilterParams
): Promise<ProjectShortResponse[]> {
    const params = new URLSearchParams();

    if (filters.title) {
        params.append("title", filters.title);
    }

    if (filters.status) {
        params.append("status", filters.status.toString());
    }

    if (filters.applicant) {
        params.append("applicant", filters.applicant.toString());
    }

    if (filters.approvalUser) {
        params.append("approvalUser", filters.approvalUser.toString());
    }

    const queryString = params.toString();
    const endpoint = queryString
        ? `${API_ENDPOINTS.PROJECTS}?${queryString}`
        : API_ENDPOINTS.PROJECTS;

    return apiClient<ProjectShortResponse[]>(endpoint);
}

export async function updateDecisionAsync(
    projectId: string,
    request: DecisionStepUpdateRequest
): Promise<Project> {
    const response = await apiClient<ProjectResponse>(
        `${API_ENDPOINTS.PROJECTS}/${projectId}/decision`,
        {
            method: "PATCH",
            body: JSON.stringify(request),
        }
    );
    return mapProject(response);
}
