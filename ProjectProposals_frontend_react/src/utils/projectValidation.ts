import type { ProjectProposalCreateRequest } from "@/api/requests/ProjectProposalCreateRequest";

export type FormErrors = Record<string, string>;

export function validateProjectForm(
    form: ProjectProposalCreateRequest
): FormErrors {
    const errors: FormErrors = {};

    if (!form.title.trim()) {
        errors.title = "Title is required";
    } else if (form.title.trim().length > 255) {
        errors.title = "Title cannot exceed 255 characters";
    }

    if (!form.description.trim()) {
        errors.description = "Description is required";
    } else if (form.description.trim().length > 255) {
        errors.description = "Description cannot exceed 255 characters";
    }

    if (!Number.isFinite(form.amount)) {
        errors.amount = "Amount must be a valid number";
    } else if (form.amount <= 0) {
        errors.amount = "Amount must be greater than zero";
    }

    if (!Number.isFinite(form.duration)) {
        errors.duration = "Duration must be a valid number";
    } else if (form.duration <= 0) {
        errors.duration = "Duration must be greater than zero";
    }

    if (form.area <= 0) {
        errors.area = "Please select an area";
    }

    if (form.type <= 0) {
        errors.type = "Please select a type";
    }

    if (form.user <= 0) {
        errors.user = "Please select an active user";
    }

    return errors;
}

export function isProjectFormValid(
    form: ProjectProposalCreateRequest
): boolean {
    return Object.keys(validateProjectForm(form)).length === 0;
}
