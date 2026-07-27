import api from "../api/axios";
import { CategoryResponse } from "../models/category/CategoryResponse";

export const getCategories = async (): Promise<CategoryResponse[]> => {
    const response = await api.get<CategoryResponse[]>("/Category");
    return response.data;
};