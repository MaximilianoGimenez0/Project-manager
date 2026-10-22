import { FiAlertCircle, FiUserCheck } from "react-icons/fi";
import type { ProjectProposalCreateRequest } from "@/api";
import { Button } from "@/components/ui";
import { useProjectForm, useTranslation } from "@/hooks";
import type { FormMeta } from "@/hooks/useProjectForm";
import { formatCurrency } from "@/utils";
import styles from "./ProjectForm.module.css";

interface ProjectFormProps {
    onSubmit: (request: ProjectProposalCreateRequest) => Promise<void>;
    onFormChange?: (form: ProjectProposalCreateRequest, meta: FormMeta) => void;
}

export default function ProjectForm({ onSubmit, onFormChange }: ProjectFormProps) {
    const { t, translateArea, translateType, translateRole } = useTranslation();
    const {
        areas,
        types,
        selectedUser,
        form,
        errors,
        isSubmitting,
        handleChange,
        handleSubmit,
        isValid,
    } = useProjectForm({
        onSubmit,
        onFormChange,
    });

    return (
        <form className={styles.form} onSubmit={handleSubmit} noValidate>
            {selectedUser ? (
                <div className={styles.authorCard}>
                    <div className={styles.authorAvatar} aria-hidden="true">
                        {(selectedUser.name || "U").charAt(0).toUpperCase()}
                    </div>
                    <div className={styles.authorContent}>
                        <div className={styles.authorPrimaryRow}>
                            <span className={styles.authorName}>{selectedUser.name}</span>
                            <span className={styles.authorTag}>
                                <FiUserCheck className={styles.tagIcon} aria-hidden="true" />
                                {t("projects.form.author")}
                            </span>
                            {selectedUser.role?.name && (
                                <span className={styles.authorRoleBadge}>
                                    {translateRole(selectedUser.role)}
                                </span>
                            )}
                        </div>
                        {selectedUser.email && (
                            <span className={styles.authorEmail}>{selectedUser.email}</span>
                        )}
                    </div>
                </div>
            ) : (
                <div className={styles.userWarning} role="alert">
                    <FiAlertCircle className={styles.warningIcon} aria-hidden="true" />
                    <span>{t("projects.form.userWarning")}</span>
                </div>
            )}

            <div className={styles.field}>
                <div className={styles.labelRow}>
                    <label htmlFor="title" className={styles.label}>
                        {t("projects.form.title")} <span className={styles.requiredMark}>*</span>
                    </label>
                    <span className={styles.charCounter} aria-live="polite">
                        {form.title.length} / 255
                    </span>
                </div>
                <input
                    id="title"
                    name="title"
                    className={styles.input}
                    placeholder={t("projects.form.title")}
                    maxLength={255}
                    value={form.title}
                    onChange={handleChange}
                    disabled={isSubmitting}
                    aria-invalid={!!errors.title}
                    aria-describedby={errors.title ? "title-error" : undefined}
                />
                {errors.title && <span id="title-error" className={styles.error}>{errors.title}</span>}
            </div>

            <div className={styles.field}>
                <div className={styles.labelRow}>
                    <label htmlFor="description" className={styles.label}>
                        {t("projects.form.description")} <span className={styles.requiredMark}>*</span>
                    </label>
                    <span className={styles.charCounter} aria-live="polite">
                        {form.description.length} / 255
                    </span>
                </div>
                <textarea
                    id="description"
                    name="description"
                    className={styles.textarea}
                    placeholder={t("projects.form.description")}
                    maxLength={255}
                    value={form.description}
                    onChange={handleChange}
                    disabled={isSubmitting}
                    aria-invalid={!!errors.description}
                    aria-describedby={errors.description ? "description-error" : undefined}
                />
                {errors.description && (
                    <span id="description-error" className={styles.error}>{errors.description}</span>
                )}
            </div>

            <div className={styles.twoColGrid}>
                <div className={styles.field}>
                    <label htmlFor="amount" className={styles.label}>
                        {t("projects.form.amount")} ($) <span className={styles.requiredMark}>*</span>
                    </label>
                    <div className={styles.inputAffixGroup}>
                        <span className={styles.prefixAffix} aria-hidden="true">$</span>
                        <input
                            type="number"
                            id="amount"
                            name="amount"
                            className={`${styles.input} ${styles.inputWithPrefix}`}
                            min="1"
                            placeholder="0"
                            value={form.amount === 0 ? "" : form.amount}
                            onChange={handleChange}
                            disabled={isSubmitting}
                            aria-invalid={!!errors.amount}
                            aria-describedby={errors.amount ? "amount-error" : undefined}
                        />
                    </div>
                    {form.amount > 0 && (
                        <div className={styles.amountPill} aria-live="polite">
                            <span className={styles.amountPillLabel}>{t("projects.form.formattedPreview")}</span>
                            <span className={styles.amountPillValue}>{formatCurrency(form.amount)}</span>
                        </div>
                    )}
                    {errors.amount && <span id="amount-error" className={styles.error}>{errors.amount}</span>}
                </div>

                <div className={styles.field}>
                    <label htmlFor="duration" className={styles.label}>
                        {t("projects.form.duration")} ({t("projects.card.months")}) <span className={styles.requiredMark}>*</span>
                    </label>
                    <div className={styles.inputAffixGroup}>
                        <input
                            type="number"
                            id="duration"
                            name="duration"
                            className={`${styles.input} ${styles.inputWithSuffix}`}
                            min="1"
                            placeholder="0"
                            value={form.duration === 0 ? "" : form.duration}
                            onChange={handleChange}
                            disabled={isSubmitting}
                            aria-invalid={!!errors.duration}
                            aria-describedby={errors.duration ? "duration-error" : undefined}
                        />
                        <span className={styles.suffixAffix} aria-hidden="true">
                            {t("projects.card.months")}
                        </span>
                    </div>
                    {errors.duration && <span id="duration-error" className={styles.error}>{errors.duration}</span>}
                </div>
            </div>

            <div className={styles.twoColGrid}>
                <div className={styles.field}>
                    <label htmlFor="area" className={styles.label}>
                        {t("projects.card.area")} <span className={styles.requiredMark}>*</span>
                    </label>
                    <select
                        id="area"
                        name="area"
                        className={styles.select}
                        value={form.area}
                        onChange={handleChange}
                        disabled={isSubmitting}
                        aria-invalid={!!errors.area}
                        aria-describedby={errors.area ? "area-error" : undefined}
                    >
                        <option value={0}>{t("projects.form.selectArea")}</option>
                        {areas.map((area) => (
                            <option key={area.id} value={area.id}>
                                {translateArea(area)}
                            </option>
                        ))}
                    </select>
                    {errors.area && <span id="area-error" className={styles.error}>{errors.area}</span>}
                </div>

                <div className={styles.field}>
                    <label htmlFor="type" className={styles.label}>
                        {t("projects.card.type")} <span className={styles.requiredMark}>*</span>
                    </label>
                    <select
                        id="type"
                        name="type"
                        className={styles.select}
                        value={form.type}
                        onChange={handleChange}
                        disabled={isSubmitting}
                        aria-invalid={!!errors.type}
                        aria-describedby={errors.type ? "type-error" : undefined}
                    >
                        <option value={0}>{t("projects.form.selectType")}</option>
                        {types.map((type) => (
                            <option key={type.id} value={type.id}>
                                {translateType(type)}
                            </option>
                        ))}
                    </select>
                    {errors.type && <span id="type-error" className={styles.error}>{errors.type}</span>}
                </div>
            </div>

            <div className={styles.formFooter}>
                <Button
                    type="submit"
                    variant="primary"
                    size="md"
                    disabled={!isValid}
                    isLoading={isSubmitting}
                >
                    {isSubmitting ? t("projects.form.creating") : t("projects.form.create")}
                </Button>
            </div>
        </form>
    );
}
