import {
    AuthResponse,
    AuthUser,
    ForgotPasswordRequest,
    LoginRequest,
    RegisterRequest,
    ResetPasswordRequest
} from "../models/auth/AuthModels";
import {
    forgotPasswordRequest,
    loginRequest,
    registerRequest,
    resetPasswordRequest
} from "./auth/authApiService";
import {
    getCurrentUser as getSessionUser,
    isAuthenticated as isSessionAuthenticated,
    logout as clearSession,
    persistAuth,
    rememberSessionLogin
} from "./auth/authSessionService";

export const login = async (request: LoginRequest): Promise<AuthResponse> => {
    const response = await loginRequest(request);
    persistAuth(response);
    return response;
};

export const register = async (request: RegisterRequest): Promise<AuthResponse> => {
    const response = await registerRequest(request);
    persistAuth(response);
    return response;
};

export const forgotPassword = async (
    request: ForgotPasswordRequest
): Promise<{ message: string }> => {
    return await forgotPasswordRequest(request);
};

export const resetPassword = async (
    request: ResetPasswordRequest
): Promise<{ message: string }> => {
    return await resetPasswordRequest(request);
};

export const saveAuth = (response: AuthResponse): void => {
    persistAuth(response);
};

export const getCurrentUser = (): AuthUser | null => {
    return getSessionUser();
};

export const isAuthenticated = (): boolean => {
    return isSessionAuthenticated();
};

export const saveSessionLoginPreference = (): void => {
    rememberSessionLogin();
};

export const logout = (): void => {
    clearSession();
};