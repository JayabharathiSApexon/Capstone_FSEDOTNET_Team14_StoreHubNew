import api from "../api/axios";
import { ProductRequest } from "../models/product/ProductRequest";
import { ProductResponse } from "../models/product/ProductResponse";

export const getProducts = async (): Promise<ProductResponse[]> => {
    const response = await api.get<ProductResponse[]>("/Product");
    return response.data;
};

export const getProductById = async (id: string): Promise<ProductResponse> => {
    const response = await api.get<ProductResponse>(`/Product/${id}`);
    return response.data;
};

export const createProduct = async (product: ProductRequest) => {

    const formData = new FormData();

    formData.append("name", product.name);
    formData.append("categoryId", product.categoryId);
    formData.append("description", product.description);
    formData.append("brand", product.brand);
    formData.append("price", product.price.toString());
    formData.append("stockQuantity", product.stockQuantity.toString());
    formData.append("isFeatured", product.isFeatured.toString());
    formData.append("isActive", product.isActive.toString());

    product.images.forEach(image => {
        formData.append("images", image);
    });

    const response = await api.post(
        "/Product",
        formData,
        {
            headers: {
                "Content-Type": "multipart/form-data"
            }
        });

    return response.data;
};

export const updateProduct = async (
    id: string,
    product: ProductRequest
) => {

    const formData = new FormData();

    formData.append("id", product.id ?? "");
    formData.append("name", product.name);
    formData.append("categoryId", product.categoryId);
    formData.append("description", product.description);
    formData.append("brand", product.brand);
    formData.append("price", product.price.toString());
    formData.append("stockQuantity", product.stockQuantity.toString());
    formData.append("isFeatured", product.isFeatured.toString());
    formData.append("isActive", product.isActive.toString());

    product.images.forEach(image => {
        formData.append("images", image);
    });

    const response = await api.put(
        `/Product/${id}`,
        formData,
        {
            headers: {
                "Content-Type": "multipart/form-data"
            }
        });

    return response.data;
};

export const deleteProduct = async (id: string) => {
    const response = await api.delete(`/Product/${id}`);
    return response.data;
};