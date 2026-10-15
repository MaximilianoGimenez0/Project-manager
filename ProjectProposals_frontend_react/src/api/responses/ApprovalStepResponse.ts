import type { UsersResponse } from "@/api/responses/UsersResponse";
import type { GenericResponse } from "@/api/responses/GenericResponse";

export interface ApprovalStepResponse {
    id: number;
    stepOrder: number;
    decisionDate: string | null;
    observations: string | null;
    approverUser: UsersResponse | null;
    approverRole: GenericResponse;
    status: GenericResponse;
}
