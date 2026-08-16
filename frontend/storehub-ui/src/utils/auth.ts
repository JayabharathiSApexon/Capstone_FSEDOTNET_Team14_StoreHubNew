export interface AuthUser {
    userId: string;
    email: string;
    name?: string;
    roles?: string[];
}

export const getAuthUser = (): AuthUser | null => {
    try {
        const userJson = localStorage.getItem("authUser");
        if (userJson) {
            return JSON.parse(userJson) as AuthUser;
        }
    } catch (error) {
        console.error("Error retrieving auth user:", error);
    }
    return null;
};

export const setAuthUser = (user: AuthUser): void => {
    try {
        localStorage.setItem("authUser", JSON.stringify(user));
    } catch (error) {
        console.error("Error saving auth user:", error);
    }
};

export const clearAuthUser = (): void => {
    try {
        localStorage.removeItem("authUser");
    } catch (error) {
        console.error("Error clearing auth user:", error);
    }
};
