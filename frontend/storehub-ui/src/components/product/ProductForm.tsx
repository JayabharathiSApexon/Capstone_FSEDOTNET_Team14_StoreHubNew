import { useEffect, useState } from "react";
import { ProductRequest } from "../../models/product/ProductRequest";
import { CategoryResponse } from "../../models/category/CategoryResponse";

interface ProductFormProps {
    product?: ProductRequest;
    categories: CategoryResponse[];
    onSubmit: (product: ProductRequest) => void;
    onCancel: () => void;
}

function ProductForm({
    product,
    categories,
    onSubmit,
    onCancel
}: ProductFormProps) {

    const [formData, setFormData] = useState<ProductRequest>({
        id: "",
        name: "",
        categoryId: "",
        description: "",
        brand: "",
        price: 0,
        stockQuantity: 0,
        isFeatured: false,
        isActive: true,
        images: []
    });

    const [errors, setErrors] = useState<Record<string, string>>({});

    useEffect(() => {

        if (product) {

            setFormData(product);

        } else {

            setFormData({
                id: "",
                name: "",
                categoryId: "",
                description: "",
                brand: "",
                price: 0,
                stockQuantity: 0,
                isFeatured: false,
                isActive: true,
                images: []
            });

            setErrors({});

        }

    }, [product]);

    const handleChange = (
        e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
    ) => {
        const { name, value, type } = e.target;

        let fieldValue: string | number | boolean = value;

        if (type === "checkbox") {
            fieldValue = (e.target as HTMLInputElement).checked;
        } else if (type === "number") {
            fieldValue = value === "" ? "" : Number(value);
        }

        setFormData(prev => ({
            ...prev,
            [name]: fieldValue
        }));

        setErrors(prev => ({
            ...prev,
            [name]: ""
        }));
    };

    const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files) {
            setFormData({
                ...formData,
                images: Array.from(e.target.files)
            });
        }
    };

    const validateForm = () => {

        const validationErrors: Record<string, string> = {};

        if (!formData.name.trim()) {
            validationErrors.name = "Product name is required.";
        }

        if (!formData.categoryId) {
            validationErrors.categoryId = "Please select a category.";
        }

        if (!formData.brand.trim()) {
            validationErrors.brand = "Brand is required.";
        }

        if (formData.price <= 0) {
            validationErrors.price = "Price must be greater than ₹0.";
        }

        if (formData.stockQuantity < 0) {
            validationErrors.stockQuantity =
                "Stock quantity cannot be negative.";
        }

        setErrors(validationErrors);

        return Object.keys(validationErrors).length === 0;
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!validateForm()) {
            return;
        }
        onSubmit(formData);
    };

    return (
        <form onSubmit={handleSubmit}>

            <div className="mb-3">
                <label className="form-label">Product Name</label>

                <input
                    type="text"
                    className={`form-control ${errors.name ? "is-invalid" : ""}`}
                    name="name"
                    value={formData.name}
                    onChange={handleChange}
                />

                {errors.name && (
                    <div className="invalid-feedback">
                        {errors.name}
                    </div>
                )}
            </div>

            <div className="row mb-3">

                <div className="col-md-6">

                    <label className="form-label">Category</label>

                    <select
                        className="form-select"
                        name="categoryId"
                        value={formData.categoryId}
                        onChange={handleChange}
                    >
                        <option value="">Select Category</option>

                        {categories.map(category => (
                            <option
                                key={category.id}
                                value={category.id}
                            >
                                {category.name}
                            </option>
                        ))}
                    </select>

                    {errors.categoryId && (
                        <div className="invalid-feedback d-block">
                            {errors.categoryId}
                        </div>
                    )}

                </div>

                <div className="col-md-6">

                    <label className="form-label">Brand</label>

                    <input
                        type="text"
                        className={`form-control ${errors.brand ? "is-invalid" : ""}`}
                        name="brand"
                        value={formData.brand}
                        onChange={handleChange}
                    />

                    {errors.brand && (
                        <div className="invalid-feedback">
                            {errors.brand}
                        </div>
                    )}

                </div>

            </div>

            <div className="mb-3">
                <label className="form-label">Description</label>

                <textarea
                    className="form-control"
                    rows={2}
                    name="description"
                    value={formData.description}
                    onChange={handleChange}
                />
            </div>

            <div className="row">

                <div className="col-md-6">
                    <label className="form-label">Price</label>

                    <input
                        type="number"
                        className={`form-control ${errors.price ? "is-invalid" : ""}`}
                        name="price"
                        value={formData.price}
                        onChange={handleChange}
                    />

                    {errors.price && (
                        <div className="invalid-feedback">
                            {errors.price}
                        </div>
                    )}
                </div>

                <div className="col-md-6">
                    <label className="form-label">Stock Quantity</label>

                    <input
                        type="number"
                        className={`form-control ${errors.stockQuantity ? "is-invalid" : ""}`}
                        name="stockQuantity"
                        value={formData.stockQuantity}
                        onChange={handleChange}
                    />

                    {errors.stockQuantity && (
                        <div className="invalid-feedback">
                            {errors.stockQuantity}
                        </div>
                    )}
                </div>

            </div>

            <div className="mb-4">

                <label className="form-label">
                    Product Image
                </label>

                <input
                    type="file"
                    className="form-control"
                    accept="image/*"
                    onChange={handleImageChange}
                />

                {formData.images.length > 0 && (
                    <small className="text-success">
                        {formData.images.length} image selected.
                    </small>
                )}

            </div>

            <div className="form-check mb-4">

                <input
                    className="form-check-input"
                    type="checkbox"
                    id="isFeatured"
                    name="isFeatured"
                    checked={formData.isFeatured}
                    onChange={handleChange}
                />

                <label
                    className="form-check-label ms-2"
                    htmlFor="isFeatured"
                >
                    Featured Product
                </label>

            </div>

            <div className="d-flex justify-content-between mt-4">

                <button
                    className="btn btn-secondary px-4"
                    type="button"
                    onClick={onCancel}
                >
                    Cancel
                </button>

                <button
                    className="btn btn-primary px-4"
                    type="submit"
                >
                    Save Product
                </button>

            </div>

        </form>
    );
}

export default ProductForm;