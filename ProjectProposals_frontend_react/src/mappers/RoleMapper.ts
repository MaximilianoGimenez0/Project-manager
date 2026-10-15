import type { GenericResponse } from "@/api/responses/GenericResponse";
import type { Role } from "@/models";

export function mapRole(response: GenericResponse): Role {
    return {
        id: response.id,
        name: response.name,
    };
}