import { CategoryResponse } from "../models/category/CategoryResponse";
import { getCategoriesRequest } from "./category/categoryApiService";

export const getCategories = async (): Promise<CategoryResponse[]> => {
    return await getCategoriesRequest();
};