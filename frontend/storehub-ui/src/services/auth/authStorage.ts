import { AuthResponse, AuthUser } from "../../models/auth/AuthModels";

const AUTH_TOKEN_KEY = "storehub_auth_token";
const AUTH_USER_KEY = "storehub_auth_user";
const SESSION_LOGIN_KEY = "storehub_session_login";

export const setAuth = (response: AuthResponse): void => {
    const user: AuthUser = {
        userId: response.userId,
        fullName: response.fullName,
        email: response.email,
        isAdmin: response.isAdmin,
        expiresAtUtc: response.expiresAtUtc
    };

    localStorage.setItem(AUTH_TOKEN_KEY, response.token);
    localStorage.setItem(AUTH_USER_KEY, JSON.stringify(user));
};

export const getAuthToken = (): string | null => {
    return localStorage.getItem(AUTH_TOKEN_KEY);
};

export const getAuthUser = (): AuthUser | null => {
    const rawUser = localStorage.getItem(AUTH_USER_KEY);

    if (!rawUser) {
        return null;
    }

    try {
        return JSON.parse(rawUser) as AuthUser;
    }
    catch {
        return null;
    }
};

export const clearAuth = (): void => {
    localStorage.removeItem(AUTH_TOKEN_KEY);
    localStorage.removeItem(AUTH_USER_KEY);
    sessionStorage.removeItem(SESSION_LOGIN_KEY);
};

export const markSessionLogin = (): void => {
    sessionStorage.setItem(SESSION_LOGIN_KEY, "true");
};
