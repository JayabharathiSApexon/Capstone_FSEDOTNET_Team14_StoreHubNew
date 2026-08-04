import api from "../api/axios";
import UserListItem from "../models/user/UserListItem";

export const getUsers = async (): Promise<UserListItem[]> => {
    const response = await api.get<UserListItem[]>("/Users");
    return response.data;
};