import { useEffect, useState } from "react";
import { CategoryResponse } from "../../models/category/CategoryResponse";
import { ProductRequest } from "../../models/product/ProductRequest";
import { ProductResponse } from "../../models/product/ProductResponse";
import { getCategories } from "../../services/categoryService";
import {
    createProduct,
    deleteProduct,
    getProducts,
    updateProduct
} from "../../services/productService";
import { getApiErrorMessage } from "../../utils/apiError";
import { useAsyncState } from "../common/useAsyncState";

export const useProductManagement = () => {
    const [products, setProducts] = useState<ProductResponse[]>([]);
    const [categories, setCategories] = useState<CategoryResponse[]>([]);

    const [showModal, setShowModal] = useState(false);
    const [showMessageModal, setShowMessageModal] = useState(false);
    const [messageTitle, setMessageTitle] = useState("");
    const [messageText, setMessageText] = useState("");
    const [messageType, setMessageType] = useState<"success" | "danger">("success");

    const [showConfirmModal, setShowConfirmModal] = useState(false);
    const [deleteProductId, setDeleteProductId] = useState("");
    const [deleteProductName, setDeleteProductName] = useState("");

    const [selectedProduct, setSelectedProduct] =
        useState<ProductRequest | undefined>(undefined);

    const { loading, error, runSafely, clearError } = useAsyncState(true);

    useEffect(() => {
        void loadData();
    }, []);

    const loadData = async () => {
        const data = await runSafely(
            async () => await Promise.all([getProducts(), getCategories()]),
            {
                defaultErrorMessage: "Failed to load products.",
                onError: errorValue => console.error(errorValue)
            }
        );

        if (!data) {
            return;
        }

        const [productData, categoryData] = data;
        setProducts(productData);
        setCategories(categoryData);
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
        catch (errorValue) {
            console.error(errorValue);

            setMessageTitle("Operation Failed");
            setMessageText(
                getApiErrorMessage(
                    errorValue,
                    `Unable to ${product.id ? "update" : "create"} the product. Please try again.`
                )
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
            setMessageText(`Product "${deleteProductName}" has been deleted successfully.`);
            setMessageType("success");
            setShowMessageModal(true);
        }
        catch (errorValue) {
            console.error(errorValue);
            setShowConfirmModal(false);

            setMessageTitle("Delete Failed");
            setMessageText(
                getApiErrorMessage(
                    errorValue,
                    `Unable to delete "${deleteProductName}". Please try again.`
                )
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

    const closeMessageModal = () => {
        setShowMessageModal(false);
        clearError();
    };

    return {
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
    };
};
