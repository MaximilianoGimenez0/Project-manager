import type { User } from "@/models/User";
import type { Status } from "@/models/Status";
import type { Role } from "@/models/Role";

export interface ApprovalStep {
    id: number;
    stepOrder: number;
    decisionDate: Date | null;
    observations: string | null;
    approverUser: User | null;
    approverRole: Role;
    status: Status;
}
