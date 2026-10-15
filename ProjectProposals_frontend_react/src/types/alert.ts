export type AlertType = "success" | "error" | "warning" | "info";

export interface AlertData {
    id: string;
    type: AlertType;
    message: string;
    duration?: number;
}
