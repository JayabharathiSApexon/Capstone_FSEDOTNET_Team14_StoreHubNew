export interface ProductRequest {
    id?: string;
    name: string;
    categoryId: string;
    description: string;
    brand: string;
    price: number;
    stockQuantity: number;
    isFeatured: boolean;
    isActive: boolean;
    images: File[];
}