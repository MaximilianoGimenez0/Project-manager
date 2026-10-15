import { useState } from "react";
import { Link } from "react-router-dom";
import {
    FiClock,
    FiCheckCircle,
    FiXCircle,
    FiArrowRight,
    FiUser,
    FiCalendar,
    FiLayers,
    FiTag,
    FiZap,
} from "react-icons/fi";

import { STATUS_IDS } from "@/config";

import DecisionModal from "@/components/DecisionModal";
import { Card, Badge, Button } from "@/components/ui";

import { useTranslation, useCurrentUser } from "@/hooks";

import { updateDecisionAsync } from "@/services";

import { showSuccess, showError, formatCurrency } from "@/utils";
import { ApiError } from "@/errors";

import type { DecisionStepUpdateRequest } from "@/api";
import type { Project, ApprovalStep, Role, User } from "@/models";

import styles from "./PendingProjectCard.module.css";

interface PendingProjectCardProps {
    project: Project;
    onDecisionSuccess?: () => void;
    currentUser?: User | null;
}

function isStepResolved(statusId?: number | null): boolean {
    return statusId === STATUS_IDS.APPROVED || statusId === STATUS_IDS.REJECTED;
}

function isStepApproved(statusId?: number | null): boolean {
    return statusId === STATUS_IDS.APPROVED;
}

function isProjectWorkflowCompleted(statusId?: number | null): boolean {
    return statusId === STATUS_IDS.APPROVED || statusId === STATUS_IDS.REJECTED;
}

function hasMatchingRole(userRole?: Role | null, approverRole?: Role | null): boolean {
    if (!userRole || !approverRole) return false;
    if (userRole.id !== undefined && approverRole.id !== undefined) {
        return Number(userRole.id) === Number(approverRole.id);
    }
    return false;
}

