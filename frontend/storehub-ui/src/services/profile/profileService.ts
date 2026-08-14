import api from "../../api/axios";

import type { ProfileResponse } from "../../models/profile/ProfileResponse";
import type { UpdateProfileRequest } from "../../models/profile/UpdateProfileRequest";

export const getProfile = async (): Promise<ProfileResponse> => {
    const response = await api.get<ProfileResponse>("/Profile");

    return response.data;
};

export const updateProfile = async (
    request: UpdateProfileRequest
): Promise<void> => {
    await api.put("/Profile", request);
};