import type { Area } from "./Area";
import type { User } from "./User";
import type { ApprovalStep } from "./ApprovalStep";
import type { Status } from "./Status";
import type { Type } from "./Type";

export interface Project {
    id: string;
    title: string;
    description: string;
    amount: number;
    duration: number;
    user: User | null;
    area: Area;
    status: Status;
    type: Type;
    steps: ApprovalStep[] | null;
}