import type { GenericResponse } from "@/api/responses/GenericResponse";
import type { Type } from "@/models";

export function mapType(
    response: GenericResponse
): Type {

    return {
        id: response.id,
        name: response.name,
    };
}