import api from "../../api/axios";
import { ProductResponse } from "../../models/product/ProductResponse";

const multipartHeaders = {
    headers: {
        "Content-Type": "multipart/form-data"
    }
};

export const getProductsRequest = async (): Promise<ProductResponse[]> => {
    const response = await api.get<ProductResponse[]>("/Product");
    return response.data;
};

export const getProductByIdRequest = async (id: string): Promise<ProductResponse> => {
    const response = await api.get<ProductResponse>(`/Product/${id}`);
    return response.data;
};

export const createProductRequest = async (formData: FormData) => {
    const response = await api.post("/Product", formData, multipartHeaders);
    return response.data;
};

export const updateProductRequest = async (id: string, formData: FormData) => {
    const response = await api.put(`/Product/${id}`, formData, multipartHeaders);
    return response.data;
};

export const deleteProductRequest = async (id: string) => {
    const response = await api.delete(`/Product/${id}`);
    return response.data;
};
