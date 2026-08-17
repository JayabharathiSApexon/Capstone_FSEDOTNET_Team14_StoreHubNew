export interface OrderItemRequest {
    productId: string;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
}

export default interface OrderCreateRequest {
    userId: string;
    fullName: string;
    shippingAddress: string;
    city: string;
    state?: string;
    zipCode: string;
    phoneNumber: string;
    paymentMethod: string;
    totalAmount: number;
    items: OrderItemRequest[];
}
