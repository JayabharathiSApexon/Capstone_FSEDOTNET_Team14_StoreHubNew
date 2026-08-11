interface MyOrderResponse {
    id: string;
    userId: string;
    totalAmount: number;
    status: string;
    shippingAddress?: string;
    city?: string;
    state?: string;
    zipCode?: string;
    paymentMethod?: string;
    orderDate: string;
    updatedDate?: string | null;
}

export default MyOrderResponse;
