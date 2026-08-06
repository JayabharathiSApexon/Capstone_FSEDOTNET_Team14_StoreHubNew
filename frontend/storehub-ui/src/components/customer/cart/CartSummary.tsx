import { useNavigate } from "react-router-dom";
import { CartResponse } from "../../../models/cart/CartResponse";

interface CartSummaryProps {

    cart: CartResponse;

}

function CartSummary({
    cart
}: CartSummaryProps) {

    const navigate = useNavigate();

    return (

        <div className="card shadow-sm border-0">

            <div className="card-header bg-white">

                <h4 className="mb-0 fw-semibold">

                    Order Summary

                </h4>

            </div>

            <div className="card-body">

                <div className="d-flex justify-content-between mb-3">

                    <span className="text-muted">

                        Sub Total

                    </span>

                    <strong>

                        ₹{cart.subTotal.toLocaleString("en-IN")}

                    </strong>

                </div>

                <div className="d-flex justify-content-between mb-3">

                    <span className="text-muted">

                        Shipping

                    </span>

                    <strong>

                        ₹{cart.shipping.toLocaleString("en-IN")}

                    </strong>

                </div>

                <hr />

                <div className="d-flex justify-content-between align-items-center mb-4">

                    <h4 className="mb-0">

                        Grand Total

                    </h4>

                    <h3
                        className="mb-0 text-primary fw-bold"
                    >

                        ₹{cart.total.toLocaleString("en-IN")}

                    </h3>

                </div>

                <div className="d-grid">

                    <button
                        className="btn btn-primary btn-lg"
                        onClick={() => navigate("/checkout")}
                    >

                        Proceed To Checkout

                    </button>

                </div>

            </div>

        </div>

    );

}

export default CartSummary;