import { useEffect, useRef } from "react";
import { FiX } from "react-icons/fi";
import type { DecisionStepUpdateRequest } from "@/api";
import { Button } from "@/components/ui";
import { useTranslation, useMakeDecision } from "@/hooks";
import styles from "./DecisionModal.module.css";

interface DecisionModalProps {
    isOpen: boolean;
    stepId: number;
    onClose: () => void;
    onDecision: (request: DecisionStepUpdateRequest) => Promise<void>;
}

export default function DecisionModal({ isOpen, stepId, onClose, onDecision }: DecisionModalProps) {
    const { t, translateStatus } = useTranslation();
    const modalRef = useRef<HTMLDivElement>(null);
    const previousFocusRef = useRef<HTMLElement | null>(null);

    const {
        statuses,
        form,
        errors,
        isSubmitting,
        handleChange,
        handleSubmit,
        isValid,
    } = useMakeDecision({ stepId, onSubmit: onDecision });

    // Focus management: capture previous focus and restore on close, focus first input on open
    useEffect(() => {
        if (isOpen) {
            previousFocusRef.current = document.activeElement as HTMLElement | null;
            // Delay slightly to ensure elements are mounted
            const timer = setTimeout(() => {
                const firstInput = modalRef.current?.querySelector<HTMLElement>(
                    "select, input, textarea, button"
                );
                firstInput?.focus();
            }, 50);

            // Lock body scroll while modal is active
            const originalOverflow = document.body.style.overflow;
            document.body.style.overflow = "hidden";

            return () => {
                clearTimeout(timer);
                document.body.style.overflow = originalOverflow;
                previousFocusRef.current?.focus();
            };
        }
    }, [isOpen]);

    // Keyboard handling: Escape to close and Tab trapping
    useEffect(() => {
        function handleKeyDown(event: KeyboardEvent) {
            if (!isOpen) return;

            if (event.key === "Escape" && !isSubmitting) {
                event.preventDefault();
                onClose();
                return;
            }

            if (event.key === "Tab" && modalRef.current) {
                const focusableElements = modalRef.current.querySelectorAll<HTMLElement>(
                    'button:not([disabled]), [href], input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])'
                );

                if (focusableElements.length === 0) return;

                const firstElement = focusableElements[0];
                const lastElement = focusableElements[focusableElements.length - 1];

                if (event.shiftKey) {
                    if (document.activeElement === firstElement) {
                        event.preventDefault();
                        lastElement.focus();
                    }
                } else {
                    if (document.activeElement === lastElement) {
                        event.preventDefault();
                        firstElement.focus();
                    }
                }
            }
        }

        window.addEventListener("keydown", handleKeyDown);
        return () => window.removeEventListener("keydown", handleKeyDown);
    }, [isOpen, isSubmitting, onClose]);

    if (!isOpen) {
        return null;
    }

    return (
        <div className={styles.overlay} onClick={onClose} role="presentation">
            <div
                ref={modalRef}
                className={styles.modal}
                onClick={(e) => e.stopPropagation()}
                role="dialog"
                aria-modal="true"
                aria-labelledby="decision-modal-title"
            >
                <div className={styles.modalHeader}>
                    <h3 id="decision-modal-title" className={styles.title}>{t("projects.modal.title")}</h3>
                    <button
                        type="button"
                        className={styles.closeButton}
                        onClick={onClose}
                        disabled={isSubmitting}
                        aria-label={t("common.close") || "Close"}
                    >
                        <FiX size={20} aria-hidden="true" />
                    </button>
                </div>
                <form onSubmit={handleSubmit}>
                    <div className={styles.formGroup}>
                        <label htmlFor="status" className={styles.label}>
                            {t("projects.modal.status")}
                        </label>
                        <select
                            id="status"
                            name="status"
                            value={form.status}
                            onChange={handleChange}
                            disabled={isSubmitting}
                            className={styles.select}
                            aria-invalid={!!errors.status}
                            aria-describedby={errors.status ? "decision-status-error" : undefined}
                        >
                            <option value={0}>{t("projects.modal.selectStatus")}</option>
                            {statuses.map((status) => (
                                <option key={status.id} value={status.id}>
                                    {translateStatus(status)}
                                </option>
                            ))}
                        </select>
                        {errors.status && (
                            <span id="decision-status-error" className={styles.error}>
                                {errors.status}
                            </span>
                        )}
                    </div>

                    <div className={styles.formGroup}>
                        <label htmlFor="observation" className={styles.label}>
                            {t("projects.modal.observation")}
                        </label>
                        <textarea
                            id="observation"
                            name="observation"
                            value={form.observation}
                            onChange={handleChange}
                            disabled={isSubmitting}
                            className={styles.textarea}
                            aria-invalid={!!errors.observation}
                            aria-describedby={errors.observation ? "decision-observation-error" : undefined}
                        />
                        {errors.observation && (
                            <span id="decision-observation-error" className={styles.error}>
                                {errors.observation}
                            </span>
                        )}
                    </div>

                    <div className={styles.actions}>
                        <Button
                            type="button"
                            variant="outline"
                            onClick={onClose}
                            disabled={isSubmitting}
                        >
                            {t("projects.modal.cancel")}
                        </Button>
                        <Button
                            type="submit"
                            variant="primary"
                            disabled={!isValid}
                            isLoading={isSubmitting}
                        >
                            {t("projects.modal.confirm")}
                        </Button>
                    </div>
                </form>
            </div>
        </div>
    );
}

