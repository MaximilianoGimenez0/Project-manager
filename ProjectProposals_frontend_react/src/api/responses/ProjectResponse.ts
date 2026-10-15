import type { UsersResponse } from "@/api/responses/UsersResponse";
import type { GenericResponse } from "@/api/responses/GenericResponse";
import type { ApprovalStepResponse } from "@/api/responses/ApprovalStepResponse";

export interface ProjectResponse {
    id: string;
    title: string;
    description: string;
    amount: number;
    duration: number;
    user: UsersResponse | null;
    area: GenericResponse;
    status: GenericResponse;
    type: GenericResponse;
    steps: ApprovalStepResponse[] | null;
}