import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import CustomerLayout from "../../components/customer/CustomerLayout";
import CartTable from "../../components/customer/cart/CartTable";
import CartSummary from "../../components/customer/cart/CartSummary";
import MessageModal from "../../components/common/MessageModal";
import ConfirmModal from "../../components/common/ConfirmModal";
import { CartResponse } from "../../models/cart/CartResponse";
import { CartItemResponse } from "../../models/cart/CartItemResponse";
import { useCart } from "../../context/CartContext";

import {
    getCart,
    updateCartItem,
    removeCartItem,
    clearCart
} from "../../services/cartService";

function ShoppingCart() {

    const navigate = useNavigate();

    const [cart, setCart] =
        useState<CartResponse | null>(null);

    const [loading, setLoading] =
        useState(true);

    const [selectedItem, setSelectedItem] =
        useState<CartItemResponse | null>(null);

    const [showDeleteModal, setShowDeleteModal] =
        useState(false);

    const [showClearCartModal, setShowClearCartModal] =
        useState(false);

    const [showMessageModal, setShowMessageModal] =
        useState(false);

    const [messageTitle, setMessageTitle] =
        useState("");

    const [messageText, setMessageText] =
        useState("");

    const [messageType, setMessageType] =
        useState<"success" | "danger">("success");

    const {
        loadCart: refreshHeaderCart
    } = useCart();

    useEffect(() => {

        loadCart();

    }, []);

    const loadCart = async () => {

        try {

            const data = await getCart();

            setCart(data);

        }
        catch (error) {

            console.error(error);

        }
        finally {

            setLoading(false);

        }

    };

    const handleQuantityChange = async (
        cartItemId: string,
        quantity: number
    ) => {

        try {

            await updateCartItem({

                cartItemId,

                quantity

            });

            await loadCart();

            await refreshHeaderCart();

        }
        catch (error) {

            console.error(error);

            setMessageTitle("Update Failed");

            setMessageText(
                "Unable to update cart quantity."
            );

            setMessageType("danger");

            setShowMessageModal(true);

        }

    };

    const handleDeleteClick = (
        item: CartItemResponse
    ) => {

        setSelectedItem(item);

        setShowDeleteModal(true);

    };

    const confirmDelete = async () => {

        if (!selectedItem)
            return;

        try {

            await removeCartItem(
                selectedItem.cartItemId
            );

            setShowDeleteModal(false);

            await loadCart();

            await refreshHeaderCart();

            setMessageTitle("Success");

            setMessageText(
                `"${selectedItem.productName}" removed from cart.`
            );

            setMessageType("success");

            setShowMessageModal(true);

        }
        catch {

            setShowDeleteModal(false);

            setMessageTitle("Delete Failed");

            setMessageText(
                "Unable to remove item."
            );

            setMessageType("danger");

            setShowMessageModal(true);

        }

    };

    const handleClearCartClick = () => {

        setShowClearCartModal(true);

    };

    const confirmClearCart = async () => {

        try {

            await clearCart();

            setShowClearCartModal(false);

            await loadCart();

            await refreshHeaderCart();

            setMessageTitle("Success");

            setMessageText(
                "All items have been removed from your cart."
            );

            setMessageType("success");

            setShowMessageModal(true);

        }
        catch {

            setShowClearCartModal(false);

            setMessageTitle("Clear Cart Failed");

            setMessageText(
                "Unable to clear the cart."
            );

            setMessageType("danger");

            setShowMessageModal(true);

        }

    };

    return (

        <CustomerLayout
            showHeader={true}
            isShoppingCart={true}
        >

            {() => (

                <div className="container-fluid py-4">

                    {

                        loading

                            ?

                            <p>Loading...</p>

                            :

                            cart &&

                            <>

                                {/* Cart Header */}

                                <div className="d-flex justify-content-between align-items-center mb-3">

                                    <h5 className="fw-bold mb-0">

                                        Your Cart

                                    </h5>

                                    {

                                        cart.items.length > 0 && (

                                            <button
                                                type="button"
                                                className="btn btn-outline-danger"
                                                onClick={handleClearCartClick}
                                            >

                                                Clear Cart

                                            </button>

                                        )

                                    }

                                </div>

                                {/* Cart Items */}

                                <CartTable

                                    items={cart.items}

                                    onQuantityChange={handleQuantityChange}

                                    onDelete={handleDeleteClick}

                                />

                                {

                                    cart.items.length > 0 && (

                                        <div className="row mt-4">

                                            {/* Continue Shopping */}

                                            <div className="col-md-6">

                                                <button
                                                    type="button"
                                                    className="btn btn-outline-secondary"
                                                    onClick={() => navigate("/customer")}
                                                >

                                                    ← Continue Shopping

                                                </button>

                                            </div>

                                            {/* Order Summary */}

                                            <div className="col-md-6 d-flex justify-content-end">

                                                <div style={{ width: "420px" }}>

                                                    <CartSummary
                                                        cart={cart}
                                                    />

                                                </div>

                                            </div>

                                        </div>

                                    )

                                }

                            </>

                    }

                    {/* Remove Single Product Confirmation */}

                    <ConfirmModal

                        show={showDeleteModal}

                        title="Remove Product"

                        message={`Remove "${selectedItem?.productName}" from cart?`}

                        onConfirm={confirmDelete}

                        onCancel={() => setShowDeleteModal(false)}

                    />

                    {/* Clear Entire Cart Confirmation */}

                    <ConfirmModal

                        show={showClearCartModal}

                        title="Clear Cart"

                        message="Are you sure you want to remove all items from your cart?"

                        onConfirm={confirmClearCart}

                        onCancel={() => setShowClearCartModal(false)}

                    />

                    {/* Success / Error Message */}

                    <MessageModal

                        show={showMessageModal}

                        title={messageTitle}

                        message={messageText}

                        variant={messageType}

                        onClose={() => setShowMessageModal(false)}

                    />

                </div>

            )}

        </CustomerLayout>

    );

}

export default ShoppingCart;