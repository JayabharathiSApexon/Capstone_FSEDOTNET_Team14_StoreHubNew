import { getAuthUser as getStoredAuthUser } from "../services/auth/authStorage";
import { AuthUser } from "../models/auth/AuthModels";

export const getAuthUser = (): AuthUser | null => {
    return getStoredAuthUser();
};
