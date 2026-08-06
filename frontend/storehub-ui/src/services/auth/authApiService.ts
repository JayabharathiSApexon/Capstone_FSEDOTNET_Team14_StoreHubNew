import api from "../../api/axios";
import {
    AuthResponse,
    ForgotPasswordRequest,
    LoginRequest,
    RegisterRequest,
    ResetPasswordRequest
} from "../../models/auth/AuthModels";

export const loginRequest = async (request: LoginRequest): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>("/Auth/login", request);
    return response.data;
};

export const registerRequest = async (request: RegisterRequest): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>("/Auth/register", request);
    return response.data;
};

export const forgotPasswordRequest = async (
    request: ForgotPasswordRequest
): Promise<{ message: string }> => {
    const response = await api.post<{ message: string }>("/Auth/forgot-password", request);
    return response.data;
};

export const resetPasswordRequest = async (
    request: ResetPasswordRequest
): Promise<{ message: string }> => {
    const response = await api.post<{ message: string }>("/Auth/reset-password", request);
    return response.data;
};
