import api from "../api/axios";
import {
    AuthResponse,
    AuthUser,
    ForgotPasswordRequest,
    LoginRequest,
    RegisterRequest,
    ResetPasswordRequest
} from "../models/auth/AuthModels";

const AUTH_TOKEN_KEY = "storehub_auth_token";
const AUTH_USER_KEY = "storehub_auth_user";

export const login = async (request: LoginRequest): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>("/Auth/login", request);
    saveAuth(response.data);

    return response.data;
};

export const register = async (request: RegisterRequest): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>("/Auth/register", request);
    saveAuth(response.data);

    return response.data;
};

export const forgotPassword = async (
    request: ForgotPasswordRequest
): Promise<{ message: string }> => {
    const response = await api.post<{ message: string }>("/Auth/forgot-password", request);
    return response.data;
};

export const resetPassword = async (
    request: ResetPasswordRequest
): Promise<{ message: string }> => {
    const response = await api.post<{ message: string }>("/Auth/reset-password", request);
    return response.data;
};

export const saveAuth = (response: AuthResponse): void => {
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

export const getCurrentUser = (): AuthUser | null => {
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

export const isAuthenticated = (): boolean => {
    const token = getAuthToken();
    const user = getCurrentUser();

    if (!token || !user) {
        return false;
    }

    return new Date(user.expiresAtUtc).getTime() > Date.now();
};

export const logout = (): void => {
    localStorage.removeItem(AUTH_TOKEN_KEY);
    localStorage.removeItem(AUTH_USER_KEY);
    sessionStorage.removeItem("storehub_session_login");
};