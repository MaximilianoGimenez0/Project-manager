import type { AlertData, AlertType } from "@/types";

export const APP_ALERT_EVENT = "app-alert";

function generateId(): string {
    if (typeof crypto !== "undefined" && typeof crypto.randomUUID === "function") {
        return crypto.randomUUID();
    }
    return `${Date.now()}-${Math.random().toString(36).slice(2, 9)}`;
}

export function showAlert(type: AlertType, message: string, duration: number = 5000) {
    const alertData: AlertData = {
        id: generateId(),
        type,
        message,
        duration,
    };

    const event = new CustomEvent(APP_ALERT_EVENT, { detail: alertData });
    window.dispatchEvent(event);
}

export function showSuccess(message: string, duration?: number) {
    showAlert("success", message, duration);
}

export function showError(message: string, duration?: number) {
    showAlert("error", message, duration);
}

export function showWarning(message: string, duration?: number) {
    showAlert("warning", message, duration);
}

export function showInfo(message: string, duration?: number) {
    showAlert("info", message, duration);
}
