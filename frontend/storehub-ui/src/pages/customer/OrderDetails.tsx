import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import CustomerLayout from "../../components/customer/CustomerLayout";
import TrackingResponse from "../../models/order/TrackingResponse";
import { getOrderTracking } from "../../services/orderService";

function OrderDetails() {
    const { id } = useParams<{ id: string }>();

    const [tracking, setTracking] = useState<TrackingResponse | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!id) return;

        load();
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
        finally { setLoading(false); }
    };

    return (
        <CustomerLayout>
            {() => (
                <div className="container-fluid py-4">
                    <h2 className="fw-bold mb-4">Order Details</h2>

                    {loading ? (
                        <div className="text-center py-5">Loading...</div>
                    ) : error ? (
                        <div className="alert alert-danger">{error}</div>
                    ) : tracking ? (
                        <div className="row">
                            <div className="col-md-6">
                                <div className="card shadow-sm mb-3">
                                    <div className="card-body">
                                        <h5>Items</h5>
                                        {tracking.products.map(p => (
                                            <div key={p.productId} className="d-flex align-items-center mb-2">
                                                <img src={p.imageUrl ?? '/placeholder.png'} alt="" style={{ width: 64, height: 64, objectFit: 'cover' }} className="me-3" />
                                                <div>
                                                    <div className="fw-bold">{p.name}</div>
                                                    <div className="text-muted">Qty: {p.quantity}</div>
                                                </div>
                                            </div>
                                        ))}
                                        <hr />
                                        <div className="fw-bold">Total Amount: ₹{Number(tracking.totalAmount).toLocaleString()}</div>
                                    </div>
                                </div>
                            </div>

                            <div className="col-md-6">
                                <div className="card shadow-sm mb-3">
                                    <div className="card-body">
                                        <h5>Order Status</h5>
                                        <div className="timeline">
                                            {tracking.trackingHistory.map((t, idx) => (
                                                <div key={idx} className="d-flex mb-3">
                                                    <div style={{ width: 48 }}>
                                                        <div className={`rounded-circle ${t.completed ? 'bg-primary' : 'bg-light'}`} style={{ width: 14, height: 14 }} />
                                                    </div>
                                                    <div>
                                                        <div className="fw-bold">{t.status}</div>
                                                        <small className="text-muted">{new Date(t.statusDate).toLocaleString()}</small>
                                                    </div>
                                                </div>
                                            ))}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    ) : (
                        <div>No tracking data found.</div>
                    )}
                </div>
            )}
        </CustomerLayout>
    );
}

export default OrderDetails;
