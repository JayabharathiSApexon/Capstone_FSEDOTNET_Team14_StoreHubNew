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

    useEffect(() => {
        if (product) {
            setFormData(product);
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
            fieldValue = value === "" ? 0 : Number(value);
        }

        setFormData({
            ...formData,
            [name]: fieldValue
        });
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSubmit(formData);
    };

    return (
        <form onSubmit={handleSubmit}>

            <div className="mb-3">
                <label className="form-label">Product Name</label>

                <input
                    type="text"
                    className="form-control"
                    name="name"
                    value={formData.name}
                    onChange={handleChange}
                />
            </div>

            <div className="mb-3">
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
            </div>

            <div className="mb-3">
                <label className="form-label">Brand</label>

                <input
                    type="text"
                    className="form-control"
                    name="brand"
                    value={formData.brand}
                    onChange={handleChange}
                />
            </div>

            <div className="mb-3">
                <label className="form-label">Description</label>

                <textarea
                    className="form-control"
                    rows={3}
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
                        className="form-control"
                        name="price"
                        value={formData.price}
                        onChange={handleChange}
                    />
                </div>

                <div className="col-md-6">
                    <label className="form-label">Stock Quantity</label>

                    <input
                        type="number"
                        className="form-control"
                        name="stockQuantity"
                        value={formData.stockQuantity}
                        onChange={handleChange}
                    />
                </div>

            </div>

            <div className="form-check mt-3">

                <input
                    className="form-check-input"
                    type="checkbox"
                    name="isFeatured"
                    checked={formData.isFeatured}
                    onChange={handleChange}
                />

                <label className="form-check-label">
                    Featured Product
                </label>

            </div>

            <div className="mt-4">

                <button
                    className="btn btn-primary me-2"
                    type="submit"
                >
                    Save Product
                </button>

                <button
                    className="btn btn-secondary"
                    type="button"
                    onClick={onCancel}
                >
                    Cancel
                </button>

            </div>

        </form>
    );
}

export default ProductForm;