import axios from "axios";
import { getAuthToken } from "../services/auth/authStorage";

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

export default api;