import { createContext, useContext, useEffect, useState, ReactNode } from "react";
import { getCart } from "../services/cartService";
import { CartResponse } from "../models/cart/CartResponse";

interface CartContextType {
    cart: CartResponse | null;
    cartCount: number;
    loadCart: () => Promise<void>;
    setCart: React.Dispatch<
        React.SetStateAction<CartResponse | null>
    >;

}

const CartContext =
    createContext<CartContextType | undefined>(
        undefined
    );

interface CartProviderProps {

    children: ReactNode;

}

export function CartProvider({
    children
}: CartProviderProps) {

    const [cart, setCart] = useState<CartResponse | null>(null);

    const loadCart = async () => {

        try {

            const response = await getCart();

            setCart({
                ...response,
                items: [...response.items]
            });

        }
        catch (error) {

            console.error("Unable to load cart.", error);

            setCart(null);

        }

    };

    useEffect(() => {

        loadCart();

    }, []);

    const cartCount =
        cart?.items.reduce(
            (total, item) =>
                total + item.quantity,
            0
        ) ?? 0;

    return (

        <CartContext.Provider
            value={{
                cart,
                cartCount,
                loadCart,
                setCart
            }}
        >

            {children}

        </CartContext.Provider>

    );

}

export function useCart() {

    const context = useContext(CartContext);

    if (!context) {

        throw new Error("useCart must be used inside CartProvider.");

    }

    return context;

}