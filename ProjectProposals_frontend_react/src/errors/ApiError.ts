
export class ApiError extends Error {
    public status: number;

    constructor(message: string, status: number) {
        super(message);
        this.status = status;

        // Restore prototype chain broken when extending built-in Error in ES5 targets
        Object.setPrototypeOf(this, ApiError.prototype);
    }
}
