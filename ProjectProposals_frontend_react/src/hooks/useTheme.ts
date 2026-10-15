import { useState, useEffect, useCallback } from "react";

export type Theme = "light" | "dark";

const THEME_STORAGE_KEY = "app_theme";

function getInitialTheme(): Theme {
  try {
    const savedTheme = localStorage.getItem(THEME_STORAGE_KEY);
    if (savedTheme === "light" || savedTheme === "dark") {
      return savedTheme;
    }
  } catch {
    // localStorage might be unavailable
  }

  if (typeof window !== "undefined" && window.matchMedia) {
    return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
  }

  return "light";
}

export function useTheme() {
  const [theme, setThemeState] = useState<Theme>(getInitialTheme);

  const applyTheme = useCallback((newTheme: Theme) => {
    document.documentElement.setAttribute("data-theme", newTheme);
    try {
      localStorage.setItem(THEME_STORAGE_KEY, newTheme);
    } catch {
      // Ignore write errors
    }
  }, []);

  const setTheme = useCallback((newTheme: Theme) => {
    setThemeState(newTheme);
    applyTheme(newTheme);
  }, [applyTheme]);

  const toggleTheme = useCallback(() => {
    setThemeState((prev) => {
      const nextTheme = prev === "dark" ? "light" : "dark";
      applyTheme(nextTheme);
      return nextTheme;
    });
  }, [applyTheme]);

  // Synchronize on mount and listen to system preference changes
  useEffect(() => {
    document.documentElement.setAttribute("data-theme", theme);

    const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");
    function handleSystemChange(event: MediaQueryListEvent) {
      const savedTheme = localStorage.getItem(THEME_STORAGE_KEY);
      // Only track system changes if user hasn't manually set a preference
      if (!savedTheme) {
        const systemTheme: Theme = event.matches ? "dark" : "light";
        setThemeState(systemTheme);
        document.documentElement.setAttribute("data-theme", systemTheme);
      }
    }

    mediaQuery.addEventListener("change", handleSystemChange);
    return () => mediaQuery.removeEventListener("change", handleSystemChange);
  }, [theme]);

  return {
    theme,
    isDark: theme === "dark",
    toggleTheme,
    setTheme,
  };
}
