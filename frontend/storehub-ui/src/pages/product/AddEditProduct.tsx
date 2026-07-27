import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Layout from "../../components/common/Layout";
import ProductForm from "../../components/product/ProductForm";
import { CategoryResponse } from "../../models/category/CategoryResponse";
import { ProductRequest } from "../../models/product/ProductRequest";
import { getCategories } from "../../services/categoryService";
import { createProduct } from "../../services/productService";

function AddEditProduct() {
    const navigate = useNavigate();

    const [categories, setCategories] = useState<CategoryResponse[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadCategories();
    }, []);

    const loadCategories = async () => {
        try {
            const data = await getCategories();
            setCategories(data);
        } catch (error) {
            console.error("Failed to load categories", error);
        } finally {
            setLoading(false);
        }
    };

    const handleSubmit = async (product: ProductRequest) => {
        try {
            await createProduct(product);

            alert("Product created successfully.");

            navigate("/products");
        } catch (error) {
            console.error(error);
            alert("Failed to create product.");
        }
    };

    if (loading) {
        return (
            <Layout>
                <p>Loading...</p>
            </Layout>
        );
    }

    return (
        <Layout>
            <div className="container">

                <div className="d-flex justify-content-between align-items-center mb-4">
                    <h3>Add Product</h3>
                </div>

                <ProductForm
                    categories={categories}
                    onSubmit={handleSubmit}
                    onCancel={() => navigate("/products")}
                />

            </div>
        </Layout>
    );
}

export default AddEditProduct;