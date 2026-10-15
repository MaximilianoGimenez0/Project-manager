import type { Project } from "@/models/Project";
import { mapUser } from "./UserMapper";
import { mapStatus } from "./StatusMapper";
import type { ProjectResponse } from "@/api/responses/ProjectResponse";
import { mapApprovalStep } from "./ApprovalStepMapper";
import { mapArea } from "./AreaMapper";
import { mapType } from "./TypeMapper";

export function mapProject(
    response: ProjectResponse
): Project {
    return {
        id: response.id,
        title: response.title,
        description: response.description,
        amount: response.amount,
        duration: response.duration,
        user: response.user != null ? mapUser(response.user) : null,
        area: mapArea(response.area),
        status: mapStatus(response.status),
        type: mapType(response.type),
        steps: response.steps?.map(mapApprovalStep) ?? null,
    };
}