import OrderResponse from "../../models/order/OrderResponse";
import OrderUpdateRequest from "../../models/order/OrderUpdateRequest";

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

export const getMockOrders = async (): Promise<OrderResponse[]> => {
    return Promise.resolve(mockOrders);
};

export const updateMockOrderStatus = async (request: OrderUpdateRequest): Promise<boolean> => {
    const order = mockOrders.find(x => x.id === request.orderId);

    if (order) {
        order.status = request.status;
        return Promise.resolve(true);
    }

    return Promise.resolve(false);
};
