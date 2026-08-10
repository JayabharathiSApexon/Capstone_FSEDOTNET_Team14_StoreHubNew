import OrderResponse from "../models/order/OrderResponse";
import OrderUpdateRequest from "../models/order/OrderUpdateRequest";
import {
    getOrdersRequest,
    updateOrderStatusRequest
} from "./order/orderDataSource";

export const getOrders = async (): Promise<OrderResponse[]> => {
    return await getOrdersRequest();
};

export const updateOrderStatus = async (
    request: OrderUpdateRequest
): Promise<boolean> => {
    return await updateOrderStatusRequest(request);
};