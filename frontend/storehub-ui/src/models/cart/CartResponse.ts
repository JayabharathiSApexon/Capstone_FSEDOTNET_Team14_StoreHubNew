import { CartItemResponse } from "./CartItemResponse";

export interface CartResponse {

    cartId: string;

    subTotal: number;

    shipping: number;

    total: number;

    items: CartItemResponse[];

}