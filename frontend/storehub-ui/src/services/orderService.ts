import api from "../api/axios";
import OrderResponse from "../models/order/OrderResponse";
import OrderUpdateRequest from "../models/order/OrderUpdateRequest";
import MyOrderResponse from "../models/order/MyOrderResponse";

interface OrderCreateRequest {
    userId: string;
    fullName: string;
    shippingAddress: string;
    city: string;
    zipCode: string;
    phoneNumber: string;
    paymentMethod: string;
    totalAmount: number;
    items: {
        productId: string;
        quantity: number;
        unitPrice: number;
        totalPrice: number;
    }[];
}

export const getOrders = async (): Promise<OrderResponse[]> => {
    try {
        const response = await api.get<OrderResponse[]>("/Order");
        return response.data;
    } catch (error) {
        console.error("Failed to fetch orders:", error);
        throw error;
    }
};

export const getOrdersByUser = async (
    userId: string
): Promise<MyOrderResponse[]> => {
    try {
        const response = await api.get<MyOrderResponse[]>(
            `/Order/user/${userId}`
        );
        return response.data;
    } catch (error) {
        console.error(
            `Failed to fetch orders for user ${userId}:`,
            error
        );
        throw error;
    }
};

export const getMyOrders = async (): Promise<MyOrderResponse[]> => {
    try {
        const response = await api.get<MyOrderResponse[]>("/Order/me");
        return response.data;
    } catch (error) {
        console.error("Failed to fetch my orders:", error);
        throw error;
    }
};

export const createOrder = async (
    request: OrderCreateRequest
): Promise<MyOrderResponse> => {
    try {
        console.log("Creating order with data:", request);
        const response = await api.post<MyOrderResponse>("/Order", request);
        console.log("Order created successfully:", response.data);
        return response.data;
    } catch (error) {
        console.error("Failed to create order:", error);
        throw error;
    }
};

export const updateOrderStatus = async (
    request: OrderUpdateRequest
): Promise<boolean> => {
    try {
        await api.put(`/Order/${request.orderId}/status`, request);
        return true;
    } catch (error) {
        console.error(
            `Failed to update order ${request.orderId} status:`,
            error
        );
        throw error;
    }
};

export const getOrderTracking = async (orderId: string) => {
    try {
        const response = await api.get(
            `/Order/${orderId}/tracking`
        );
        return response.data;
    } catch (error) {
        console.error(
            `Failed to fetch tracking information for order ${orderId}:`,
            error
        );
        throw error;
    }
};