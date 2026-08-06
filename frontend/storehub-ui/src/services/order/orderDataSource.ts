import OrderResponse from "../../models/order/OrderResponse";
import OrderUpdateRequest from "../../models/order/OrderUpdateRequest";
import { getMockOrders, updateMockOrderStatus } from "./orderMockStore";

export const getOrdersRequest = async (): Promise<OrderResponse[]> => {
    return await getMockOrders();
};

export const updateOrderStatusRequest = async (
    request: OrderUpdateRequest
): Promise<boolean> => {
    return await updateMockOrderStatus(request);
};
