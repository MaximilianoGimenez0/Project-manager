import { useMemo } from "react";
import { Link, useParams } from "react-router-dom";
import {
  FiArrowLeft,
  FiShield,
  FiClock,
  FiCheckCircle,
  FiAlertTriangle,
  FiUser,
  FiDollarSign,
  FiCalendar,
  FiLayers,
  FiTag,
  FiFileText,
} from "react-icons/fi";

import { STATUS_IDS } from "@/config";

import { Card, Badge } from "@/components/ui";
import { useProjectDetail, useTranslation } from "@/hooks";
import { formatCurrency, getStatusBadgeVariant } from "@/utils";

import styles from "./ProjectDetailPage.module.css";

export default function ProjectDetailPage() {
  const { id } = useParams<{ id: string }>();
  const {
    t,
    language,
    translateStatus,
    translateRole,
    translateArea,
    translateType,
  } = useTranslation();
  const {
    project,
    steps,
    nextPendingStep,
    nextApproverRole,
    isWorkflowCompleted,
    isLoading,
    error,
  } = useProjectDetail(id);

  const dateFormatter = useMemo(
    () =>
      new Intl.DateTimeFormat(
        language === "en" ? "en-US" : language === "pt" ? "pt-BR" : "es-AR",
        {
          year: "numeric",
          month: "short",
          day: "numeric",
        }
      ),
    [language]
  );

  if (isLoading) {
    return (
      <div className={styles.pageContainer}>
        <div className={styles.stateWrapper} role="status" aria-live="polite">
          <div className={styles.spinner} />
          <p className={styles.stateText}>{t("pages.projectDetail.loading")}</p>
        </div>
      </div>
    );
  }

  if (error || !project) {
    return (
      <div className={styles.pageContainer}>
        <div className={styles.stateWrapper} role="alert">
          <FiAlertTriangle className={styles.errorIcon} aria-hidden="true" />
          <h2 className={styles.errorTitle}>
            {t("pages.projectDetail.notFound")}
          </h2>
          <p className={styles.stateText}>
            {error || t("pages.projectDetail.notFoundDesc")}
          </p>
          <Link to="/search" className={styles.backButton}>
            <FiArrowLeft aria-hidden="true" />
            <span>{t("pages.projectDetail.backToSearch")}</span>
          </Link>
        </div>
      </div>
    );
  }

  const projectCode = `PRJ-${project.id.slice(0, 8).toUpperCase()}`;

  return (
    <div className={styles.pageContainer}>
      <div className={styles.contentWrapper}>
        <div className={styles.navigationBar}>
          <Link to="/search" className={styles.backLink}>
            <FiArrowLeft aria-hidden="true" />
            <span>{t("pages.projectDetail.backToSearch")}</span>
          </Link>
          <span className={styles.projectCodeBadge}>{projectCode}</span>
        </div>

        <header className={styles.headerSection}>
          <div className={styles.headerInfo}>
            <div className={styles.titleRow}>
              <h1 className={styles.title}>{project.title}</h1>
              <Badge variant={getStatusBadgeVariant(project.status?.id ?? project.status?.name)}>
                {translateStatus(project.status) || t("pages.projectDetail.notAvailable")}
              </Badge>
            </div>
          </div>
        </header>

        <section
          className={styles.nextApproverSection}
          aria-label={t("pages.projectDetail.nextApproverAriaLabel")}
        >
          <Card padding="lg" className={styles.nextApproverCard}>
            <div className={styles.nextApproverLayout}>
              <div className={styles.nextApproverLeft}>
                <div
                  className={`${styles.roleIconWrapper} ${
                    isWorkflowCompleted
                      ? styles.roleIconCompleted
                      : styles.roleIconPending
                  }`}
                >
                  {isWorkflowCompleted ? (
                    <FiCheckCircle size={28} aria-hidden="true" />
                  ) : (
                    <FiShield size={28} aria-hidden="true" />
                  )}
                </div>

                <div className={styles.nextApproverContent}>
                  <div className={styles.nextApproverHeader}>
                    <span className={styles.nextApproverTag}>
                      {t("pages.projectDetail.nextApproverTitle")}
                    </span>
                    {nextPendingStep && (
                      <span className={styles.stepProgressText}>
                        {t("pages.projectDetail.nextApproverStep", {
                          current: nextPendingStep.stepOrder,
                          total: steps.length,
                        })}
                      </span>
                    )}
                  </div>

                  {!isWorkflowCompleted && nextApproverRole ? (
                    <>
                      <div className={styles.roleDisplayRow}>
                        <h2 className={styles.roleName}>
                          {translateRole(nextApproverRole)}
                        </h2>
                        <Badge variant="warning">
                          <FiClock aria-hidden="true" /> {t("pages.projectDetail.pendingEvaluation")}
                        </Badge>
                      </div>
                      <p className={styles.nextApproverDescription}>
                        {t("pages.projectDetail.nextApproverSubtitleRole", {
                          role: translateRole(nextApproverRole),
                        })}
                      </p>
                    </>
                  ) : (
                    <div className={styles.completedNotice}>
                      <h2 className={styles.roleName}>
                        {t("pages.projectDetail.workflowFinished")}
                      </h2>
                      <p className={styles.nextApproverDescription}>
                        {project?.status?.id === STATUS_IDS.REJECTED
                          ? t("pages.projectDetail.workflowRejected")
                          : t("pages.projectDetail.workflowApproved")}
                      </p>
                    </div>
                  )}
                </div>
              </div>
            </div>
          </Card>
        </section>

        <section
          className={styles.metricsGrid}
          aria-label={t("pages.projectDetail.metricsAriaLabel")}
        >
          <Card padding="md" className={styles.metricCard}>
            <div className={styles.metricIcon}>
              <FiDollarSign aria-hidden="true" />
            </div>
            <div className={styles.metricData}>
              <span className={styles.metricLabel}>
                {t("pages.projectDetail.budget")}
              </span>
              <span className={styles.metricValue}>
                {formatCurrency(project.amount)}
              </span>
            </div>
          </Card>

          <Card padding="md" className={styles.metricCard}>
            <div className={styles.metricIcon}>
              <FiCalendar aria-hidden="true" />
            </div>
            <div className={styles.metricData}>
              <span className={styles.metricLabel}>
                {t("pages.projectDetail.duration")}
              </span>
              <span className={styles.metricValue}>
                {project.duration} {t("pages.projectDetail.months")}
              </span>
            </div>
          </Card>

          <Card padding="md" className={styles.metricCard}>
            <div className={styles.metricIcon}>
              <FiLayers aria-hidden="true" />
            </div>
            <div className={styles.metricData}>
              <span className={styles.metricLabel}>
                {t("pages.projectDetail.area")}
              </span>
              <span className={styles.metricValue}>
                {translateArea(project.area) || t("pages.projectDetail.notAvailable")}
              </span>
            </div>
          </Card>

          <Card padding="md" className={styles.metricCard}>
            <div className={styles.metricIcon}>
              <FiTag aria-hidden="true" />
            </div>
            <div className={styles.metricData}>
              <span className={styles.metricLabel}>
                {t("pages.projectDetail.type")}
              </span>
              <span className={styles.metricValue}>
                {translateType(project.type) || t("pages.projectDetail.notAvailable")}
              </span>
            </div>
          </Card>
        </section>

        <div className={styles.mainGrid}>
          <div className={styles.primaryColumn}>
            <Card padding="lg" className={styles.sectionCard}>
              <div className={styles.cardHeader}>
                <FiFileText className={styles.sectionIcon} aria-hidden="true" />
                <h3 className={styles.sectionHeading}>
                  {t("pages.projectDetail.description")}
                </h3>
              </div>
              <p className={styles.descriptionText}>{project.description}</p>
            </Card>

            <Card padding="lg" className={styles.sectionCard}>
              <div className={styles.cardHeader}>
                <FiShield className={styles.sectionIcon} aria-hidden="true" />
                <h3 className={styles.sectionHeading}>
                  {t("pages.projectDetail.approvalWorkflow")}
                </h3>
              </div>

              <ol className={styles.stepsTimeline}>
                {steps.map((step) => {
                  const isCurrentPending = nextPendingStep?.id === step.id;
                  const isApproved =
                    step.status?.id === STATUS_IDS.APPROVED ||
                    Boolean(step.status?.name && (step.status.name.toLowerCase().includes("approv") || step.status.name.toLowerCase().includes("aprob")));
                  const isRejected =
                    step.status?.id === STATUS_IDS.REJECTED ||
                    Boolean(step.status?.name && (step.status.name.toLowerCase().includes("reject") || step.status.name.toLowerCase().includes("rechaz")));

                  return (
                    <li
                      key={step.id}
                      className={`${styles.timelineItem} ${
                        isCurrentPending ? styles.timelineItemActive : ""
                      }`}
                    >
                      <div className={styles.timelineMarker}>
                        <div
                          className={`${styles.timelineBadge} ${
                            isApproved
                              ? styles.timelineBadgeSuccess
                              : isRejected
                              ? styles.timelineBadgeError
                              : isCurrentPending
                              ? styles.timelineBadgePending
                              : styles.timelineBadgeInactive
                          }`}
                        >
                          {step.stepOrder}
                        </div>
                        <div className={styles.timelineLine} />
                      </div>

                      <div className={styles.timelineContent}>
                        <div className={styles.stepTitleRow}>
                          <h4 className={styles.stepApproverRole}>
                            {t("pages.projectDetail.stepOrderAndRole", {
                              order: step.stepOrder,
                              role: translateRole(step.approverRole),
                            })}
                          </h4>
                          <Badge variant={getStatusBadgeVariant(step.status?.id ?? step.status?.name)}>
                            {translateStatus(step.status) || t("pages.projectDetail.notAvailable")}
                          </Badge>
                        </div>

                        <div className={styles.stepDetailsList}>
                          {step.approverUser ? (
                            <div className={styles.stepDetailItem}>
                              <span className={styles.detailLabel}>
                                {t("pages.projectDetail.decidedBy")}:
                              </span>
                              <span className={styles.detailValue}>
                                {step.approverUser.name} ({step.approverUser.email})
                              </span>
                            </div>
                          ) : (
                            <div className={styles.stepDetailItem}>
                              <span className={styles.detailLabel}>
                                {t("pages.projectDetail.approverRole")}:
                              </span>
                              <span className={styles.detailValue}>
                                {translateRole(step.approverRole)} ({t("pages.projectDetail.pendingEvaluation")})
                              </span>
                            </div>
                          )}

                          {step.decisionDate && (
                            <div className={styles.stepDetailItem}>
                              <span className={styles.detailLabel}>
                                {t("pages.projectDetail.decisionDate")}:
                              </span>
                              <span className={styles.detailValue}>
                                {dateFormatter.format(step.decisionDate)}
                              </span>
                            </div>
                          )}

                          {step.observations && (
                            <div className={styles.stepObservations}>
                              <span className={styles.detailLabel}>
                                {t("pages.projectDetail.observations")}:
                              </span>
                              <p className={styles.observationBox}>
                                {step.observations}
                              </p>
                            </div>
                          )}
                        </div>
                      </div>
                    </li>
                  );
                })}
              </ol>
            </Card>
          </div>

          <aside
            className={styles.secondaryColumn}
            aria-label={t("pages.projectDetail.applicantAriaLabel")}
          >
            <Card padding="lg" className={styles.sectionCard}>
              <div className={styles.cardHeader}>
                <FiUser className={styles.sectionIcon} aria-hidden="true" />
                <h3 className={styles.sectionHeading}>
                  {t("pages.projectDetail.applicant")}
                </h3>
              </div>

              {project.user ? (
                <div className={styles.applicantProfile}>
                  <div className={styles.applicantAvatar}>
                    {(project.user.name?.trim() || "U").charAt(0).toUpperCase()}
                  </div>
                  <div className={styles.applicantInfo}>
                    <h4 className={styles.applicantName}>
                      {project.user.name || t("pages.projectDetail.notAvailable")}
                    </h4>
                    <span className={styles.applicantEmail}>
                      {project.user.email || t("pages.projectDetail.notAvailable")}
                    </span>
                    {project.user.role?.name && (
                      <span className={styles.applicantRoleBadge}>
                        {translateRole(project.user.role)}
                      </span>
                    )}
                  </div>
                </div>
              ) : (
                <p className={styles.descriptionText}>
                  {t("pages.projectDetail.notAvailable")}
                </p>
              )}
            </Card>
          </aside>
        </div>
      </div>
    </div>
  );
}
