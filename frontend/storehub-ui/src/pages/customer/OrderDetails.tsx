import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import CustomerLayout from "../../components/customer/CustomerLayout";
import TrackingResponse from "../../models/order/TrackingResponse";
import { getOrderTracking } from "../../services/orderService";
import "./OrderDetails.css";

function OrderDetails() {
    const { id } = useParams<{ id: string }>();

    const [tracking, setTracking] = useState<TrackingResponse | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        if (id) {
            load();
        }
    }, [id]);

    const load = async () => {
        try {
            setLoading(true);
            const data = await getOrderTracking(id!);
            setTracking(data as TrackingResponse);
        }
        catch (err) {
            console.error(err);
            setError("Failed to load tracking details.");
        }
        finally {
            setLoading(false);
        }
    };

    return (
        <CustomerLayout>
            {() => (
                <div className="container-fluid py-4">

                    <h2 className="fw-bold mb-4">Order Details</h2>

                    {loading ? (
                        <div className="text-center py-5">
                            Loading...
                        </div>
                    ) : error ? (
                        <div className="alert alert-danger">
                            {error}
                        </div>
                    ) : tracking ? (

                        <div className="row align-items-stretch">

                            {/* Items */}

                            <div className="col-md-6">
                                <div className="card shadow-sm h-100">
                                    <div className="card-body order-items">

                                        <h4 className="mb-4">
                                            Items
                                        </h4>

                                        {tracking.products.map(product => (

                                            <div
                                                key={product.productId}
                                                className="d-flex align-items-center mb-3"
                                            >

                                                <img
                                                    src={product.imageUrl || "/placeholder.png"}
                                                    alt={product.name}
                                                    className="me-3"
                                                />

                                                <div>

                                                    <div className="fw-bold">
                                                        {product.name}
                                                    </div>

                                                    <div className="text-muted">
                                                        Qty : {product.quantity}
                                                    </div>

                                                </div>

                                            </div>

                                        ))}

                                        <hr />

                                        <h5 className="fw-bold">
                                            Total Amount : ₹
                                            {Number(tracking.totalAmount).toLocaleString()}
                                        </h5>

                                    </div>
                                </div>
                            </div>

                            {/* Tracking */}

                            <div className="col-md-6">

                                <div className="card shadow-sm h-100">

                                    <div className="card-body">

                                        <h4 className="mb-4">
                                            Order Status
                                        </h4>

                                        <div className="timeline">

                                            {tracking.trackingHistory.map((status, index) => (

                                                <div
                                                    key={index}
                                                    className="timeline-item"
                                                >

                                                    <div className="timeline-marker">

                                                        <div
                                                            className={`timeline-dot ${status.completed
                                                                ? "completed"
                                                                : index === tracking.trackingHistory.length - 1
                                                                    ? "current"
                                                                    : ""
                                                                }`}
                                                        >
                                                            {status.completed ? "✓" : ""}
                                                        </div>

                                                    </div>

                                                    <div className="timeline-content">

                                                        <div className="fw-bold">
                                                            {status.status}
                                                        </div>

                                                        <small>
                                                            {new Date(status.statusDate).toLocaleString()}
                                                        </small>

                                                    </div>

                                                </div>

                                            ))}

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

                </div>
            )}
        </CustomerLayout>
    );
}

export default OrderDetails;