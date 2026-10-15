import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { FiSearch, FiFolder, FiFilter, FiAlertTriangle, FiPlus, FiRotateCcw } from "react-icons/fi";
import type { ProjectFilterParams } from "@/api";
import { ProjectFilters, ProjectShortCard } from "@/components";
import { Button } from "@/components/ui";
import { useProjectsFilter, useTranslation, useDebounce } from "@/hooks";
import styles from "./ProjectsSearchPage.module.css";

export default function ProjectsSearchPage() {
    const { t } = useTranslation();
    const { projects, statuses, users, isLoading, error, search } = useProjectsFilter();

    const [filters, setFilters] = useState<ProjectFilterParams>({});
    const debouncedFilters = useDebounce(filters, 350);

    const hasActiveFilters = Boolean(
        filters.title?.trim() ||
        filters.status !== undefined ||
        filters.applicant !== undefined ||
        filters.approvalUser !== undefined
    );

    useEffect(() => {
        void search(debouncedFilters);
    }, [search, debouncedFilters]);

    function handleFiltersChange(newFilters: ProjectFilterParams) {
        setFilters(newFilters);
    }

    function handleSearch() {
        void search(filters);
    }

    function handleClearFilters() {
        setFilters({});
    }

    return (
        <main className={styles.pageContainer}>
            <header className={styles.header}>
                <h1 className={styles.title}>
                    <FiSearch className={styles.headerIcon} aria-hidden="true" />
                    <span>{t("pages.search.header.title")}</span>
                </h1>
                <p className={styles.subtitle}>{t("pages.search.header.subtitle")}</p>
            </header>

            <section className={styles.filtersSection} aria-labelledby="filters-heading">
                <div className={styles.filtersHeader}>
                    <FiFilter className={styles.filtersIcon} aria-hidden="true" />
                    <h2 id="filters-heading" className={styles.filtersTitle}>
                        {t("pages.search.filters")}
                    </h2>
                </div>
                <ProjectFilters
                    filters={filters}
                    statuses={statuses}
                    users={users}
                    onChange={handleFiltersChange}
                    onSearch={handleSearch}
                    onClear={handleClearFilters}
                />
            </section>

            <section className={styles.resultsSection} aria-live="polite">
                {isLoading ? (
                    <div className={styles.loadingState}>
                        <div className={styles.spinner} aria-hidden="true"></div>
                        <p>{t("pages.search.loading")}</p>
                    </div>
                ) : error ? (
                    <div className={styles.emptyState}>
                        <div className={styles.emptyIcon}><FiAlertTriangle aria-hidden="true" /></div>
                        <h3 className={styles.emptyTitle}>{t("pages.search.errorTitle")}</h3>
                        <p className={styles.emptyDesc}>{error}</p>
                    </div>
                ) : (
                    <>
                        <div className={styles.resultsHeader}>
                            <span className={styles.resultsCountBadge}>
                                {t("pages.search.resultsCount", { count: projects.length })}
                            </span>
                        </div>
                        <div className={styles.resultsGrid}>
                            {projects.length > 0 ? (
                                projects.map((project) => (
                                    <ProjectShortCard key={project.id} project={project} />
                                ))
                            ) : (
                                <div className={styles.emptyState}>
                                    <div className={styles.emptyIcon}><FiFolder aria-hidden="true" /></div>
                                    <h3 className={styles.emptyTitle}>{t("pages.search.emptyTitle")}</h3>
                                    <p className={styles.emptyDesc}>
                                        {t("pages.search.emptyDesc")}
                                    </p>
                                    <div className={styles.emptyActions}>
                                        {hasActiveFilters && (
                                            <Button
                                                variant="outline"
                                                size="sm"
                                                onClick={handleClearFilters}
                                            >
                                                <FiRotateCcw aria-hidden="true" />
                                                {t("pages.search.clearFilters")}
                                            </Button>
                                        )}
                                        <Link to="/create-project" className={styles.createLink}>
                                            <Button variant="primary" size="sm">
                                                <FiPlus aria-hidden="true" />
                                                {t("pages.search.createNew")}
                                            </Button>
                                        </Link>
                                    </div>
                                </div>
                            )}
                        </div>
                    </>
                )}
            </section>
        </main>
    );
}

