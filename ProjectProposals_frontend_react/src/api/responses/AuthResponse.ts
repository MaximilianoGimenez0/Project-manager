import type { UsersResponse } from "@/api/responses/UsersResponse";

export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    tokenType: string;
    expiresIn: number;
    user: UsersResponse;
}
