import { ProductRequest } from "../models/product/ProductRequest";
import { ProductResponse } from "../models/product/ProductResponse";
import {
    createProductRequest,
    deleteProductRequest,
    getProductByIdRequest,
    getProductsRequest,
    updateProductRequest
} from "./product/productApiService";
import {
    buildCreateProductFormData,
    buildUpdateProductFormData
} from "./product/productFormDataBuilder";

export const getProducts = async (): Promise<ProductResponse[]> => {
    return await getProductsRequest();
};

export const getProductById = async (id: string): Promise<ProductResponse> => {
    return await getProductByIdRequest(id);
};

export const createProduct = async (product: ProductRequest) => {
    const formData = buildCreateProductFormData(product);
    return await createProductRequest(formData);
};

export const updateProduct = async (
    id: string,
    product: ProductRequest
) => {
    const formData = buildUpdateProductFormData(product);
    return await updateProductRequest(id, formData);
};

export const deleteProduct = async (id: string) => {
    return await deleteProductRequest(id);
};