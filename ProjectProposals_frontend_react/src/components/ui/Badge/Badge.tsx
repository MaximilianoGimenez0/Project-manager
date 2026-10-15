import type { HTMLAttributes } from "react";
import styles from "./Badge.module.css";

interface BadgeProps extends HTMLAttributes<HTMLSpanElement> {
  variant?: "primary" | "secondary" | "success" | "warning" | "error" | "info";
}

export default function Badge({
  children,
  variant = "secondary",
  className = "",
  ...props
}: BadgeProps) {
  const combinedClasses = [
    styles.badge,
    styles[variant],
    className,
  ]
    .filter(Boolean)
    .join(" ");

  return (
    <span className={combinedClasses} {...props}>
      {children}
    </span>
  );
}
