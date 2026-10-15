import { Link } from "react-router-dom";
import { FiArrowRight, FiCalendar, FiLayers, FiTag } from "react-icons/fi";

import { Card, Badge } from "@/components/ui";
import { useTranslation } from "@/hooks";
import { formatCurrency, getStatusBadgeVariant } from "@/utils";
import type { ProjectShortResponse } from "@/api/responses/ProjectShortResponse";

import styles from "./ProjectShortCard.module.css";

interface ProjectShortCardProps {
    project: ProjectShortResponse;
}

export default function ProjectShortCard({ project }: ProjectShortCardProps) {
    const { t, translateStatus, translateType, translateArea } = useTranslation();

    const statusVariant = getStatusBadgeVariant(project.status);
    const translatedStatus = translateStatus(project.status) || project.status || t("projects.card.notAvailable");
    const translatedArea = translateArea(project.area) || project.area;
    const translatedType = translateType(project.type) || project.type;

    return (
        <Link
            to={`/project/${project.id}`}
            className={styles.cardLink}
            aria-label={t("projects.card.viewDetailsFor", { title: project.title })}
        >
            <Card className={styles.cardContent}>
                {/* Meta row: category tags and status badge */}
                <div className={styles.metaRow}>
                    <div className={styles.tagsGroup}>
                        {translatedArea && (
                            <span className={styles.categoryChip}>
                                <FiLayers className={styles.chipIcon} aria-hidden="true" />
                                <span className={styles.chipText}>{translatedArea}</span>
                            </span>
                        )}
                        {translatedType && (
                            <span className={styles.categoryChip}>
                                <FiTag className={styles.chipIcon} aria-hidden="true" />
                                <span className={styles.chipText}>{translatedType}</span>
                            </span>
                        )}
                    </div>
                    <Badge variant={statusVariant} className={styles.statusBadge}>
                        {translatedStatus}
                    </Badge>
                </div>

                {/* Content: Title & clamped description */}
                <div className={styles.body}>
                    <h3 className={styles.title}>{project.title}</h3>
                    {project.description && (
                        <p className={styles.description}>{project.description}</p>
                    )}
                </div>

                {/* Metrics 2-column block */}
                <div className={styles.metricsGrid}>
                    <div className={styles.metricItem}>
                        <span className={styles.metricLabel}>{t("projects.card.amount")}</span>
                        <span className={styles.metricAmount}>
                            {formatCurrency(project.amount)}
                        </span>
                    </div>
                    <div className={styles.metricItem}>
                        <span className={styles.metricLabel}>{t("projects.card.duration")}</span>
                        <span className={styles.metricValue}>
                            <FiCalendar className={styles.metricIcon} aria-hidden="true" />
                            <span>{project.duration} {t("projects.card.months")}</span>
                        </span>
                    </div>
                </div>

                {/* Card footer action */}
                <div className={styles.footerAction}>
                    <span className={styles.viewProposalText}>
                        {t("projects.card.viewProposal")}
                    </span>
                    <FiArrowRight className={styles.arrowIcon} aria-hidden="true" />
                </div>
            </Card>
        </Link>
    );
}

