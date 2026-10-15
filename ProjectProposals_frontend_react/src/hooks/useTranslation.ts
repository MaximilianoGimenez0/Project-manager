import { useContext, useCallback } from "react";

import { LanguageContext } from "@/i18n/context";

import { formatStatus, formatRole, formatArea, formatType } from "@/utils";

import type { Area, Role, Status, Type } from "@/models";

export function useTranslation() {
    const context = useContext(LanguageContext);
    if (context === undefined) {
        throw new Error("useTranslation must be used within a LanguageProvider");
    }

    const { t } = context;

    const translateStatus = useCallback(
        (status: Status | string | number | null | undefined): string => {
            return formatStatus(status, t);
        },
        [t]
    );

    const translateRole = useCallback(
        (role: Role | string | number | null | undefined): string => {
            return formatRole(role, t);
        },
        [t]
    );

    const translateArea = useCallback(
        (area: Area | string | number | null | undefined): string => {
            return formatArea(area, t);
        },
        [t]
    );

    const translateType = useCallback(
        (type: Type | string | number | null | undefined): string => {
            return formatType(type, t);
        },
        [t]
    );

    return {
        ...context,
        translateStatus,
        translateRole,
        translateArea,
        translateType,
    };
}
