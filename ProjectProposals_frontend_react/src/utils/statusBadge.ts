import { STATUS_IDS } from "@/config";

export type BadgeVariant = "primary" | "secondary" | "success" | "warning" | "error" | "info";

/**
 * Returns the semantic badge variant for a given proposal status (numeric ID or string name).
 * - Approved (2 / "aprobado" / "approved") -> "success" (Emerald)
 * - Rejected (3 / "rechazado" / "rejected") -> "error" (Red)
 * - Pending (1 / "pendiente" / "pending") -> "warning" (Amber)
 * - Observed (4 / "observado" / "observed") -> "info" (Sky)
 */
export function getStatusBadgeVariant(status?: number | string | null): BadgeVariant {
    if (status === null || status === undefined) {
        return "secondary";
    }

    if (typeof status === "number") {
        switch (status) {
            case STATUS_IDS.APPROVED:
                return "success";
            case STATUS_IDS.REJECTED:
                return "error";
            case STATUS_IDS.PENDING:
                return "warning";
            case STATUS_IDS.OBSERVED:
                return "info";
            default:
                return "secondary";
        }
    }

    const trimmed = String(status).trim().toLowerCase();

    // Check if string is a numeric string (e.g. "2")
    const numeric = Number(trimmed);
    if (!isNaN(numeric) && trimmed !== "") {
        return getStatusBadgeVariant(numeric);
    }

    if (trimmed.includes("aprob") || trimmed.includes("approv")) {
        return "success";
    }
    if (trimmed.includes("rechaz") || trimmed.includes("reject")) {
        return "error";
    }
    if (trimmed.includes("pend") || trimmed.includes("wait")) {
        return "warning";
    }
    if (trimmed.includes("observ")) {
        return "info";
    }

    return "secondary";
}
