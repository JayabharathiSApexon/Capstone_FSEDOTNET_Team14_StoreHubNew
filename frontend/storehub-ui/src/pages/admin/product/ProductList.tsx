import { useEffect, useState } from "react";
import Layout from "../../../components/admin/AdminLayout";
import ProductTable from "../../../components/admin/product/ProductTable";
import ProductModal from "../../../components/admin/product/ProductModal";
import MessageModal from "../../../components/common/MessageModal";
import ConfirmModal from "../../../components/common/ConfirmModal";
import Pagination from "../../../components/common/Pagination";
import { ProductRequest } from "../../../models/product/ProductRequest";
import { ProductResponse } from "../../../models/product/ProductResponse";
import { CategoryResponse } from "../../../models/category/CategoryResponse";
import { createProduct, updateProduct, getProducts, deleteProduct } from "../../../services/productService";
import { getCategories } from "../../../services/categoryService";

function ProductList() {

    const [products, setProducts] = useState<ProductResponse[]>([]);
    const [categories, setCategories] = useState<CategoryResponse[]>([]);
    const [loading, setLoading] = useState(true);

    const [showModal, setShowModal] = useState(false);
    const [showMessageModal, setShowMessageModal] = useState(false);
    const [messageTitle, setMessageTitle] = useState("");
    const [messageText, setMessageText] = useState("");
    const [messageType, setMessageType] = useState<"success" | "danger">("success");

    const [showConfirmModal, setShowConfirmModal] = useState(false);
    const [deleteProductId, setDeleteProductId] = useState("");
    const [deleteProductName, setDeleteProductName] = useState("");
    const [selectedProduct, setSelectedProduct] = useState<ProductRequest | undefined>(undefined);

    const itemsPerPage = 5;
    const [currentPage, setCurrentPage] = useState(1);

    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentProducts = products.slice(indexOfFirstItem, indexOfLastItem);

    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {

        try {

            const [productData, categoryData] =
                await Promise.all([

                    getProducts(),
                    getCategories()

                ]);

            setProducts(productData);
            setCategories(categoryData);

            setCurrentPage(1);
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

    const openEditModal = ( product: ProductResponse ) => {

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

    const handleSubmit = async ( product: ProductRequest ) => {

        try {

            if (product.id) {

                await updateProduct(
                    product.id,
                    product
                );

                setMessageText(
                    `Product "${product.name}" updated successfully.`
                );

            }
            else {

                await createProduct(product);

                setMessageText(
                    `Product "${product.name}" created successfully.`
                );

            }

            closeModal();

            await loadData();

            setMessageTitle("Success");

            setMessageType("success");
            setShowMessageModal(true);

        }
        catch (error) {

            console.error(error);

            setMessageTitle("Operation Failed");

            setMessageText(
                `Unable to ${product.id ? "update" : "create"} the product.`
            );
            
            setMessageType("danger");
            setShowMessageModal(true);
        }

    };

    const handleDelete = (
        id: string,
        name: string
    ) => {

        setDeleteProductId(id);

        setDeleteProductName(name);

        setShowConfirmModal(true);

    };

    const confirmDelete = async () => {

        try {

            await deleteProduct(deleteProductId);

            setShowConfirmModal(false);

            await loadData();

            setMessageTitle("Success");

            setMessageText(
                `Product "${deleteProductName}" deleted successfully.`
            );

            setMessageType("success");

            setShowMessageModal(true);

        }
        catch (error) {

            console.error(error);

            setShowConfirmModal(false);

            setMessageTitle("Delete Failed");

            setMessageText(
                `Unable to delete "${deleteProductName}".`
            );

            setMessageType("danger");

            setShowMessageModal(true);

        }

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
                            ? (
                                <p>
                                    Loading...
                                </p>
                            )
                            : (
                                <>
                                    <ProductTable
                                        products={currentProducts}
                                        onEdit={openEditModal}
                                        onDelete={handleDelete}
                                    />

                                    <Pagination
                                        currentPage={currentPage}
                                        totalItems={products.length}
                                        itemsPerPage={itemsPerPage}
                                        onPageChange={setCurrentPage}
                                    />
                                </>
                            )
                    }

                </div>

            </div>

            <ProductModal
                show={showModal}
                title={
                    selectedProduct
                        ? "Edit Product"
                        : "Add Product"
                }
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
                onClose={() =>
                    setShowMessageModal(false)
                }
            />

            <ConfirmModal
                show={showConfirmModal}
                title="Delete Product"
                message={`Are you sure you want to delete "${deleteProductName}"?`}
                onConfirm={confirmDelete}
                onCancel={() =>
                    setShowConfirmModal(false)
                }
            />

        </Layout>

    );

}

export default ProductList;