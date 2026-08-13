import MyOrderResponse from "../../../models/order/MyOrderResponse";
import StatusBadge from "./StatusBadge";

interface OrderTableProps {
    orders: MyOrderResponse[];
    onUpdateStatus: (order: MyOrderResponse) => void;
}

function OrderTable({
    orders,
    onUpdateStatus
}: OrderTableProps) {

    return (

        <div className="table-responsive">

            <table className="table table-hover align-middle">

                <thead className="table-light">

                    <tr>

                        <th>Order No</th>

                        <th>Customer ID</th>

                        <th>Order Date</th>

                        <th>Amount</th>

                        <th>Status</th>

                        <th className="text-center">
                            Actions
                        </th>

                    </tr>

                </thead>

                <tbody>

                    {orders.length > 0 ? (

                        orders.map((order) => (

                            <tr key={order.id}>

                                <td>
                                    {order.id
                                        .slice(0, 8)
                                        .toUpperCase()}
                                </td>

                                <td>
                                    {order.userId
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
                                    ₹{order.totalAmount.toLocaleString("en-IN")}
                                </td>

                                <td>
                                    <StatusBadge
                                        status={order.status}
                                    />
                                </td>

                                <td className="text-center">

                                    <button
                                        type="button"
                                        className="btn btn-primary btn-sm"
                                        onClick={() =>
                                            onUpdateStatus(order)
                                        }
                                    >
                                        Update Status
                                    </button>

                                </td>

                            </tr>

                        ))

                    ) : (

                        <tr>

                            <td
                                colSpan={6}
                                className="text-center py-4"
                            >
                                No orders found.
                            </td>

                        </tr>

                    )}

                </tbody>

            </table>

        </div>

    );
}

export default OrderTable;