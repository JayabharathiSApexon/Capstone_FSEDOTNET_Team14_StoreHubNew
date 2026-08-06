export const getApiErrorMessage = (
    error: unknown,
    fallbackMessage: string
): string => {
    if (typeof error !== "object" || error === null) {
        return fallbackMessage;
    }

    const maybeError = error as {
        response?: {
            data?: {
                message?: unknown;
            };
        };
    };

    const message = maybeError.response?.data?.message;
    return typeof message === "string" ? message : fallbackMessage;
};
