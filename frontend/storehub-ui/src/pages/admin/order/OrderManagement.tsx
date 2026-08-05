import { useEffect, useMemo, useState } from "react";

import Layout from "../../../components/admin/AdminLayout";
import Pagination from "../../../components/common/Pagination";

import OrderTable from "../../../components/admin/order/OrderTable";
import UpdateStatusModal from "../../../components/admin/order/UpdateStatusModal";

import OrderResponse from "../../../models/order/OrderResponse";

import {
    getOrders,
    updateOrderStatus
} from "../../../services/orderService";

function OrderManagement() {

    const [orders, setOrders] = useState<OrderResponse[]>([]);

    const [loading, setLoading] = useState(true);

    const [showModal, setShowModal] = useState(false);

    const [selectedOrder, setSelectedOrder] = useState<OrderResponse | null>(null);

    const [statusFilter, setStatusFilter] = useState("All");

    const [currentPage, setCurrentPage] = useState(1);

    const itemsPerPage = 5;

    useEffect(() => {

        loadOrders();

    }, []);

    const loadOrders = async () => {

        try {

            const data = await getOrders();

            setOrders(data);

            setCurrentPage(1);

        }
        catch (error) {

            console.error(error);

        }
        finally {

            setLoading(false);

        }

    };

    const handleUpdateStatus = ( order: OrderResponse ) => {

        setSelectedOrder(order);

        setShowModal(true);

    };

    const handleSave = async ( status: string ) => {

        if (!selectedOrder) {
            return;
        }

        try {

            await updateOrderStatus({

                orderId: selectedOrder.id,

                status

            });

            setShowModal(false);

            await loadOrders();

        }
        catch (error) {

            console.error(error);

            setShowModal(false);

        }

    };

    const filteredOrders = useMemo(() => {

        if (statusFilter === "All") {

            return orders;

        }

        return orders.filter(

            order => order.status === statusFilter

        );

    }, [orders, statusFilter]);

    const pagedOrders = filteredOrders.slice(

        (currentPage - 1) * itemsPerPage,

        currentPage * itemsPerPage

    );

    return (

        <Layout>

            <div className="d-flex justify-content-between align-items-center mb-4">

                <div>

                    <h3 className="mb-0">

                        Order Management

                    </h3>

                    <small className="text-muted">

                        Manage customer orders

                    </small>

                </div>

                <div style={{ width: "180px" }}>

                    <select

                        className="form-select"

                        value={statusFilter}

                        onChange={(e) => {

                            setStatusFilter(e.target.value);

                            setCurrentPage(1);

                        }}

                    >

                        <option value="All">

                            All Status

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

                    {

                        loading

                            ? (

                                <p>

                                    Loading...

                                </p>

                            )

                            : (

                                <>

                                    <OrderTable

                                        orders={pagedOrders}

                                        onUpdateStatus={handleUpdateStatus}

                                    />

                                    <Pagination

                                        currentPage={currentPage}

                                        totalItems={filteredOrders.length}

                                        itemsPerPage={itemsPerPage}

                                        onPageChange={setCurrentPage}

                                    />

                                </>

                            )

                    }

                </div>

            </div>

            <UpdateStatusModal

                show={showModal}

                order={selectedOrder}

                onClose={() => setShowModal(false)}

                onSave={handleSave}

            />

        </Layout>

    );

}

export default OrderManagement;