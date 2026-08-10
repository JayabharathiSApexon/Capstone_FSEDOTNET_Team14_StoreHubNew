export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    fullName: string;
    email: string;
    password: string;
    phoneNumber?: string;
}

export interface AuthResponse {
    token: string;
    expiresAtUtc: string;
    userId: string;
    fullName: string;
    email: string;
    isAdmin: boolean;
}

export interface AuthUser {
    userId: string;
    fullName: string;
    email: string;
    isAdmin: boolean;
    expiresAtUtc: string;
}

export interface ForgotPasswordRequest {
    email: string;
    newPassword: string;
    confirmPassword: string;
}

export interface ResetPasswordRequest {
    email: string;
    newPassword: string;
    confirmPassword: string;
}