import axios from "axios";
import {
    clearAuth,
    getAuthToken
} from "../services/auth/authStorage";

console.log("API URL:", import.meta.env.VITE_API_URL);

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    headers: {
        "Content-Type": "application/json",
    },
});

api.interceptors.request.use(config => {
    const token = getAuthToken();

    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
});

api.interceptors.response.use(
    response => response,
    error => {
        const statusCode = error?.response?.status;
        const token = getAuthToken();

        // If the token is stale/invalid, clear local auth state and force a clean login flow.
        if (statusCode === 401 && token) {
            clearAuth();

            const currentPath = window.location.pathname;
            if (currentPath !== "/login") {
                window.location.href = "/login";
            }
        }

        return Promise.reject(error);
    }
);

export default api;