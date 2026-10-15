import { useState, useRef, useEffect } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { FiChevronDown, FiMenu, FiX, FiLogOut, FiLogIn, FiShield } from "react-icons/fi";

import { APP_NAME } from "@/config";
import BrandLogo from "@/components/BrandLogo";
import LanguageSelector from "@/components/LanguageSelector";
import ThemeToggle from "@/components/ThemeToggle";
import { useTranslation, useCurrentUser } from "@/hooks";
import { showSuccess } from "@/utils";

import styles from "./Navbar.module.css";

export default function Navbar() {
  const { t, translateRole } = useTranslation();
  const navigate = useNavigate();
  const { currentUser, isAuthenticated, logout } = useCurrentUser();
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);
  const userButtonRef = useRef<HTMLButtonElement>(null);

  // Close dropdown on outside click
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsDropdownOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  // Close menus on Escape key
  useEffect(() => {
    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape") {
        if (isDropdownOpen) {
          setIsDropdownOpen(false);
          userButtonRef.current?.focus();
        }
        if (isMobileMenuOpen) {
          setIsMobileMenuOpen(false);
        }
      }
    }
    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, [isDropdownOpen, isMobileMenuOpen]);

  async function handleLogout() {
    setIsDropdownOpen(false);
    setIsMobileMenuOpen(false);
    await logout();
    showSuccess(t("navbar.logoutSuccess"));
    navigate("/login", { replace: true });
  }

  // When logged in: show all navigation links.
  // When not logged in: only show Home (Dashboard) and About.
  const navLinks = isAuthenticated
    ? [
        { to: "/", label: t("navbar.dashboard") },
        { to: "/create-project", label: t("navbar.newProject") },
        { to: "/search", label: t("navbar.explore") },
        { to: "/pending-projects", label: t("navbar.approvals") },
        { to: "/about", label: t("navbar.about") },
      ]
    : [
        { to: "/", label: t("navbar.dashboard") },
        { to: "/about", label: t("navbar.about") },
      ];

  return (
    <header className={styles.header}>
      <div className={styles.container}>
        {/* Left: Brand with SVG Logo */}
        <div className={styles.brandContainer}>
          <NavLink to="/" className={styles.brandLink}>
            <BrandLogo size="sm" />
            <span>{APP_NAME}</span>
          </NavLink>
        </div>

        {/* Center: Navigation Links */}
        <nav
          id="mobile-navigation-menu"
          className={`${styles.navLinks} ${isMobileMenuOpen ? styles.navLinksOpen : ""}`}
        >
          {navLinks.map((link) => (
            <NavLink
              key={link.to}
              to={link.to}
              className={({ isActive }) =>
                isActive ? `${styles.link} ${styles.active}` : styles.link
              }
              onClick={() => setIsMobileMenuOpen(false)}
            >
              {link.label}
            </NavLink>
          ))}

          {!isAuthenticated && (
            <NavLink
              to="/login"
              className={styles.mobileLoginItem}
              onClick={() => setIsMobileMenuOpen(false)}
            >
              <FiLogIn aria-hidden="true" />
              <span>{t("navbar.login")}</span>
            </NavLink>
          )}
        </nav>

        {/* Right: Actions & User Menu */}
        <div className={styles.rightSection} ref={dropdownRef}>
          <ThemeToggle />
          <LanguageSelector />

          {isAuthenticated && currentUser ? (
            <div className={styles.userMenuContainer}>
              <button
                ref={userButtonRef}
                className={styles.userButton}
                onClick={() => setIsDropdownOpen(!isDropdownOpen)}
                aria-expanded={isDropdownOpen}
                aria-haspopup="menu"
                aria-controls="user-dropdown-menu"
              >
                <div className={styles.avatar}>
                  {(currentUser.name || "U").charAt(0).toUpperCase()}
                </div>
                <div className={styles.userInfoInline}>
                  <span className={styles.userName}>{currentUser.name}</span>
                  {currentUser.role?.name && (
                    <span className={styles.roleBadge}>{translateRole(currentUser.role)}</span>
                  )}
                </div>
                <FiChevronDown
                  className={`${styles.chevron} ${isDropdownOpen ? styles.chevronOpen : ""}`}
                  aria-hidden="true"
                />
              </button>

              {isDropdownOpen && (
                <div
                  id="user-dropdown-menu"
                  role="menu"
                  className={styles.dropdownMenu}
                >
                  <div className={styles.dropdownHeader}>
                    <span className={styles.profileName}>{currentUser.name}</span>
                    <span className={styles.profileEmail}>{currentUser.email}</span>
                    <div className={styles.profileRole}>
                      <FiShield className={styles.roleIcon} aria-hidden="true" />
                      <span>{translateRole(currentUser.role) || t("navbar.role")}</span>
                    </div>
                  </div>

                  <div className={styles.dropdownDivider} />

                  <div className={styles.dropdownActions}>
                    <button
                      role="menuitem"
                      className={styles.logoutButton}
                      onClick={handleLogout}
                    >
                      <FiLogOut className={styles.logoutIcon} aria-hidden="true" />
                      <span>{t("navbar.logout")}</span>
                    </button>
                  </div>
                </div>
              )}
            </div>
          ) : (
            <NavLink
              to="/login"
              className={styles.loginLink}
              onClick={() => setIsMobileMenuOpen(false)}
            >
              <FiLogIn aria-hidden="true" />
              <span>{t("navbar.login")}</span>
            </NavLink>
          )}

          {/* Mobile Menu Toggle Button */}
          <button
            className={styles.burgerButton}
            onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
            aria-expanded={isMobileMenuOpen}
            aria-controls="mobile-navigation-menu"
            aria-label={t("navbar.toggleMenu")}
          >
            {isMobileMenuOpen ? (
              <FiX size={24} aria-hidden="true" />
            ) : (
              <FiMenu size={24} aria-hidden="true" />
            )}
          </button>
        </div>
      </div>
    </header>
  );
}