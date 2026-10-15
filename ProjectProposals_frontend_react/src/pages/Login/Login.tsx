import { useState, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { FiMail, FiLock, FiEye, FiEyeOff, FiAlertCircle, FiLogIn, FiUserCheck, FiCheck } from "react-icons/fi";

import { ROLE_IDS } from "@/config";

import { Button, Card, Badge } from "@/components/ui";

import { useCurrentUser, useTranslation } from "@/hooks";

import { loginAsync } from "@/services";

import { showSuccess, showError } from "@/utils";

import styles from "./Login.module.css";

interface DemoUser {
  id: number;
  name: string;
  email: string;
  roleId: number;
}

interface FormErrors {
  email?: string;
  password?: string;
  general?: string;
}

const DEMO_USERS: DemoUser[] = [
  { id: 1, name: "José Ferreyra", email: "jferreyra@unaj.com", roleId: ROLE_IDS.MANAGER },
  { id: 2, name: "Ana Lucero", email: "alucero@unaj.com", roleId: ROLE_IDS.AREA_LEADER },
  { id: 3, name: "Gonzalo Molinas", email: "gmolinas@unaj.com", roleId: ROLE_IDS.MANAGER },
  { id: 4, name: "Lucas Olivera", email: "lolivera@unaj.com", roleId: ROLE_IDS.DIRECTOR },
  { id: 5, name: "Danilo Fagundez", email: "dfagundez@unaj.com", roleId: ROLE_IDS.TECHNICAL_COMMITTEE },
  { id: 6, name: "Gabriel Galli", email: "ggalli@unaj.com", roleId: ROLE_IDS.TECHNICAL_COMMITTEE },
  { id: 7, name: "Mariana Torres", email: "mtorres@unaj.com", roleId: ROLE_IDS.AREA_LEADER },
  { id: 8, name: "Carlos Mendoza", email: "cmendoza@unaj.com", roleId: ROLE_IDS.DIRECTOR }
];


const DEFAULT_DEMO_PASSWORD = "Password123!";

export default function Login() {
  const { t, translateRole } = useTranslation();
  const navigate = useNavigate();
  const location = useLocation();
  const { isAuthenticated } = useCurrentUser();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState<FormErrors>({});

  // Destination path after successful login
  const fromPath = (location.state as { from?: { pathname: string } })?.from?.pathname || "/";

  useEffect(() => {
    if (isAuthenticated) {
      navigate("/", { replace: true });
    }
  }, [isAuthenticated, navigate]);

  function validate(): boolean {
    const newErrors: FormErrors = {};

    if (!email.trim()) {
      newErrors.email = t("pages.login.errors.emailRequired");
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim())) {
      newErrors.email = t("pages.login.errors.emailInvalid");
    }

    if (!password) {
      newErrors.password = t("pages.login.errors.passwordRequired");
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    if (!validate()) {
      return;
    }

    try {
      setIsSubmitting(true);
      setErrors({});

      const user = await loginAsync({
        email: email.trim(),
        password,
      });

      showSuccess(t("pages.login.successMessage", { name: user.name ?? "" }));
      navigate(fromPath, { replace: true });
    } catch (err) {
      const errorMessage =
        err instanceof Error
          ? err.message
          : t("pages.login.errorTitle");

      setErrors({ general: errorMessage });
      showError(errorMessage);
    } finally {
      setIsSubmitting(false);
    }
  }

  function handleSelectDemoUser(demoUser: DemoUser) {
    setEmail(demoUser.email);
    setPassword(DEFAULT_DEMO_PASSWORD);
    setErrors({});
  }

  return (
    <div className={styles.pageContainer}>
      <div className={styles.loginWrapper}>
        <Card padding="none" className={styles.loginCard}>
          <div className={styles.formColumn}>
            <header className={styles.cardHeader}>
              <h1 className={styles.title}>{t("pages.login.title")}</h1>
            </header>

            {errors.general && (
              <div className={styles.errorAlert} role="alert" aria-live="assertive">
                <FiAlertCircle className={styles.alertIcon} />
                <span>{errors.general}</span>
              </div>
            )}

            <form onSubmit={handleSubmit} className={styles.form} noValidate>
              <div className={styles.fieldGroup}>
                <label htmlFor="login-email" className={styles.label}>
                  {t("pages.login.emailLabel")}
                </label>
                <div className={`${styles.inputContainer} ${errors.email ? styles.inputError : ""}`}>
                  <FiMail className={styles.fieldIcon} aria-hidden="true" />
                  <input
                    id="login-email"
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder={t("pages.login.emailPlaceholder")}
                    className={styles.input}
                    autoComplete="email"
                    disabled={isSubmitting}
                    aria-invalid={Boolean(errors.email)}
                    aria-describedby={errors.email ? "login-email-error" : undefined}
                  />
                </div>
                {errors.email && (
                  <span id="login-email-error" className={styles.errorMessage} role="alert">
                    {errors.email}
                  </span>
                )}
              </div>

              <div className={styles.fieldGroup}>
                <label htmlFor="login-password" className={styles.label}>
                  {t("pages.login.passwordLabel")}
                </label>
                <div className={`${styles.inputContainer} ${errors.password ? styles.inputError : ""}`}>
                  <FiLock className={styles.fieldIcon} aria-hidden="true" />
                  <input
                    id="login-password"
                    type={showPassword ? "text" : "password"}
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    placeholder={t("pages.login.passwordPlaceholder")}
                    className={styles.input}
                    autoComplete="current-password"
                    disabled={isSubmitting}
                    aria-invalid={Boolean(errors.password)}
                    aria-describedby={errors.password ? "login-password-error" : undefined}
                  />
                  <button
                    type="button"
                    className={styles.toggleVisibilityBtn}
                    onClick={() => setShowPassword(!showPassword)}
                    aria-label={
                      showPassword
                        ? t("pages.login.hidePassword")
                        : t("pages.login.showPassword")
                    }
                    aria-pressed={showPassword}
                  >
                    {showPassword ? <FiEyeOff aria-hidden="true" /> : <FiEye aria-hidden="true" />}
                  </button>
                </div>
                {errors.password && (
                  <span id="login-password-error" className={styles.errorMessage} role="alert">
                    {errors.password}
                  </span>
                )}
              </div>

              <Button
                type="submit"
                variant="primary"
                size="lg"
                isLoading={isSubmitting}
                className={styles.submitButton}
              >
                <FiLogIn />
                <span>
                  {isSubmitting
                    ? t("pages.login.submittingButton")
                    : t("pages.login.submitButton")}
                </span>
              </Button>
            </form>
          </div>

          {/* Quick-fill Demo Users Section */}
          <section className={styles.demoSection}>
            <div className={styles.demoHeader}>
              <FiUserCheck className={styles.demoIcon} />
              <div>
                <h2 className={styles.demoTitle}>{t("pages.login.demoAccountsTitle")}</h2>
                <p className={styles.demoSubtitle}>{t("pages.login.demoAccountsSubtitle")}</p>
              </div>
            </div>

            <div className={styles.demoGrid} role="group" aria-label={t("pages.login.demoAccountsTitle")}>
              {DEMO_USERS.map((demo) => {
                const isSelected = email === demo.email;
                return (
                  <button
                    key={demo.id}
                    type="button"
                    className={`${styles.demoUserButton} ${isSelected ? styles.demoUserActive : ""}`}
                    onClick={() => handleSelectDemoUser(demo)}
                    disabled={isSubmitting}
                    aria-pressed={isSelected}
                    aria-label={`${demo.name}, ${translateRole(demo.roleId)} (${demo.email})`}
                  >
                    <div
                      className={`${styles.demoAvatar} ${isSelected ? styles.demoAvatarActive : ""}`}
                      aria-hidden="true"
                    >
                      {demo.name.charAt(0).toUpperCase()}
                    </div>

                    <div className={styles.demoContent}>
                      <div className={styles.demoHeaderRow}>
                        <span className={styles.demoUserName} title={demo.name}>
                          {demo.name}
                        </span>
                        <div
                          className={`${styles.demoIndicator} ${isSelected ? styles.demoIndicatorActive : ""}`}
                          aria-hidden="true"
                        >
                          <FiCheck className={styles.checkIcon} />
                        </div>
                      </div>
                      <span className={styles.demoUserEmail} title={demo.email}>
                        {demo.email}
                      </span>
                      <div className={styles.demoBadgeWrapper}>
                        <Badge
                          variant={
                            demo.roleId === ROLE_IDS.DIRECTOR
                              ? "warning"
                              : demo.roleId === ROLE_IDS.MANAGER
                                ? "primary"
                                : demo.roleId === ROLE_IDS.AREA_LEADER
                                  ? "info"
                                  : "secondary"
                          }
                        >
                          {translateRole(demo.roleId)}
                        </Badge>
                      </div>
                    </div>
                  </button>
                );
              })}
            </div>
          </section>
        </Card>
      </div>
    </div>
  );
}
