import { useState, useCallback } from "react";
import {
  FiEdit3,
  FiInfo,
  FiEye,
  FiCheckCircle,
  FiCircle,
  FiDollarSign,
  FiCalendar,
} from "react-icons/fi";
import type { ProjectProposalCreateRequest } from "@/api";
import { ProjectForm } from "@/components";
import { Badge } from "@/components/ui";
import { useTranslation, useCurrentUser } from "@/hooks";
import type { FormMeta } from "@/hooks/useProjectForm";
import { createProjectAsync } from "@/services";
import { showError, showSuccess, formatCurrency } from "@/utils";
import styles from "./CreateProject.module.css";

export default function CreateProject() {
  const { t, translateArea, translateType } = useTranslation();
  const { currentUser } = useCurrentUser();

  const [liveForm, setLiveForm] = useState<ProjectProposalCreateRequest | null>(null);
  const [liveMeta, setLiveMeta] = useState<FormMeta>({});

  const handleFormChange = useCallback(
    (form: ProjectProposalCreateRequest, meta: FormMeta) => {
      setLiveForm(form);
      setLiveMeta(meta);
    },
    []
  );

  async function handleCreate(request: ProjectProposalCreateRequest) {
    try {
      await createProjectAsync(request);
      showSuccess(t("pages.createProject.messages.success"));
    } catch (error: unknown) {
      const errorMessage =
        error instanceof Error ? error.message : t("pages.createProject.messages.error");
      showError(errorMessage);
    }
  }

  // Completeness flags for interactive guidance checklist
  const hasTitle = Boolean(liveForm?.title?.trim());
  const hasDescription = Boolean(liveForm?.description?.trim());
  const hasAmount = Boolean(liveForm?.amount && liveForm.amount > 0);
  const hasDuration = Boolean(liveForm?.duration && liveForm.duration > 0);
  const hasArea = Boolean(liveForm?.area && liveForm.area > 0);
  const hasType = Boolean(liveForm?.type && liveForm.type > 0);

  const completedCount = [
    hasTitle,
    hasDescription,
    hasAmount,
    hasDuration,
    hasArea,
    hasType,
  ].filter(Boolean).length;

  return (
    <div className={styles.pageContainer}>
      {/* Row 1: Proposal Draft Header (Single full-width row) */}
      <header className={styles.pageHeader}>
        <div className={styles.headerTitleRow}>
          <h1 className={styles.title}>
            <FiEdit3 className={styles.headerIcon} aria-hidden="true" />
            <span>{t("pages.createProject.header.title")}</span>
          </h1>
          <span className={styles.headerDraftBadge}>
            {t("projects.form.draftBadge")}
          </span>
        </div>
        <p className={styles.subtitle}>
          {t("pages.createProject.header.subtitle")}
        </p>
      </header>

      {/* Row 2: 2 Uniform Columns (Form & Preview/Guidance) */}
      <div className={styles.columnsLayout}>
        {/* Column 1: Proposal Form */}
        <section className={styles.formColumn} aria-label={t("pages.createProject.header.title")}>
          <div className={styles.formCard}>
            <ProjectForm
              onSubmit={handleCreate}
              onFormChange={handleFormChange}
            />
          </div>
        </section>

        {/* Column 2: Live Preview & Quality Checklist */}
        <aside className={styles.sidebarColumn} aria-label={t("projects.form.previewTitle")}>
          {/* Live Proposal Preview Card */}
          <div className={styles.previewCard}>
            <div className={styles.previewHeader}>
              <span className={styles.previewBadge}>
                <FiEye className={styles.previewIcon} aria-hidden="true" />
                {t("projects.form.previewTitle")}
              </span>
            </div>

            <div className={styles.previewBody}>
              <div className={styles.previewTags}>
                {liveMeta.area ? (
                  <Badge variant="primary">{translateArea(liveMeta.area)}</Badge>
                ) : (
                  <Badge variant="secondary">{t("projects.card.area")}</Badge>
                )}

                {liveMeta.type ? (
                  <Badge variant="info">{translateType(liveMeta.type)}</Badge>
                ) : (
                  <Badge variant="secondary">{t("projects.card.type")}</Badge>
                )}
              </div>

              <h2 className={styles.previewProposalTitle}>
                {liveForm?.title?.trim() || t("projects.form.previewEmptyTitle")}
              </h2>

              <p className={styles.previewProposalDesc}>
                {liveForm?.description?.trim() || t("projects.form.previewEmptyDesc")}
              </p>

              <div className={styles.previewMetrics}>
                <div className={styles.metricItem}>
                  <div className={styles.metricLabel}>
                    <FiDollarSign aria-hidden="true" />
                    <span>{t("projects.form.amount")}</span>
                  </div>
                  <span className={styles.metricValue}>
                    {formatCurrency(liveForm?.amount || 0)}
                  </span>
                </div>

                <div className={styles.metricItem}>
                  <div className={styles.metricLabel}>
                    <FiCalendar aria-hidden="true" />
                    <span>{t("projects.form.duration")}</span>
                  </div>
                  <span className={styles.metricValue}>
                    {liveForm?.duration || 0} {t("projects.card.months")}
                  </span>
                </div>
              </div>

              {currentUser && (
                <div className={styles.previewAuthor}>
                  <div className={styles.previewAvatar} aria-hidden="true">
                    {(currentUser.name || "U").charAt(0).toUpperCase()}
                  </div>
                  <div className={styles.previewAuthorInfo}>
                    <span className={styles.previewAuthorRole}>{t("projects.form.author")}</span>
                    <span className={styles.previewAuthorName}>{currentUser.name}</span>
                  </div>
                </div>
              )}
            </div>

            <div className={styles.previewFooter}>
              <p className={styles.previewNoticeText}>
                {t("projects.form.previewNotice")}
              </p>
            </div>
          </div>

          {/* Institutional Quality Checklist */}
          <div className={styles.guidanceCard}>
            <div className={styles.guidanceHeader}>
              <h2 className={styles.guidanceTitle}>
                <FiInfo className={styles.guidanceIcon} aria-hidden="true" />
                <span>{t("pages.createProject.guidance.title")}</span>
              </h2>
              <span className={styles.progressPill}>
                {completedCount} / 6
              </span>
            </div>

            <ul className={styles.guidanceList}>
              <li className={hasTitle ? styles.completedStep : ""}>
                <span className={styles.checkIcon} aria-hidden="true">
                  {hasTitle ? <FiCheckCircle /> : <FiCircle />}
                </span>
                <span className={styles.checkLabel}>
                  {t("pages.createProject.guidance.titleLabel").replace(":", "")}
                </span>
              </li>
              <li className={hasAmount ? styles.completedStep : ""}>
                <span className={styles.checkIcon} aria-hidden="true">
                  {hasAmount ? <FiCheckCircle /> : <FiCircle />}
                </span>
                <span className={styles.checkLabel}>
                  {t("pages.createProject.guidance.amountLabel").replace(":", "")}
                </span>
              </li>
              <li className={hasDescription ? styles.completedStep : ""}>
                <span className={styles.checkIcon} aria-hidden="true">
                  {hasDescription ? <FiCheckCircle /> : <FiCircle />}
                </span>
                <span className={styles.checkLabel}>
                  {t("pages.createProject.guidance.descLabel").replace(":", "")}
                </span>
              </li>
              <li className={hasDuration ? styles.completedStep : ""}>
                <span className={styles.checkIcon} aria-hidden="true">
                  {hasDuration ? <FiCheckCircle /> : <FiCircle />}
                </span>
                <span className={styles.checkLabel}>
                  {t("pages.createProject.guidance.durationLabel").replace(":", "")}
                </span>
              </li>
              <li className={hasArea ? styles.completedStep : ""}>
                <span className={styles.checkIcon} aria-hidden="true">
                  {hasArea ? <FiCheckCircle /> : <FiCircle />}
                </span>
                <span className={styles.checkLabel}>
                  {t("pages.createProject.guidance.areaLabel").replace(":", "")}
                </span>
              </li>
              <li className={hasType ? styles.completedStep : ""}>
                <span className={styles.checkIcon} aria-hidden="true">
                  {hasType ? <FiCheckCircle /> : <FiCircle />}
                </span>
                <span className={styles.checkLabel}>
                  {t("pages.createProject.guidance.typeLabel").replace(":", "")}
                </span>
              </li>
            </ul>
          </div>
        </aside>
      </div>
    </div>
  );
}