import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Layout from "../../components/common/Layout";
import ProductTable from "../../components/product/ProductTable";
import { ProductResponse } from "../../models/product/ProductResponse";
import { getProducts } from "../../services/productService";

function ProductList() {
    const [products, setProducts] = useState<ProductResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        loadProducts();
    }, []);

    const loadProducts = async () => {
        try {
            const data = await getProducts();
            setProducts(data);
        } catch (error) {
            console.error("Failed to load products:", error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <Layout>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="mb-0">Product Management</h3>
                    <small className="text-muted">
                        Manage all products
                    </small>
                </div>

                <button
                    className="btn btn-primary"
                    onClick={() => navigate("/products/add")}
                >
                    + Add Product
                </button>
            </div>

            <div className="card shadow-sm">
                <div className="card-body">
                    {loading ? (
                        <p>Loading products...</p>
                    ) : (
                        <ProductTable products={products} />
                    )}
                </div>
            </div>
        </Layout>
    );
}

export default ProductList;