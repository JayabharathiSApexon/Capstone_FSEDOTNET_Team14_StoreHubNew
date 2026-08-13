import api from "../api/axios";
import MyOrderResponse from "../models/order/MyOrderResponse";
import OrderUpdateRequest from "../models/order/OrderUpdateRequest";

export const getOrders = async (): Promise<MyOrderResponse[]> => {
    try {
        const response = await api.get<MyOrderResponse[]>("/Order");

        return response.data;
    }
    catch (error) {
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
    }
    catch (error) {
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
    }
    catch (error) {
        console.error("Failed to fetch my orders:", error);

        throw error;
    }
};

export const updateOrderStatus = async (
    request: OrderUpdateRequest
): Promise<boolean> => {
    try {
        await api.put(
            `/Order/${request.orderId}/status`,
            request
        );

        return true;
    }
    catch (error) {
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
    }
    catch (error) {
        console.error(
            `Failed to fetch tracking information for order ${orderId}:`,
            error
        );

        throw error;
    }
};

export const cancelOrder = async (
    orderId: string
): Promise<boolean> => {

    try {

        await api.put(
            `/Order/${orderId}/cancel`
        );

        return true;
    }
    catch (error) {

        console.error(
            `Failed to cancel order ${orderId}:`,
            error
        );

        throw error;
    }
};