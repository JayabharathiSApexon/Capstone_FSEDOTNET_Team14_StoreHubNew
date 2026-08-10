import Layout from "../../../components/admin/AdminLayout";
import ProductTable from "../../../components/admin/product/ProductTable";
import ProductModal from "../../../components/admin/product/ProductModal";
import MessageModal from "../../../components/common/MessageModal";
import ConfirmModal from "../../../components/common/ConfirmModal";
import { useProductManagement } from "../../../hooks/admin/useProductManagement";

function ProductList() {
    const {
        products,
        categories,
        loading,
        error,
        showModal,
        showMessageModal,
        messageTitle,
        messageText,
        messageType,
        showConfirmModal,
        deleteProductName,
        selectedProduct,
        openAddModal,
        closeModal,
        openEditModal,
        handleSubmit,
        confirmDelete,
        handleDelete,
        setShowConfirmModal,
        closeMessageModal
    } = useProductManagement();

    return (

        <Layout>

            <div className="d-flex justify-content-between align-items-center mb-4">

                <div>

                    <h4 className="mb-0">
                        Product Management
                    </h4>

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
                            : error
                                ? <div className="alert alert-danger mb-0">{error}</div>
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
                onClose={closeMessageModal}
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