import { ProductImageResponse } from "./ProductImageResponse";

export interface ProductResponse {
    id: string;
    categoryId: string;
    categoryName: string;
    name: string;
    description: string;
    brand: string;
    price: number;
    stockQuantity: number;
    isFeatured: boolean;
    isActive: boolean;
    createdDate: string;
    updatedDate?: string;

    images: ProductImageResponse[];
}