import styles from "./BrandLogo.module.css";

interface BrandLogoProps {
  size?: "sm" | "md" | "lg";
  className?: string;
}

export default function BrandLogo({
  size = "sm",
  className = "",
}: BrandLogoProps) {
  const sizeClass = styles[size];
  const combinedClass = [styles.logoWrapper, sizeClass, className]
    .filter(Boolean)
    .join(" ");

  return (
    <div className={combinedClass} aria-hidden="true">
      <svg
        viewBox="0 0 32 32"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
        className={styles.svgIcon}
      >
        <defs>
          <linearGradient
            id="brandLogoGradient"
            x1="0"
            y1="0"
            x2="32"
            y2="32"
            gradientUnits="userSpaceOnUse"
          >
            <stop offset="0%" stopColor="#3b82f6" />
            <stop offset="100%" stopColor="#0284c7" />
          </linearGradient>
        </defs>

        <rect
          x="2"
          y="2"
          width="28"
          height="28"
          rx="7"
          fill="url(#brandLogoGradient)"
        />
        <rect
          x="2.5"
          y="2.5"
          width="27"
          height="27"
          rx="6.5"
          fill="none"
          stroke="rgba(255,255,255,0.25)"
          strokeWidth="1"
        />

        <rect
          x="7"
          y="6"
          width="18"
          height="20"
          rx="3"
          fill="rgba(255,255,255,0.18)"
        />

        <line
          x1="10"
          y1="10"
          x2="16"
          y2="10"
          stroke="#ffffff"
          strokeWidth="1.75"
          strokeLinecap="round"
        />
        <line
          x1="10"
          y1="13.5"
          x2="20"
          y2="13.5"
          stroke="rgba(255,255,255,0.75)"
          strokeWidth="1.5"
          strokeLinecap="round"
        />
        <line
          x1="10"
          y1="17"
          x2="15"
          y2="17"
          stroke="rgba(255,255,255,0.5)"
          strokeWidth="1.5"
          strokeLinecap="round"
        />

        <circle cx="19.5" cy="19.5" r="5.5" fill="#10b981" />
        <path
          d="M17.5 19.5L18.8 20.8L21.8 17.8"
          stroke="#ffffff"
          strokeWidth="1.5"
          strokeLinecap="round"
          strokeLinejoin="round"
        />
      </svg>
    </div>
  );
}
