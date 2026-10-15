import { useEffect, useState, useCallback } from "react";
import { APP_ALERT_EVENT } from "@/utils";
import type { AlertData } from "@/types";
import Alert from "@/components/Alert";
import styles from "./AlertContainer.module.css";

export default function AlertContainer() {
    const [alerts, setAlerts] = useState<AlertData[]>([]);

    useEffect(() => {
        function handleAlert(event: Event) {
            const customEvent = event as CustomEvent<AlertData>;
            setAlerts((prev) => [...prev, customEvent.detail]);
        }

        window.addEventListener(APP_ALERT_EVENT, handleAlert);

        return () => {
            window.removeEventListener(APP_ALERT_EVENT, handleAlert);
        };
    }, []);

    const removeAlert = useCallback((id: string) => {
        setAlerts((prev) => prev.filter((alert) => alert.id !== id));
    }, []);

    if (alerts.length === 0) {
        return null;
    }

    return (
        <div className={styles.container}>
            {alerts.map((alert) => (
                <Alert key={alert.id} alert={alert} onClose={removeAlert} />
            ))}
        </div>
    );
}
