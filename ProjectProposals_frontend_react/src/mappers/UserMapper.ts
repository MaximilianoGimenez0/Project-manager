import type { UsersResponse } from "@/api/responses/UsersResponse";
import type { User } from "@/models";
import { mapRole } from "@/mappers/RoleMapper";

export function mapUser(response: UsersResponse): User {
    return {
        id: response.id,
        name: response.name,
        email: response.email,
        role: mapRole(response.role),
    };
}