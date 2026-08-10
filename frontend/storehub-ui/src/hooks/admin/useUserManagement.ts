import { useEffect, useState } from "react";
import UserListItem from "../../models/user/UserListItem";
import { getUsers } from "../../services/userService";
import { useAsyncState } from "../common/useAsyncState";

export const useUserManagement = () => {
    const [users, setUsers] = useState<UserListItem[]>([]);
    const { loading, error, runSafely } = useAsyncState(true);

    useEffect(() => {
        void loadUsers();
    }, []);

    const loadUsers = async () => {
        const userData = await runSafely(
            async () => await getUsers(),
            { defaultErrorMessage: "Failed to load users." }
        );

        if (userData) {
            setUsers(userData);
        }
    };

    return {
        users,
        loading,
        error,
        reloadUsers: loadUsers
    };
};
