import type { ButtonHTMLAttributes } from "react";
import styles from "./Button.module.css";

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: "primary" | "secondary" | "outline" | "ghost" | "danger" | "success";
  size?: "sm" | "md" | "lg";
  isLoading?: boolean;
}

export default function Button({
  children,
  variant = "primary",
  size = "md",
  isLoading = false,
  className = "",
  disabled,
  ...props
}: ButtonProps) {
  const baseClass = styles.button;
  const variantClass = styles[variant];
  const sizeClass = styles[size];
  const loadingClass = isLoading ? styles.loading : "";

  const combinedClasses = [baseClass, variantClass, sizeClass, loadingClass, className]
    .filter(Boolean)
    .join(" ");

  return (
    <button
      className={combinedClasses}
      disabled={disabled || isLoading}
      aria-busy={isLoading || undefined}
      {...props}
    >
      {isLoading ? <span className={styles.spinner} aria-hidden="true" /> : null}
      {children}
    </button>
  );
}
