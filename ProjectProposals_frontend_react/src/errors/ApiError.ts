
export class ApiError extends Error {
    public status: number;

    constructor(message: string, status: number) {
        super(message);
        this.status = status;

        // Necesario para que instanceof funcione correctamente
        Object.setPrototypeOf(this, ApiError.prototype);
    }
}
