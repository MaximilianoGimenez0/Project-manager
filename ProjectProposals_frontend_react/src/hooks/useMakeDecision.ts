import { useEffect, useState } from "react";
import type { Status } from "@/models";
import type { DecisionStepUpdateRequest } from "@/api";
import { getStatusAsync } from "@/services";
import { useCurrentUser } from "./useCurrentUser";

interface UseMakeDecisionProps {
    stepId: number;
    onSubmit: (request: DecisionStepUpdateRequest) => Promise<void>;
}

export function useMakeDecision({ stepId, onSubmit }: UseMakeDecisionProps) {
    const { currentUser } = useCurrentUser();
    const [statuses, setStatuses] = useState<Status[]>([]);
    const [errors, setErrors] = useState<Record<string, string>>({});
    const [isSubmitting, setIsSubmitting] = useState(false);

    const [form, setForm] = useState({
        status: 0,
        observation: "",
    });

    useEffect(() => {
        let isMounted = true;
        async function loadData() {
            try {
                const statusesData = await getStatusAsync();
                if (isMounted) {
                    setStatuses(statusesData);
                }
            } catch {
                // Handled gracefully
            }
        }
        void loadData();
        return () => {
            isMounted = false;
        };
    }, []);

    function handleChange(
        event: React.ChangeEvent<
            HTMLInputElement |
            HTMLTextAreaElement |
            HTMLSelectElement
        >
    ) {
        const { name, value } = event.target;

        setForm((prev) => ({
            ...prev,
            [name]: name === "status" ? Number(value) : value,
        }));
    }

    async function handleSubmit(event: React.FormEvent) {
        event.preventDefault();

        if (isSubmitting) {
            return;
        }

        const request: DecisionStepUpdateRequest = {
            id: stepId,
            user: currentUser?.id ?? 0,
            status: form.status,
            observation: form.observation,
        };

        const validationErrors = validateDecisionForm(request);
        setErrors(validationErrors);

        if (Object.keys(validationErrors).length > 0) {
            return;
        }

        try {
            setIsSubmitting(true);
            await onSubmit(request);
        } finally {
            setIsSubmitting(false);
        }
    }

    const fullForm: DecisionStepUpdateRequest = {
        id: stepId,
        user: currentUser?.id ?? 0,
        status: form.status,
        observation: form.observation,
    };

    return {
        statuses,
        selectedUser: currentUser,
        form: fullForm,
        errors,
        isSubmitting,
        handleChange,
        handleSubmit,
        isValid: isDecisionFormValid(form.status, form.observation),
    };
}

function validateDecisionForm(form: DecisionStepUpdateRequest): Record<string, string> {
    const errors: Record<string, string> = {};

    if (form.status === 0) {
        errors.status = "Please select a status";
    }

    if (form.observation.trim() === "") {
        errors.observation = "Observation is required";
    }

    return errors;
}

function isDecisionFormValid(status: number, observation: string): boolean {
    return status !== 0 && observation.trim() !== "";
}