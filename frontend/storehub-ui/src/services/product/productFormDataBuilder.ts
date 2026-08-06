import { ProductRequest } from "../../models/product/ProductRequest";

const appendBaseFields = (formData: FormData, product: ProductRequest): void => {
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
};

export const buildCreateProductFormData = (product: ProductRequest): FormData => {
    const formData = new FormData();
    appendBaseFields(formData, product);
    return formData;
};

export const buildUpdateProductFormData = (product: ProductRequest): FormData => {
    const formData = new FormData();
    formData.append("id", product.id ?? "");
    appendBaseFields(formData, product);
    return formData;
};
