import api from "../api/axios";
import { AddToCartRequest } from "../models/cart/AddToCartRequest";
import { UpdateCartRequest } from "../models/cart/UpdateCartRequest";
import { CartResponse } from "../models/cart/CartResponse";

const baseUrl = "/cart";

export async function getCart(): Promise<CartResponse> {

    const response =
        await api.get<CartResponse>(baseUrl);

    return response.data;

}

export async function addToCart(
    request: AddToCartRequest
): Promise<CartResponse> {

    const response =
        await api.post<CartResponse>(
            baseUrl,
            request
        );

    return response.data;

}

export async function updateCartItem(
    request: UpdateCartRequest
): Promise<CartResponse> {

    const response =
        await api.put<CartResponse>(
            `${baseUrl}/${request.cartItemId}`,
            request
        );

    return response.data;

}

export async function removeCartItem(
    cartItemId: string
): Promise<void> {

    await api.delete(
        `${baseUrl}/${cartItemId}`
    );

}

export async function clearCart(): Promise<void> {

    await api.delete(baseUrl);

}