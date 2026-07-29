import { useEffect, useState } from "react";
import Layout from "../../components/common/Layout";
import ProductTable from "../../components/product/ProductTable";
import ProductModal from "../../components/product/ProductModal";
import MessageModal from "../../components/common/MessageModal";
import ConfirmModal from "../../components/common/ConfirmModal";
import { ProductRequest } from "../../models/product/ProductRequest";
import { ProductResponse } from "../../models/product/ProductResponse";
import { createProduct, updateProduct, getProducts, deleteProduct } from "../../services/productService";
import { getCategories } from "../../services/categoryService";
import { CategoryResponse } from "../../models/category/CategoryResponse";

function ProductList() {

    const [products, setProducts] = useState<ProductResponse[]>([]);
    const [categories, setCategories] = useState<CategoryResponse[]>([]);
    const [loading, setLoading] = useState(true);

    const [showModal, setShowModal] = useState(false);
    const [showMessageModal, setShowMessageModal] = useState(false);
    const [messageTitle, setMessageTitle] = useState("");
    const [messageText, setMessageText] = useState("");
    const [messageType, setMessageType] =
        useState<"success" | "danger">("success");

    const [showConfirmModal, setShowConfirmModal] = useState(false);
    const [deleteProductId, setDeleteProductId] = useState("");
    const [deleteProductName, setDeleteProductName] = useState("");

    const [selectedProduct, setSelectedProduct] =
        useState<ProductRequest | undefined>(undefined);

    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {

        try {

            const [productData, categoryData] = await Promise.all([
                getProducts(),
                getCategories()
            ]);

            setProducts(productData);
            setCategories(categoryData);

        }
        catch (error) {

            console.error(error);

        }
        finally {

            setLoading(false);

        }
    };

    const openAddModal = () => {

        setSelectedProduct(undefined);
        setShowModal(true);

    };

    const closeModal = () => {

        setShowModal(false);

    };

    const openEditModal = (product: ProductResponse) => {

        setSelectedProduct({

            id: product.id,
            name: product.name,
            categoryId: product.categoryId,
            description: product.description,
            brand: product.brand,
            price: product.price,
            stockQuantity: product.stockQuantity,
            isFeatured: product.isFeatured,
            isActive: product.isActive,
            images: []

        });

        setShowModal(true);

    };

    const handleSubmit = async (product: ProductRequest) => {

        try {

            if (product.id) {

                await updateProduct(product.id, product);

                closeModal();

                await loadData();

                setMessageTitle("Success");
                setMessageText(`Product "${product.name}" has been updated successfully.`);
            }
            else {

                await createProduct(product);

                closeModal();

                await loadData();

                setMessageTitle("Success");
                setMessageText(`Product "${product.name}" has been created successfully.`);
            }

            setMessageType("success");
            setShowMessageModal(true);

        }
        catch (error) {

            console.error(error);

            setMessageTitle("Operation Failed");
            setMessageText(
                `Unable to ${product.id ? "update" : "create"} the product. Please try again.`
            );
            setMessageType("danger");
            setShowMessageModal(true);
        }
    };

    const confirmDelete = async () => {

        try {

            await deleteProduct(deleteProductId);

            setShowConfirmModal(false);

            await loadData();

            setMessageTitle("Success");
            setMessageText(
                `Product "${deleteProductName}" has been deleted successfully.`
            );
            setMessageType("success");
            setShowMessageModal(true);

        }
        catch (error) {

            console.error(error);

            setShowConfirmModal(false);

            setMessageTitle("Delete Failed");
            setMessageText(
                `Unable to delete "${deleteProductName}". Please try again.`
            );
            setMessageType("danger");
            setShowMessageModal(true);

        }

    };

    const handleDelete = (id: string, name: string) => {

        setDeleteProductId(id);
        setDeleteProductName(name);

        setShowConfirmModal(true);

    };

    return (

        <Layout>

            <div className="d-flex justify-content-between align-items-center mb-4">

                <div>

                    <h3 className="mb-0">
                        Product Management
                    </h3>

                    <small className="text-muted">
                        Manage all products
                    </small>

                </div>

                <button
                    className="btn btn-primary"
                    onClick={openAddModal}
                >
                    + Add Product
                </button>

            </div>

            <div className="card shadow-sm">

                <div className="card-body">

                    {
                        loading
                            ? <p>Loading...</p>
                            : <ProductTable
                                products={products}
                                onEdit={openEditModal}
                                onDelete={handleDelete}
                            />
                    }

                </div>

            </div>

            <ProductModal
                show={showModal}
                title={selectedProduct ? "Edit Product" : "Add Product"}
                product={selectedProduct}
                categories={categories}
                onSubmit={handleSubmit}
                onClose={closeModal}
            />

            <MessageModal
                show={showMessageModal}
                title={messageTitle}
                message={messageText}
                variant={messageType}
                onClose={() => setShowMessageModal(false)}
            />

            <ConfirmModal
                show={showConfirmModal}
                title="Delete Product"
                message={`Are you sure you want to delete "${deleteProductName}"?`}
                onConfirm={confirmDelete}
                onCancel={() => setShowConfirmModal(false)}
            />

        </Layout>

    );

}

export default ProductList;