import type { GenericResponse } from "@/api/responses/GenericResponse";
import type { Area } from "@/models";

export function mapArea(
    response: GenericResponse
): Area {

    return {
        id: response.id,
        name: response.name,
    };
}