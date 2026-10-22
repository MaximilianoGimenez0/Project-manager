export function getStoredItem<T>(key: string, fallback: T): T {
    try {
        const item = sessionStorage.getItem(key);
        if (item === null || item === "undefined") {
            return fallback;
        }
        return JSON.parse(item) as T;
    } catch {
        return fallback;
    }
}

export function setStoredItem<T>(key: string, value: T): void {
    try {
        sessionStorage.setItem(key, JSON.stringify(value));
    } catch {
        // Storage access failure ignored
    }
}

export function removeStoredItem(key: string): void {
    try {
        sessionStorage.removeItem(key);
    } catch {
        // Storage access failure ignored
    }
}
