import UserListItem from "../models/user/UserListItem";
import { getUsersRequest } from "./user/userApiService";

export const getUsers = async (): Promise<UserListItem[]> => {
    return await getUsersRequest();
};