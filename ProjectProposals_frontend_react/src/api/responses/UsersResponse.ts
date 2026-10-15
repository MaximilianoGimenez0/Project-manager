import type { GenericResponse } from "@/api/responses/GenericResponse";

export interface UsersResponse {
    id: number;
    name: string;
    email: string | null;
    role: GenericResponse;
}