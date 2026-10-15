import { useEffect, useState, useRef } from "react";
import type { Area, Type } from "@/models";
import type { ProjectProposalCreateRequest } from "@/api";
import { getAreasAsync, getTypesAsync } from "@/services";
import { validateProjectForm, isProjectFormValid } from "@/utils";
import { useCurrentUser } from "./useCurrentUser";

export interface FormMeta {
    area?: Area;
    type?: Type;
}

interface UseProjectFormProps {
    onSubmit: (request: ProjectProposalCreateRequest) => Promise<void>;
    onFormChange?: (form: ProjectProposalCreateRequest, meta: FormMeta) => void;
}

interface FormState {
    title: string;
    description: string;
    amount: number;
    duration: number;
    area: number;
    type: number;
}

export function useProjectForm({ onSubmit, onFormChange }: UseProjectFormProps) {
    const { currentUser } = useCurrentUser();
    const [areas, setAreas] = useState<Area[]>([]);
    const [types, setTypes] = useState<Type[]>([]);
    const [errors, setErrors] = useState<Record<string, string>>({});
    const [isSubmitting, setIsSubmitting] = useState(false);
    const onFormChangeRef = useRef(onFormChange);

    useEffect(() => {
        onFormChangeRef.current = onFormChange;
    }, [onFormChange]);

    const [form, setForm] = useState<FormState>({
        title: "",
        description: "",
        amount: 0,
        duration: 0,
        area: 0,
        type: 0,
    });

    useEffect(() => {
        let isMounted = true;
        async function loadData() {
            try {
                const [areasData, typesData] = await Promise.all([
                    getAreasAsync(),
                    getTypesAsync(),
                ]);
                if (isMounted) {
                    setAreas(areasData);
                    setTypes(typesData);
                }
            } catch {
                // Handled in services
            }
        }
        void loadData();
        return () => {
            isMounted = false;
        };
    }, []);

    function handleChange(
        event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
    ) {
        const { name, value } = event.target;

        setForm((prev) => {
            const isNumericField = ["amount", "duration", "area", "type"].includes(name);
            let parsedValue: string | number = value;

            if (isNumericField) {
                if (value === "") {
                    parsedValue = 0;
                } else {
                    const num = Number(value);
                    parsedValue = Number.isNaN(num) ? 0 : num;
                }
            }

            return {
                ...prev,
                [name]: parsedValue,
            };
        });
    }

    const fullForm: ProjectProposalCreateRequest = {
        ...form,
        user: currentUser?.id ?? 0,
    };

    useEffect(() => {
        if (onFormChangeRef.current) {
            onFormChangeRef.current(
                {
                    ...form,
                    user: currentUser?.id ?? 0,
                },
                {
                    area: areas.find((a) => a.id === form.area),
                    type: types.find((t) => t.id === form.type),
                }
            );
        }
    }, [form, currentUser, areas, types]);

    async function handleSubmit(event: React.FormEvent) {
        event.preventDefault();

        if (isSubmitting) {
            return;
        }

        const validationErrors = validateProjectForm(fullForm);
        setErrors(validationErrors);

        if (Object.keys(validationErrors).length > 0) {
            return;
        }

        try {
            setIsSubmitting(true);
            await onSubmit(fullForm);
        } finally {
            setIsSubmitting(false);
        }
    }

    return {
        areas,
        types,
        selectedUser: currentUser,
        form: fullForm,
        errors,
        isSubmitting,
        handleChange,
        handleSubmit,
        isValid: isProjectFormValid(fullForm),
    };
}