export default function PendingProjectCard({
    project,
    onDecisionSuccess,
    currentUser: propUser,
}: PendingProjectCardProps) {
    const {
        t,
        language,
        translateStatus,
        translateRole,
        translateArea,
        translateType,
    } = useTranslation();
    const { currentUser: hookUser } = useCurrentUser();
    const currentUser = propUser ?? hookUser;

    const [isDecisionModalOpen, setIsDecisionModalOpen] = useState(false);
    const [selectedStepId, setSelectedStepId] = useState<number>(0);

    const isWorkflowCompleted = isProjectWorkflowCompleted(project.status?.id);

    // Identify if there is a step directly awaiting the current user's decision
    const actionableStep = project.steps?.find((step) => {
        const isRoleMatching = hasMatchingRole(currentUser?.role, step.approverRole);
        const isResolved = isStepResolved(step.status?.id);
        return isRoleMatching && !isResolved && !isWorkflowCompleted;
    });

    const hasActionRequired = Boolean(actionableStep);

    function openDecisionModal(step: ApprovalStep) {
        if (
            !hasMatchingRole(currentUser?.role, step.approverRole) ||
            isStepResolved(step.status?.id) ||
            isWorkflowCompleted
        ) {
            return;
        }
        setSelectedStepId(step.id);
        setIsDecisionModalOpen(true);
    }

    function closeDecisionModal() {
        setIsDecisionModalOpen(false);
        setSelectedStepId(0);
    }

    async function handleDecision(request: DecisionStepUpdateRequest) {
        try {
            await updateDecisionAsync(project.id, request);
            showSuccess(t("projects.messages.decisionSuccess"));
            closeDecisionModal();
            onDecisionSuccess?.();
        } catch (error: unknown) {
            if (
                (error instanceof ApiError && error.status === 409) ||
                (error instanceof Error &&
                    (error.message.includes("409") ||
                        error.message.toLowerCase().includes("conflict") ||
                        error.message.toLowerCase().includes("creador") ||
                        error.message.toLowerCase().includes("creator")))
            ) {
                showError(t("projects.messages.creatorCannotApprove"));
            } else {
                showError(t("projects.messages.decisionError"));
            }
        }
    }

    return (
        <Card
            className={`${styles.cardWrapper} ${hasActionRequired ? styles.cardActionRequired : ""}`}
            padding="lg"
        >
            {/* Header: Status, Action Badge, Title and Description */}
            <div className={styles.header}>
                <div className={styles.headerBadges}>
                    {hasActionRequired && (
                        <span className={styles.urgentBadge}>
                            <FiZap className={styles.urgentIcon} />
                            {t("pages.pending.actionRequiredBadge")}
                        </span>
                    )}
                    <Badge variant="info">
                        {translateStatus(project.status) || t("projects.card.notAvailable")}
                    </Badge>
                </div>

                <Link to={`/project/${project.id}`} className={styles.titleLink}>
                    <h3 className={styles.title}>{project.title}</h3>
                </Link>

                {project.description && (
                    <p className={styles.description}>{project.description}</p>
                )}
            </div>

            {/* Quick Metrics Bar: Amount & Duration */}
            <div className={styles.metricsBar}>
                <div className={styles.metricItem}>
                    <span className={styles.metricLabel}>{t("projects.card.amount")}</span>
                    <span className={styles.metricValueAmount}>
                        {formatCurrency(project.amount)}
                    </span>
                </div>
                <div className={styles.metricDivider} />
                <div className={styles.metricItem}>
                    <span className={styles.metricLabel}>{t("projects.card.duration")}</span>
                    <span className={styles.metricValue}>
                        <FiCalendar className={styles.metricIcon} />
                        {project.duration} {t("projects.card.months")}
                    </span>
                </div>
            </div>

            {/* Details Pills (Area, Type, Applicant) */}
            <div className={styles.metaRow}>
                <div className={styles.metaPill}>
                    <FiLayers className={styles.metaIcon} />
                    <span className={styles.metaText}>
                        {translateArea(project.area) || t("projects.card.notAvailable")}
                    </span>
                </div>

                <div className={styles.metaPill}>
                    <FiTag className={styles.metaIcon} />
                    <span className={styles.metaText}>
                        {translateType(project.type) || t("projects.card.notAvailable")}
                    </span>
                </div>

                {project.user && (
                    <div className={styles.metaPill}>
                        <FiUser className={styles.metaIcon} />
                        <span className={styles.metaText}>
                            {project.user.name || t("projects.card.notAvailable")}
                        </span>
                    </div>
                )}
            </div>

            {/* Steps Timeline Section */}
            {project.steps && project.steps.length > 0 && (
                <div className={styles.stepsSection}>
                    <div className={styles.stepsSectionHeader}>
                        <h4 className={styles.subTitle}>{t("projects.card.steps")}</h4>
                        <span className={styles.stepsCount}>
                            {project.steps.filter((s) => isStepResolved(s.status?.id)).length} / {project.steps.length}
                        </span>
                    </div>

                    <div className={styles.stepsList}>
                        {project.steps.map((step) => {
                            const isRoleMatching = hasMatchingRole(currentUser?.role, step.approverRole);
                            const isResolved = isStepResolved(step.status?.id);
                            const isApproved = isStepApproved(step.status?.id);
                            const canDecide = isRoleMatching && !isResolved && !isWorkflowCompleted;

                            return (
                                <div
                                    key={step.id}
                                    className={`${styles.stepItem} ${canDecide ? styles.stepItemActionable : ""} ${isResolved ? styles.stepItemResolved : ""}`}
                                >
                                    <div className={styles.stepIndicator}>
                                        {isResolved ? (
                                             isApproved ? (
                                                <FiCheckCircle className={styles.stepSuccessIcon} />
                                            ) : (
                                                <FiXCircle className={styles.stepRejectIcon} />
                                            )
                                        ) : (
                                            <FiClock className={styles.stepPendingIcon} />
                                        )}
                                    </div>

                                    <div className={styles.stepBody}>
                                        <div className={styles.stepMainRow}>
                                            <span className={styles.stepOrder}>
                                                {t("projects.card.step", { order: step.stepOrder })}:{" "}
                                                <strong className={styles.stepRoleName}>
                                                    {translateRole(step.approverRole) || t("projects.card.notAvailable")}
                                                </strong>
                                            </span>
                                            <Badge variant={isResolved ? (isApproved ? "success" : "error") : "secondary"}>
                                                {translateStatus(step.status) || t("projects.card.notAvailable")}
                                            </Badge>
                                        </div>

                                        {step.approverUser && (
                                            <p className={styles.stepEvaluator}>
                                                {step.approverUser.name}
                                                {step.decisionDate &&
                                                    ` • ${step.decisionDate.toLocaleDateString(
                                                        language === "en" ? "en-US" : language === "pt" ? "pt-BR" : "es-AR"
                                                    )}`}
                                            </p>
                                        )}

                                        {step.observations && (
                                            <p className={styles.stepObservations}>
                                                &quot;{step.observations}&quot;
                                            </p>
                                        )}
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </div>
            )}

            {/* Card Footer: View Details link & Primary Decide button */}
            <div className={styles.cardFooter}>
                <Link to={`/project/${project.id}`} className={styles.viewDetailLink}>
                    <span>{t("pages.pending.viewDetails")}</span>
                    <FiArrowRight className={styles.linkArrowIcon} />
                </Link>

                {hasActionRequired && actionableStep && (
                    <Button
                        variant="primary"
                        size="sm"
                        onClick={() => openDecisionModal(actionableStep)}
                    >
                        <FiZap />
                        {t("projects.card.decide")}
                    </Button>
                )}
            </div>

            {/* Render DecisionModal only when explicitly opened */}
            {isDecisionModalOpen && (
                <DecisionModal
                    isOpen={isDecisionModalOpen}
                    stepId={selectedStepId}
                    onClose={closeDecisionModal}
                    onDecision={handleDecision}
                />
            )}
        </Card>
    );
}
