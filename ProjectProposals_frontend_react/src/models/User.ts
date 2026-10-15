import type { Role } from "./Role";

export interface User {
    id: number | null;
    name: string | null;
    email: string | null;
    role: Role;
}