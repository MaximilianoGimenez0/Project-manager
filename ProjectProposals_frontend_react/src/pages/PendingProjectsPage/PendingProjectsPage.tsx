import { useState, useMemo } from "react";
import {
    FiClock,
    FiInbox,
    FiAlertTriangle,
    FiRefreshCw,
    FiSearch,
    FiX,
    FiCheckCircle,
    FiDollarSign,
    FiZap,
} from "react-icons/fi";

import { PendingProjectCard } from "@/components";
import { STATUS_IDS } from "@/config";

import { Button } from "@/components/ui";

import { usePendingProjects, useTranslation, useCurrentUser } from "@/hooks";
import { formatCurrency } from "@/utils";

import type { Role } from "@/models";

import styles from "./PendingProjectsPage.module.css";

function isProjectWorkflowCompleted(statusId?: number | null): boolean {
    return statusId === STATUS_IDS.APPROVED || statusId === STATUS_IDS.REJECTED;
}

function isStepResolved(statusId?: number | null): boolean {
    return statusId === STATUS_IDS.APPROVED || statusId === STATUS_IDS.REJECTED;
}

function hasMatchingRole(userRole?: Role | null, approverRole?: Role | null): boolean {
    if (!userRole || !approverRole) return false;
    if (userRole.id !== undefined && approverRole.id !== undefined) {
        return Number(userRole.id) === Number(approverRole.id);
    }
    return false;
}

