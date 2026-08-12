import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import CustomerLayout from "../../components/customer/CustomerLayout";
import StatusBadge from "../../components/admin/order/StatusBadge";
import Pagination from "../../components/common/Pagination";
import MyOrderResponse from "../../models/order/MyOrderResponse";
import { getMyOrders } from "../../services/orderService";

function MyOrders() {
    const [orders, setOrders] = useState<MyOrderResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 5;

    const navigate = useNavigate();

    useEffect(() => {
        loadOrders();
    }, []);

    const loadOrders = async () => {
        try {
            setLoading(true);
            setError("");

            const data = await getMyOrders();

            setOrders(data);
        } catch (err) {
            console.error(err);
            setError("Failed to load orders.");
        } finally {
            setLoading(false);
        }
    };

    const pagedOrders = orders.slice(
        (currentPage - 1) * itemsPerPage,
        currentPage * itemsPerPage
    );

    const handleViewDetails = (orderId: string) => {
        navigate(`/customer/orders/${orderId}`);
    };

    return (
        <CustomerLayout>
            {() => (
                <div className="container-fluid py-4">
                    <h4 className="fw-bold mb-4">
                        My Orders
                    </h4>

                    <div className="card shadow-sm">
                        <div className="card-body">
                            {loading ? (
                                <div className="text-center py-5">
                                    <h5>Loading orders...</h5>
                                </div>
                            ) : error ? (
                                <div className="alert alert-danger">
                                    {error}
                                </div>
                            ) : (
                                <>
                                    <div className="table-responsive">
                                        <table className="table table-hover align-middle">
                                            <thead className="table-light">
                                                <tr>
                                                    <th>Order ID</th>
                                                    <th>Date</th>
                                                    <th>Amount</th>
                                                    <th>Status</th>
                                                    <th className="text-end">
                                                        Action
                                                    </th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                                {pagedOrders.length > 0 ? (
                                                    pagedOrders.map(order => (
                                                        <tr key={order.id}>
                                                            <td>
                                                                {order.id
                                                                    .slice(0, 8)
                                                                    .toUpperCase()}
                                                            </td>

                                                            <td>
                                                                {new Date(
                                                                    order.orderDate
                                                                ).toLocaleDateString(
                                                                    "en-GB",
                                                                    {
                                                                        day: "2-digit",
                                                                        month: "short",
                                                                        year: "numeric"
                                                                    }
                                                                )}
                                                            </td>

                                                            <td>
                                                                ₹
                                                                {Number(
                                                                    order.totalAmount
                                                                ).toLocaleString()}
                                                            </td>

                                                            <td>
                                                                <StatusBadge
                                                                    status={
                                                                        order.status
                                                                    }
                                                                />
                                                            </td>

                                                            <td className="text-end">
                                                                <button
                                                                    type="button"
                                                                    className="btn btn-outline-primary btn-sm"
                                                                    onClick={() =>
                                                                        handleViewDetails(
                                                                            order.id
                                                                        )
                                                                    }
                                                                >
                                                                    View Details
                                                                </button>
                                                            </td>
                                                        </tr>
                                                    ))
                                                ) : (
                                                    <tr>
                                                        <td
                                                            colSpan={5}
                                                            className="text-center"
                                                        >
                                                            No orders found.
                                                        </td>
                                                    </tr>
                                                )}
                                            </tbody>
                                        </table>
                                    </div>

                                    <Pagination
                                        currentPage={currentPage}
                                        totalItems={orders.length}
                                        itemsPerPage={itemsPerPage}
                                        onPageChange={setCurrentPage}
                                    />
                                </>
                            )}
                        </div>
                    </div>
                </div>
            )}
        </CustomerLayout>
    );
}

export default MyOrders;