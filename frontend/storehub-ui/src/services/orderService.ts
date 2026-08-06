import OrderResponse from "../models/order/OrderResponse";
import OrderUpdateRequest from "../models/order/OrderUpdateRequest";
import api from "../api/axios";
import { getCurrentUser } from "./authService";
import MyOrderResponse from "../models/order/MyOrderResponse";

const mockOrders: OrderResponse[] = [
    {
        id: crypto.randomUUID(),
        orderNumber: "ORD-1001",
        customerName: "John Doe",
        amount: 79999,
        status: "Pending"
    },
    {
        id: crypto.randomUUID(),
        orderNumber: "ORD-1002",
        customerName: "Jane Smith",
        amount: 3499,
        status: "Processing"
    },
    {
        id: crypto.randomUUID(),
        orderNumber: "ORD-1003",
        customerName: "Michael Johnson",
        amount: 12999,
        status: "Shipped"
    },
    {
        id: crypto.randomUUID(),
        orderNumber: "ORD-1004",
        customerName: "David Miller",
        amount: 4999,
        status: "Delivered"
    },
    {
        id: crypto.randomUUID(),
        orderNumber: "ORD-1005",
        customerName: "Emily Wilson",
        amount: 8999,
        status: "Cancelled"
    }
];

export const getOrders = async (): Promise<OrderResponse[]> => {
    return Promise.resolve(mockOrders);
};

export const getOrdersByUser = async (userId: string): Promise<MyOrderResponse[]> => {
    try {
        console.debug(`Fetching orders for user ${userId} -> ${api.defaults.baseURL}/Order/user/${userId}`);
        const response = await api.get<MyOrderResponse[]>(`/Order/user/${userId}`);
        return response.data;
    }
    catch (err) {
        console.error("getOrdersByUser failed:", err);
        throw err;
    }
};

export const getMyOrders = async (): Promise<MyOrderResponse[]> => {
    // Use the authenticated "me" endpoint which reads user id from JWT claims
    try {
        const response = await api.get<MyOrderResponse[]>('/Order/me');
        return response.data;
    }
    catch (err) {
        console.error('getMyOrders (me) failed:', err);
        throw err;
    }
};

export const updateOrderStatus = async (
    request: OrderUpdateRequest
): Promise<boolean> => {

    const order = mockOrders.find(x => x.id === request.orderId);

    if (order) {
        order.status = request.status;
        return Promise.resolve(true);
    }

    return Promise.resolve(false);
};

export const getOrderTracking = async (orderId: string) => {
    try {
        const response = await api.get<any>(`/Order/${orderId}/tracking`);
        return response.data;
    }
    catch (err) {
        console.error('getOrderTracking failed:', err);
        throw err;
    }
};