import OrderResponse from "../../models/order/OrderResponse";
import StatusBadge from "./StatusBadge";

interface OrderTableProps {
    orders: OrderResponse[];
    onUpdateStatus: (order: OrderResponse) => void;
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

                        <th>Customer</th>

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

                                <td>{order.orderNumber}</td>

                                <td>{order.customerName}</td>

                                <td>₹{order.amount.toLocaleString()}</td>

                                <td>
                                    <StatusBadge
                                        status={order.status}
                                    />
                                </td>

                                <td className="text-center">

                                    <button
                                        className="btn btn-primary btn-sm"
                                        onClick={() => onUpdateStatus(order)}
                                    >
                                        Update Status
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

    );

}

export default OrderTable;