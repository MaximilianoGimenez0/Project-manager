import type { GenericResponse } from "@/api/responses/GenericResponse";
import type { Status } from "@/models";

export function mapStatus(
    response: GenericResponse
): Status {

    return {
        id: response.id,
        name: response.name,
    };
}