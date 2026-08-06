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
    removeCartItem
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

    return (

        <CustomerLayout
            showHeader={false}
        >

            {() => (

                <div className="container-fluid py-4">

                    <h2 className="fw-bold mb-4">

                        Shopping Cart

                    </h2>

                    {

                        loading

                            ?

                            <p>Loading...</p>

                            :

                            cart &&

                            <>

                                <CartTable

                                    items={cart.items}

                                    onQuantityChange={handleQuantityChange}

                                    onDelete={handleDeleteClick}

                                />

                                {

                                    cart.items.length > 0 && (

                                        <div className="row mt-4">

                                            <div className="col-md-6">

                                                <button
                                                    className="btn btn-outline-secondary"
                                                    onClick={() => navigate("/customer")}
                                                >

                                                    ← Continue Shopping

                                                </button>

                                            </div>

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

                    <ConfirmModal

                        show={showDeleteModal}

                        title="Remove Product"

                        message={`Remove "${selectedItem?.productName}" from cart?`}

                        onConfirm={confirmDelete}

                        onCancel={() => setShowDeleteModal(false)}

                    />

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