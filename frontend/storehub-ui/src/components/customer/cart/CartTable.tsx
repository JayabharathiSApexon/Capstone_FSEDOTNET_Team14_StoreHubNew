import { useNavigate } from "react-router-dom";
import { FaTrash, FaShoppingCart } from "react-icons/fa";
import QuantitySelector from "./QuantitySelector";
import { CartItemResponse } from "../../../models/cart/CartItemResponse";

interface CartTableProps {

    items: CartItemResponse[];

    onQuantityChange: (
        cartItemId: string,
        quantity: number
    ) => void;

    onDelete: (
        item: CartItemResponse
    ) => void;

}

function CartTable({

    items,

    onQuantityChange,

    onDelete

}: CartTableProps) {

    const navigate = useNavigate();

    const apiBaseUrl = import.meta.env.VITE_API_URL.replace("/api", "");

    if (items.length === 0) {

        return (

            <div
                className="card border-0 shadow-sm text-center py-5"
                style={{
                    minHeight: "450px",
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center"
                }}
            >

                <div>

                    <FaShoppingCart
                        size={90}
                        className="text-primary mb-4"
                    />

                    <h2 className="fw-bold mb-3">

                        Your Cart is Empty

                    </h2>

                    <p
                        className="text-muted mb-4"
                        style={{
                            maxWidth: "500px",
                            margin: "0 auto"
                        }}
                    >

                        Looks like you haven't added anything to your shopping cart yet.

                        <br />

                        Browse our products and discover something you'll love.

                    </p>

                    <button
                        className="btn btn-primary btn-lg px-5"
                        onClick={() => navigate("/customer")}
                    >

                        Continue Shopping

                    </button>

                </div>

            </div>

        );

    }

    return (

        <div className="table-responsive">

            <table className="table table-hover align-middle">

                <thead className="table-light">

                    <tr>

                        <th style={{ width: "100px" }}>
                            Image
                        </th>

                        <th>
                            Product
                        </th>

                        <th className="text-center">
                            Price
                        </th>

                        <th className="text-center">
                            Quantity
                        </th>

                        <th className="text-center">
                            Total
                        </th>

                        <th className="text-center">
                            Action
                        </th>

                    </tr>

                </thead>

                <tbody>

                    {

                        items.map(item => {

                            const imageUrl =
                                item.imageUrl
                                    ? `${apiBaseUrl}${item.imageUrl}`
                                    : "https://via.placeholder.com/80x80?text=No+Image";

                            return (

                                <tr key={item.cartItemId}>

                                    <td>

                                        <img
                                            src={imageUrl}
                                            alt={item.productName}
                                            width="70"
                                            height="70"
                                            style={{
                                                objectFit: "contain",
                                                borderRadius: "8px",
                                                backgroundColor: "#fff"
                                            }}
                                        />

                                    </td>

                                    <td>

                                        <div className="fw-semibold">

                                            {item.productName}

                                        </div>

                                    </td>

                                    <td className="text-center">

                                        ₹{item.price.toLocaleString("en-IN")}

                                    </td>

                                    <td className="text-center">

                                        <QuantitySelector
                                            quantity={item.quantity}
                                            onQuantityChange={(quantity) =>

                                                onQuantityChange(
                                                    item.cartItemId,
                                                    quantity
                                                )

                                            }
                                        />

                                    </td>

                                    <td className="text-center fw-bold">

                                        ₹{item.total.toLocaleString("en-IN")}

                                    </td>

                                    <td className="text-center">

                                        <button
                                            type="button"
                                            className="btn btn-outline-danger btn-sm"
                                            title="Remove"
                                            onClick={() => onDelete(item)}
                                        >

                                            <FaTrash />

                                        </button>

                                    </td>

                                </tr>

                            );

                        })

                    }

                </tbody>

            </table>

        </div>

    );

}

export default CartTable;