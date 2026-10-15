import type { HTMLAttributes } from "react";
import styles from "./Card.module.css";

interface CardProps extends HTMLAttributes<HTMLDivElement> {
  padding?: "none" | "sm" | "md" | "lg";
  interactive?: boolean;
}

const paddingClasses: Record<NonNullable<CardProps["padding"]>, string> = {
  none: styles.paddingNone,
  sm: styles.paddingSm,
  md: styles.paddingMd,
  lg: styles.paddingLg,
};

export default function Card({
  children,
  className = "",
  padding = "md",
  interactive = false,
  ...props
}: CardProps) {
  const combinedClasses = [
    styles.card,
    paddingClasses[padding],
    interactive ? styles.interactive : "",
    className,
  ]
    .filter(Boolean)
    .join(" ");

  return (
    <div className={combinedClasses} {...props}>
      {children}
    </div>
  );
}
