import { useEffect, useState } from "react";
import { NavLink, useParams } from "react-router-dom";

import CustomerLayout from "../../components/customer/CustomerLayout";
import ConfirmModal from "../../components/common/ConfirmModal";
import MessageModal from "../../components/common/MessageModal";

import TrackingResponse from "../../models/order/TrackingResponse";

import {
    getOrderTracking,
    cancelOrder
} from "../../services/orderService";

import "./OrderDetails.css";

const apiBaseUrl = import.meta.env.VITE_API_URL.replace(/\/api\/?$/, "");

function OrderDetails() {

    const { id } = useParams<{ id: string }>();

    const [tracking, setTracking] =
        useState<TrackingResponse | null>(null);

    const [loading, setLoading] =
        useState(true);

    const [error, setError] =
        useState("");

    const [showCancelModal, setShowCancelModal] =
        useState(false);

    const [showMessageModal, setShowMessageModal] =
        useState(false);

    const [messageTitle, setMessageTitle] =
        useState("");

    const [message, setMessage] =
        useState("");

    const [messageVariant, setMessageVariant] =
        useState<
            "success" |
            "danger" |
            "warning" |
            "info"
        >("success");

    const [cancelling, setCancelling] =
        useState(false);

    useEffect(() => {
        if (id) {
            load();
        }
    }, [id]);

    const load = async () => {

        if (!id) {
            return;
        }

        try {
            setLoading(true);
            setError("");

            const data =
                await getOrderTracking(id);

            setTracking(
                data as TrackingResponse
            );
        }
        catch (err) {
            console.error(err);

            setError(
                "Failed to load tracking details."
            );
        }
        finally {
            setLoading(false);
        }
    };

    const canCancelOrder = () => {

        if (!tracking) {
            return false;
        }

        const status =
            tracking.status?.trim().toLowerCase();

        return (
            status === "orderplaced" ||
            status === "pending" ||
            status === "processing"
        );
    };

    const handleCancelOrder = async () => {

        if (!id) {
            return;
        }

        try {

            setCancelling(true);

            await cancelOrder(id);

            setShowCancelModal(false);

            setMessageTitle(
                "Order Cancelled"
            );

            setMessage(
                "Your order has been cancelled successfully."
            );

            setMessageVariant("success");

            setShowMessageModal(true);

            // Refresh tracking information
            await load();

        }
        catch (error: any) {

            console.error(
                "Failed to cancel order:",
                error
            );

            setShowCancelModal(false);

            const errorMessage =
                error?.response?.data?.message ||
                error?.response?.data?.Message ||
                "Failed to cancel the order.";

            setMessageTitle(
                "Unable to Cancel Order"
            );

            setMessage(
                errorMessage
            );

            setMessageVariant("danger");

            setShowMessageModal(true);
        }
        finally {
            setCancelling(false);
        }
    };

    return (
        <CustomerLayout showHeader={false}>
            {() => (
                <div className="container-fluid py-4">

                    <div className="d-flex justify-content-between align-items-center mb-4">

                        <h2 className="fw-bold mb-0">
                            Order Details
                        </h2>

                        {tracking &&
                            canCancelOrder() && (

                                <button
                                    type="button"
                                    className="btn btn-outline-danger"
                                    onClick={() =>
                                        setShowCancelModal(true)
                                    }
                                    disabled={cancelling}
                                >
                                    Cancel Order
                                </button>

                            )}

                    </div>

                    {loading ? (

                        <div className="text-center py-5">
                            Loading...
                        </div>

                    ) : error ? (

                        <div className="alert alert-danger">
                            {error}
                        </div>

                    ) : tracking ? (

                        <div className="row g-4">

                            {/* Items */}

                            <div className="col-md-6">

                                <div className="card shadow-sm order-items-card">

                                    <div className="card-body">

                                        <div className="d-flex justify-content-between align-items-center mb-4">

                                            <h4 className="mb-0">
                                                Items
                                            </h4>

                                            <span className="badge bg-light text-dark">
                                                {tracking.products.length}{" "}
                                                {tracking.products.length === 1
                                                    ? "Item"
                                                    : "Items"}
                                            </span>

                                        </div>

                                        <div className="order-product-list">

                                            {tracking.products.map(product => (

                                                <div
                                                    key={product.productId}
                                                    className="order-product-item"
                                                >

                                                    <NavLink
                                                        to={`/customer/products/${product.productId}`}
                                                        className="order-product-image"
                                                        title={`View ${product.productName}`}
                                                    >
                                                        <img
                                                            src={
                                                                product.imageUrl
                                                                    ? `${apiBaseUrl}${product.imageUrl}`
                                                                    : "/placeholder.png"
                                                            }
                                                            alt={product.productName}
                                                            onError={(event) => {
                                                                event.currentTarget.src =
                                                                    "/placeholder.png";
                                                            }}
                                                        />
                                                    </NavLink>

                                                    <div className="order-product-info">

                                                        <NavLink
                                                            to={`/customer/products/${product.productId}`}
                                                            className="order-product-name text-decoration-none"
                                                        >
                                                            {product.productName}
                                                        </NavLink>

                                                        <div className="text-muted small mt-1">
                                                            Quantity: {product.quantity}
                                                        </div>

                                                    </div>

                                                </div>

                                            ))}

                                        </div>

                                        <div className="order-total">

                                            <span>
                                                Total Amount
                                            </span>

                                            <strong>
                                                ₹
                                                {Number(
                                                    tracking.totalAmount
                                                ).toLocaleString("en-IN")}
                                            </strong>

                                        </div>

                                    </div>

                                </div>

                            </div>


                            {/* Tracking */}

                            <div className="col-md-6">

                                <div className="card shadow-sm h-100 order-status-card">

                                    <div className="card-body">

                                        <h4 className="mb-4">
                                            Order Status
                                        </h4>

                                        <div className="timeline">

                                            {tracking.trackingHistory.map(
                                                (status, index) => {

                                                    const isCancelled =
                                                        status.status.toLowerCase() ===
                                                        "cancelled";

                                                    const isCurrent =
                                                        status.status.toLowerCase() ===
                                                        tracking.status.toLowerCase();

                                                    return (

                                                        <div
                                                            key={index}
                                                            className="timeline-item"
                                                        >

                                                            <div className="timeline-marker">

                                                                <div
                                                                    className={`
                                                                        timeline-dot
                                                                        ${isCancelled
                                                                            ? "cancelled"
                                                                            : isCurrent
                                                                                ? "current"
                                                                                : "completed"
                                                                        }
                                                                    `}
                                                                >

                                                                    {isCancelled
                                                                        ? "×"
                                                                        : "✓"}

                                                                </div>

                                                            </div>

                                                            <div className="timeline-content">

                                                                <div
                                                                    className={`
                                                                        fw-bold
                                                                        ${isCancelled
                                                                            ? "cancelled-status"
                                                                            : isCurrent
                                                                                ? "current-status"
                                                                                : ""
                                                                        }
                                                                    `}
                                                                >
                                                                    {
                                                                        status.status
                                                                    }
                                                                </div>

                                                                <small>
                                                                    {
                                                                        new Date(
                                                                            status.statusDate
                                                                        ).toLocaleString()
                                                                    }
                                                                </small>

                                                            </div>

                                                        </div>

                                                    );
                                                }
                                            )}

                                        </div>

                                    </div>

                                </div>

                            </div>

                        </div>

                    ) : (

                        <div className="alert alert-warning">
                            No tracking data found.
                        </div>

                    )}


                    {/* Cancel Confirmation */}

                    <ConfirmModal
                        show={showCancelModal}
                        title="Cancel Order"
                        message="Are you sure you want to cancel this order?"
                        confirmText={
                            cancelling
                                ? "Cancelling..."
                                : "Cancel Order"
                        }
                        onCancel={() => {
                            if (!cancelling) {
                                setShowCancelModal(false);
                            }
                        }}
                        onConfirm={
                            handleCancelOrder
                        }
                    />


                    {/* Success / Error */}

                    <MessageModal
                        show={showMessageModal}
                        title={messageTitle}
                        message={message}
                        variant={messageVariant}
                        onClose={() =>
                            setShowMessageModal(false)
                        }
                    />

                </div>
            )}
        </CustomerLayout>
    );
}

export default OrderDetails;