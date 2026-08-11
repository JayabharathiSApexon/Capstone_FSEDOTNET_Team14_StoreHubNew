import api from "../../api/axios";
import { CategoryResponse } from "../../models/category/CategoryResponse";

export const getCategoriesRequest = async (): Promise<CategoryResponse[]> => {
    const response = await api.get<CategoryResponse[]>("/Category");
    return response.data;
};
