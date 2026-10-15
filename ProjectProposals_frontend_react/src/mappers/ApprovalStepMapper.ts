import type { ApprovalStepResponse } from "@/api/responses/ApprovalStepResponse";
import type { ApprovalStep } from "@/models/ApprovalStep";
import { mapUser } from "./UserMapper";
import { mapRole } from "./RoleMapper";
import { mapStatus } from "./StatusMapper";

export function mapApprovalStep(
    response: ApprovalStepResponse
): ApprovalStep {
    return {
        id: response.id,
        stepOrder: response.stepOrder,
        decisionDate: response.decisionDate ? new Date(response.decisionDate) : null,
        observations: response.observations,
        approverUser: response.approverUser
            ? mapUser(response.approverUser)
            : null,
        approverRole: mapRole(response.approverRole),
        status: mapStatus(response.status),
    };
}