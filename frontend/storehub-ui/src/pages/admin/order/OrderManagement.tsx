import { useEffect, useMemo, useState } from "react";

import Layout from "../../../components/admin/AdminLayout";
import Pagination from "../../../components/common/Pagination";
import OrderTable from "../../../components/admin/order/OrderTable";
import UpdateStatusModal from "../../../components/admin/order/UpdateStatusModal";
import MessageModal from "../../../components/common/MessageModal";
import MyOrderResponse from "../../../models/order/MyOrderResponse";

import {
    getOrders,
    updateOrderStatus
} from "../../../services/orderService";

function OrderManagement() {

    const [orders, setOrders] =
        useState<MyOrderResponse[]>([]);

    const [loading, setLoading] =
        useState(true);

    const [error, setError] =
        useState("");

    const [showModal, setShowModal] =
        useState(false);

    const [selectedOrder, setSelectedOrder] =
        useState<MyOrderResponse | null>(null);

    const [statusFilter, setStatusFilter] =
        useState("All");

    const [currentPage, setCurrentPage] =
        useState(1);

    const [showSuccessModal, setShowSuccessModal] =
        useState(false);

    const [successMessage, setSuccessMessage] =
        useState("");

    const [showErrorModal, setShowErrorModal] =
        useState(false);

    const [errorMessage, setErrorMessage] =
        useState("");

    const itemsPerPage = 10;

    useEffect(() => {
        loadOrders();
    }, []);

    const loadOrders = async () => {
        try {
            setLoading(true);
            setError("");

            const data = await getOrders();

            setOrders(data);
            setCurrentPage(1);
        }
        catch (error) {
            console.error(error);
            setError("Failed to load orders.");
        }
        finally {
            setLoading(false);
        }
    };

    const handleUpdateStatus = (
        order: MyOrderResponse
    ) => {
        setSelectedOrder(order);
        setShowModal(true);
    };

    const handleSave = async (status: string) => {

        if (!selectedOrder) {
            return;
        }

        try {

            await updateOrderStatus({
                orderId: selectedOrder.id,
                status: status
            });

            setShowModal(false);
            setSelectedOrder(null);

            setSuccessMessage(`Order status has been updated to "${status}" successfully.`);

            setShowSuccessModal(true);

            try {
                await loadOrders();
            }
            catch (error) {
                console.error("Failed to refresh orders:", error);
            }

        }
        catch (error: any) {

            console.error("Failed to update order status:", error);

            setShowModal(false);
            setSelectedOrder(null);

            const message =
                error?.response?.data?.message ||
                error?.response?.data?.Message ||
                "Failed to update order status.";

            setErrorMessage(message);
            setShowErrorModal(true);
        }
    };

    const filteredOrders = useMemo(() => {

        if (statusFilter === "All") {
            return orders;
        }

        return orders.filter(
            order =>
                order.status === statusFilter
        );

    }, [orders, statusFilter]);

    const pagedOrders =
        filteredOrders.slice(
            (currentPage - 1) * itemsPerPage,
            currentPage * itemsPerPage
        );

    return (
        <Layout showHeader={false}>

            <div className="d-flex justify-content-between align-items-center mb-4">

                <div>

                    <h4 className="mb-0">
                        Order Management
                    </h4>

                    <small className="text-muted">
                        Manage customer orders
                    </small>

                </div>

                <div style={{ width: "180px" }}>

                    <select
                        className="form-select"
                        value={statusFilter}
                        onChange={(e) => {
                            setStatusFilter(
                                e.target.value
                            );

                            setCurrentPage(1);
                        }}
                    >

                        <option value="All">
                            All Status
                        </option>

                        <option value="OrderPlaced">
                            Order Placed
                        </option>

                        <option value="Pending">
                            Pending
                        </option>

                        <option value="Processing">
                            Processing
                        </option>

                        <option value="Shipped">
                            Shipped
                        </option>

                        <option value="Delivered">
                            Delivered
                        </option>

                        <option value="Cancelled">
                            Cancelled
                        </option>

                    </select>

                </div>

            </div>

            <div className="card shadow-sm">

                <div className="card-body">

                    {loading ? (

                        <div className="text-center py-5">
                            Loading orders...
                        </div>

                    ) : error ? (

                        <div className="alert alert-danger">
                            {error}
                        </div>

                    ) : (

                        <>

                            <OrderTable
                                orders={pagedOrders}
                                onUpdateStatus={
                                    handleUpdateStatus
                                }
                            />

                            <Pagination
                                currentPage={currentPage}
                                totalItems={
                                    filteredOrders.length
                                }
                                itemsPerPage={
                                    itemsPerPage
                                }
                                onPageChange={
                                    setCurrentPage
                                }
                            />

                        </>

                    )}

                </div>

            </div>

            <UpdateStatusModal
                show={showModal}
                order={selectedOrder}
                onClose={() => {
                    setShowModal(false);
                    setSelectedOrder(null);
                }}
                onSave={handleSave}
            />

            <MessageModal
                show={showSuccessModal}
                title="Order Status Updated"
                message={successMessage}
                variant="success"
                onClose={() => {
                    setShowSuccessModal(false);
                }}
            />

            <MessageModal
                show={showErrorModal}
                title="Unable to Update Order Status"
                message={errorMessage}
                variant="danger"
                onClose={() => {
                    setShowErrorModal(false);
                }}
            />

        </Layout>
    );
}

export default OrderManagement;