export default function PendingProjectsPage() {
    const {
        t,
        translateArea,
        translateType,
        translateStatus,
    } = useTranslation();
    const { currentUser } = useCurrentUser();
    const { projects, isLoading, error, refresh } = usePendingProjects();

    const [searchQuery, setSearchQuery] = useState("");

    // Compute executive KPI stats
    const stats = useMemo(() => {
        let actionableCount = 0;
        let totalBudget = 0;

        for (const project of projects) {
            totalBudget += project.amount || 0;

            const isCompleted = isProjectWorkflowCompleted(project.status?.id);
            const hasAction = project.steps?.some((step) => {
                const isRoleMatch = hasMatchingRole(currentUser?.role, step.approverRole);
                const isResolved = isStepResolved(step.status?.id);
                return isRoleMatch && !isResolved && !isCompleted;
            });

            if (hasAction) {
                actionableCount++;
            }
        }

        return {
            total: projects.length,
            actionable: actionableCount,
            budget: totalBudget,
        };
    }, [projects, currentUser]);

    // Fast client-side search across relevant proposal fields (supporting original & localized terms)
    const filteredProjects = useMemo(() => {
        if (!searchQuery.trim()) {
            return projects;
        }
        const q = searchQuery.toLowerCase().trim();
        return projects.filter((project) => {
            const titleMatch = project.title.toLowerCase().includes(q);
            const descMatch = project.description?.toLowerCase().includes(q) ?? false;
            const applicantMatch = project.user?.name?.toLowerCase().includes(q) ?? false;
            const areaOriginal = project.area?.name?.toLowerCase() ?? "";
            const areaLocalized = translateArea(project.area).toLowerCase();
            const areaMatch = areaOriginal.includes(q) || areaLocalized.includes(q);
            const typeOriginal = project.type?.name?.toLowerCase() ?? "";
            const typeLocalized = translateType(project.type).toLowerCase();
            const typeMatch = typeOriginal.includes(q) || typeLocalized.includes(q);
            const statusOriginal = project.status?.name?.toLowerCase() ?? "";
            const statusLocalized = translateStatus(project.status).toLowerCase();
            const statusMatch = statusOriginal.includes(q) || statusLocalized.includes(q);
            return titleMatch || descMatch || applicantMatch || areaMatch || typeMatch || statusMatch;
        });
    }, [projects, searchQuery, translateArea, translateType, translateStatus]);

    const isSearching = searchQuery.trim().length > 0;

    return (
        <div className={styles.pageContainer}>
            {/* Executive Header with Refresh Action */}
            <div className={styles.header}>
                <div className={styles.headerContent}>
                    <h1 className={styles.title}>
                        <FiClock className={styles.headerIcon} /> {t("pages.pending.header.title")}
                    </h1>
                    <p className={styles.subtitle}>{t("pages.pending.header.subtitle")}</p>
                </div>

                <div className={styles.headerActions}>
                    <Button
                        variant="secondary"
                        size="md"
                        onClick={() => void refresh()}
                        disabled={isLoading}
                        aria-label={t("pages.pending.refresh")}
                    >
                        <FiRefreshCw className={`${styles.refreshIcon} ${isLoading ? styles.spinning : ""}`} />
                        <span>{t("pages.pending.refresh")}</span>
                    </Button>
                </div>
            </div>

            {/* KPI Stats Summary Cards */}
            {!error && projects.length > 0 && (
                <div className={styles.statsGrid}>
                    <div className={styles.statCard}>
                        <div className={styles.statIconWrapper}>
                            <FiClock className={styles.statIcon} />
                        </div>
                        <div className={styles.statInfo}>
                            <span className={styles.statLabel}>{t("pages.pending.stats.total")}</span>
                            <span className={styles.statValue}>{stats.total}</span>
                        </div>
                    </div>

                    <div className={`${styles.statCard} ${stats.actionable > 0 ? styles.statCardAlert : ""}`}>
                        <div className={`${styles.statIconWrapper} ${stats.actionable > 0 ? styles.statIconWrapperAlert : ""}`}>
                            <FiZap className={styles.statIcon} />
                        </div>
                        <div className={styles.statInfo}>
                            <span className={styles.statLabel}>{t("pages.pending.stats.actionNeeded")}</span>
                            <span className={styles.statValue}>{stats.actionable}</span>
                        </div>
                    </div>

                    <div className={styles.statCard}>
                        <div className={styles.statIconWrapper}>
                            <FiDollarSign className={styles.statIcon} />
                        </div>
                        <div className={styles.statInfo}>
                            <span className={styles.statLabel}>{t("pages.pending.stats.totalAmount")}</span>
                            <span className={styles.statValue}>{formatCurrency(stats.budget)}</span>
                        </div>
                    </div>
                </div>
            )}

            {/* Search and Toolbar */}
            {!error && projects.length > 0 && (
                <div className={styles.toolbar}>
                    <div className={styles.searchBox}>
                        <FiSearch className={styles.searchIcon} aria-hidden="true" />
                        <input
                            type="text"
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            placeholder={t("pages.pending.searchPlaceholder")}
                            aria-label={t("pages.pending.searchPlaceholder")}
                            className={styles.searchInput}
                        />
                        {isSearching && (
                            <button
                                type="button"
                                onClick={() => setSearchQuery("")}
                                className={styles.clearSearchBtn}
                                aria-label={t("pages.pending.clearSearch")}
                            >
                                <FiX />
                            </button>
                        )}
                    </div>

                    <div className={styles.resultsCount}>
                        {t("pages.pending.resultsCount", { count: filteredProjects.length })}
                    </div>
                </div>
            )}

            {/* Dashboard Content */}
            <div className={styles.dashboardContent}>
                {/* Skeleton Loading State */}
                {isLoading && projects.length === 0 && (
                    <div className={styles.skeletonGrid}>
                        {[1, 2, 3].map((index) => (
                            <div key={index} className={styles.skeletonCard}>
                                <div className={styles.skeletonLineShort} />
                                <div className={styles.skeletonLineTitle} />
                                <div className={styles.skeletonLineBody} />
                                <div className={styles.skeletonMetricsBox} />
                                <div className={styles.skeletonFooter} />
                            </div>
                        ))}
                    </div>
                )}

                {/* Error State with Retry Button */}
                {error && (
                    <div className={styles.errorState}>
                        <FiAlertTriangle className={styles.errorIcon} />
                        <h3 className={styles.errorTitle}>{t("pages.pending.errorTitle")}</h3>
                        <p className={styles.errorDesc}>{error}</p>
                        <Button
                            variant="primary"
                            size="md"
                            onClick={() => void refresh()}
                            className={styles.retryButton}
                        >
                            <FiRefreshCw className={styles.refreshIcon} />
                            <span>{t("pages.pending.retry")}</span>
                        </Button>
                    </div>
                )}

                {/* Empty State: Zero matches from Search */}
                {!isLoading && !error && projects.length > 0 && filteredProjects.length === 0 && (
                    <div className={styles.emptyState}>
                        <div className={styles.emptyIcon}><FiInbox /></div>
                        <h3 className={styles.emptyTitle}>{t("pages.pending.noMatchesTitle")}</h3>
                        <p className={styles.emptyDesc}>
                            {t("pages.pending.noMatchesDesc", { query: searchQuery })}
                        </p>
                        <Button
                            variant="outline"
                            size="sm"
                            onClick={() => setSearchQuery("")}
                        >
                            {t("pages.pending.clearSearch")}
                        </Button>
                    </div>
                )}

                {/* Empty State: Zero pending proposals overall */}
                {!isLoading && !error && projects.length === 0 && (
                    <div className={styles.emptyState}>
                        <div className={styles.emptyIconSuccess}><FiCheckCircle /></div>
                        <h3 className={styles.emptyTitle}>{t("pages.pending.emptyTitle")}</h3>
                        <p className={styles.emptyDesc}>{t("pages.pending.emptyDesc")}</p>
                    </div>
                )}

                {/* Projects Grid */}
                {!error && filteredProjects.length > 0 && (
                    <div className={styles.projectsList}>
                        {filteredProjects.map((project) => (
                            <PendingProjectCard
                                key={project.id}
                                project={project}
                                onDecisionSuccess={refresh}
                            />
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}
