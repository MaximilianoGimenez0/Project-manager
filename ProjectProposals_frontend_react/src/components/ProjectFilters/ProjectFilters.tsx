import { FiSearch, FiX } from "react-icons/fi";
import type { ProjectFilterParams } from "@/api/params/ProjectFilterParams";
import type { Status, User } from "@/models";
import styles from "./ProjectFilters.module.css";
import { Button } from "@/components/ui";
import { useTranslation } from "@/hooks";

interface ProjectFiltersProps {
    filters: ProjectFilterParams;
    statuses: Status[];
    users: User[];
    onChange: (filters: ProjectFilterParams) => void;
    onSearch: () => void;
    onClear?: () => void;
}

export default function ProjectFilters({
    filters,
    statuses,
    users,
    onChange,
    onSearch,
    onClear,
}: ProjectFiltersProps) {
    const { t, translateStatus } = useTranslation();

    const hasActiveFilters = Boolean(
        filters.title?.trim() ||
        filters.status !== undefined ||
        filters.applicant !== undefined ||
        filters.approvalUser !== undefined
    );

    function handleChange(e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) {
        const { name, value } = e.target;

        let parsedValue: string | number | undefined;
        if (name !== 'title') {
            parsedValue = value ? Number(value) : undefined;
        } else {
            parsedValue = value || undefined;
        }

        onChange({
            ...filters,
            [name]: parsedValue,
        });
    }

    function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        onSearch();
    }

    function handleReset() {
        if (onClear) {
            onClear();
        } else {
            onChange({});
            onSearch();
        }
    }

    return (
        <form className={styles.filtersContainer} onSubmit={handleSubmit} role="search">
            <div className={`${styles.filterGroup} ${styles.filterGroupTitle}`}>
                <label htmlFor="filter-title" className={styles.label}>
                    {t("projects.filters.title")}
                </label>
                <div className={styles.inputWrapper}>
                    <FiSearch className={styles.inputIcon} aria-hidden="true" />
                    <input
                        type="search"
                        id="filter-title"
                        name="title"
                        value={filters.title || ""}
                        onChange={handleChange}
                        className={styles.inputWithIcon}
                        placeholder={t("projects.filters.searchTitle")}
                    />
                </div>
            </div>

            <div className={styles.filterGroup}>
                <label htmlFor="filter-status" className={styles.label}>
                    {t("projects.filters.status")}
                </label>
                <select
                    id="filter-status"
                    name="status"
                    value={filters.status ?? ""}
                    onChange={handleChange}
                    className={styles.select}
                >
                    <option value="">{t("projects.filters.allStatuses")}</option>
                    {statuses.map(status => (
                        <option key={status.id} value={status.id}>
                            {translateStatus(status)}
                        </option>
                    ))}
                </select>
            </div>

            <div className={styles.filterGroup}>
                <label htmlFor="filter-applicant" className={styles.label}>
                    {t("projects.filters.creator")}
                </label>
                <select
                    id="filter-applicant"
                    name="applicant"
                    value={filters.applicant ?? ""}
                    onChange={handleChange}
                    className={styles.select}
                >
                    <option value="">{t("projects.filters.allCreators")}</option>
                    {users.map((user, index) => (
                        <option key={user.id ?? `user-${index}`} value={user.id ?? ""}>
                            {user.name || "Unknown"}
                        </option>
                    ))}
                </select>
            </div>

            <div className={styles.filterGroup}>
                <label htmlFor="filter-approval-user" className={styles.label}>
                    {t("projects.filters.approver")}
                </label>
                <select
                    id="filter-approval-user"
                    name="approvalUser"
                    value={filters.approvalUser ?? ""}
                    onChange={handleChange}
                    className={styles.select}
                >
                    <option value="">{t("projects.filters.allApprovers")}</option>
                    {users.map((user, index) => (
                        <option key={user.id ?? `user-${index}`} value={user.id ?? ""}>
                            {user.name || "Unknown"}
                        </option>
                    ))}
                </select>
            </div>

            <div className={styles.actionsGroup}>
                <Button type="submit" variant="primary" className={styles.searchButton}>
                    <FiSearch aria-hidden="true" />
                    {t("projects.filters.search")}
                </Button>
                {hasActiveFilters && (
                    <Button
                        type="button"
                        variant="outline"
                        onClick={handleReset}
                        className={styles.clearButton}
                    >
                        <FiX aria-hidden="true" />
                        {t("pages.search.clearFilters")}
                    </Button>
                )}
            </div>
        </form>
    );
}

