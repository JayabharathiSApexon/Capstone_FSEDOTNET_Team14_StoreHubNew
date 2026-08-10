import { AuthResponse, AuthUser } from "../../models/auth/AuthModels";
import {
    clearAuth,
    getAuthToken,
    getAuthUser,
    markSessionLogin,
    setAuth
} from "./authStorage";

export const persistAuth = (response: AuthResponse): void => {
    setAuth(response);
};

export const getCurrentUser = (): AuthUser | null => {
    return getAuthUser();
};

export const isAuthenticated = (): boolean => {
    const token = getAuthToken();
    const user = getAuthUser();

    if (!token || !user) {
        return false;
    }

    return new Date(user.expiresAtUtc).getTime() > Date.now();
};

export const rememberSessionLogin = (): void => {
    markSessionLogin();
};

export const logout = (): void => {
    clearAuth();
};
