import { useCallback, useState } from "react";
import { getApiErrorMessage } from "../../utils/apiError";

interface RunSafelyOptions {
    defaultErrorMessage?: string;
    rethrow?: boolean;
    onError?: (error: unknown) => void;
}

export const useAsyncState = (initialLoading = false) => {
    const [loading, setLoading] = useState(initialLoading);
    const [error, setError] = useState("");

    const runSafely = useCallback(async <T>(
        action: () => Promise<T>,
        options?: RunSafelyOptions
    ): Promise<T | undefined> => {
        setLoading(true);
        setError("");

        try {
            return await action();
        }
        catch (errorValue) {
            options?.onError?.(errorValue);

            const fallbackMessage = options?.defaultErrorMessage ?? "Operation failed.";
            const message = getApiErrorMessage(errorValue, fallbackMessage);

            setError(message);

            if (options?.rethrow) {
                throw errorValue;
            }

            return undefined;
        }
        finally {
            setLoading(false);
        }
    }, []);

    const clearError = useCallback(() => {
        setError("");
    }, []);

    return {
        loading,
        error,
        runSafely,
        clearError
    };
};
