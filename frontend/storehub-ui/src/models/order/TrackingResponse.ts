import TrackingProduct from "./TrackingProduct";
import TrackingStatus from "./TrackingStatus";

interface TrackingResponse {
    orderId: string;
    orderDate: string;
    expectedDeliveryDate: string;
    totalAmount: number;
    products: TrackingProduct[];
    trackingHistory: TrackingStatus[];
}

export default TrackingResponse;
