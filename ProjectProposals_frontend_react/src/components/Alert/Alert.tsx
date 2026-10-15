import { useEffect } from "react";
import { FiCheckCircle, FiAlertCircle, FiInfo, FiX, FiAlertTriangle } from "react-icons/fi";
import type { AlertData } from "@/types";
import { useTranslation } from "@/hooks";
import styles from "./Alert.module.css";

interface AlertProps {
    alert: AlertData;
    onClose: (id: string) => void;
}

const icons = {
    success: <FiCheckCircle />,
    error: <FiAlertCircle />,
    warning: <FiAlertTriangle />,
    info: <FiInfo />,
};

export default function Alert({ alert, onClose }: AlertProps) {
    const { t } = useTranslation();
    useEffect(() => {
        if (alert.duration) {
            const timer = setTimeout(() => {
                onClose(alert.id);
            }, alert.duration);
            return () => clearTimeout(timer);
        }
    }, [alert, onClose]);

    return (
        <div className={`${styles.alert} ${styles[alert.type]}`} role="alert">
            <div className={styles.iconContainer}>
                {icons[alert.type]}
            </div>
            <span className={styles.message}>{alert.message}</span>
            <button
                className={styles.closeButton}
                onClick={() => onClose(alert.id)}
                aria-label={t("common.closeAlert")}
            >
                <FiX />
            </button>
        </div>
    );
}
