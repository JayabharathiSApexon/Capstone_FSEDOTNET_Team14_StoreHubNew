import api from "../api/axios";
import OrderResponse from "../models/order/OrderResponse";
import OrderUpdateRequest from "../models/order/OrderUpdateRequest";
import MyOrderResponse from "../models/order/MyOrderResponse";

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