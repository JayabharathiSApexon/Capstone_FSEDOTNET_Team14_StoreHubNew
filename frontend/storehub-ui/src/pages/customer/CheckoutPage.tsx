import { FormEvent, useMemo, useState } from "react";
import {
    FaHeadphones,
    FaKeyboard,
    FaMouse
} from "react-icons/fa";
import CustomerLayout from "../../components/customer/CustomerLayout";
import "./CheckoutPage.css";

type PaymentMethod = "card" | "upi" | "cod";

interface CheckoutItem {
    id: string;
    name: string;
    quantity: number;
    price: number;
    icon: JSX.Element;
}

const checkoutItems: CheckoutItem[] = [
    {
        id: "keyboard",
        name: "Wireless Keyboard",
        quantity: 2,
        price: 3998,
        icon: <FaKeyboard />
    },
    {
        id: "mouse",
        name: "Wireless Mouse",
        quantity: 1,
        price: 799,
        icon: <FaMouse />
    },
    {
        id: "headset",
        name: "Gaming Headset",
        quantity: 1,
        price: 2499,
        icon: <FaHeadphones />
    }
];

const currency = new Intl.NumberFormat("en-IN", {
    style: "currency",
    currency: "INR",
    maximumFractionDigits: 0
});

function CheckoutPage() {
    const [fullName, setFullName] = useState("");
    const [address, setAddress] = useState("");
    const [city, setCity] = useState("");
    const [pincode, setPincode] = useState("");
    const [phoneNumber, setPhoneNumber] = useState("");
    const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>("card");
    const [message, setMessage] = useState("");

    const subtotal = useMemo(() => {
        return checkoutItems.reduce((total, item) => total + item.price, 0);
    }, []);

    const shipping = 0;
    const total = subtotal + shipping;

    const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setMessage(`Order placed successfully using ${paymentMethod.toUpperCase()}.`);
    };

    return (
        <CustomerLayout>
            {() => (
                <div className="checkout-page container-fluid">
                    <h2 className="checkout-title">Checkout</h2>

                    <div className="checkout-shell row g-4">
                        <div className="col-12 col-xl-7">
                            <section className="checkout-card">
                                <h5 className="checkout-section-title">Shipping Information</h5>

                                <form onSubmit={handleSubmit}>
                                    <div className="mb-3">
                                        <label className="form-label">Full Name</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            placeholder="Enter your full name"
                                            value={fullName}
                                            onChange={(e) => setFullName(e.target.value)}
                                            required
                                        />
                                    </div>

                                    <div className="mb-3">
                                        <label className="form-label">Address</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            placeholder="Enter your address"
                                            value={address}
                                            onChange={(e) => setAddress(e.target.value)}
                                            required
                                        />
                                    </div>

                                    <div className="row g-3">
                                        <div className="col-12 col-md-6">
                                            <label className="form-label">City</label>
                                            <input
                                                type="text"
                                                className="form-control"
                                                placeholder="Enter city"
                                                value={city}
                                                onChange={(e) => setCity(e.target.value)}
                                                required
                                            />
                                        </div>

                                        <div className="col-12 col-md-6">
                                            <label className="form-label">Pincode</label>
                                            <input
                                                type="text"
                                                className="form-control"
                                                placeholder="Enter pincode"
                                                value={pincode}
                                                onChange={(e) => setPincode(e.target.value)}
                                                required
                                            />
                                        </div>
                                    </div>

                                    <div className="mt-3 mb-4">
                                        <label className="form-label">Phone Number</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            placeholder="Enter phone number"
                                            value={phoneNumber}
                                            onChange={(e) => setPhoneNumber(e.target.value)}
                                            required
                                        />
                                    </div>

                                    <h6 className="checkout-section-title mb-3">Payment Method</h6>

                                    <div className="checkout-payment-options">
                                        <label className="form-check-label">
                                            <input
                                                type="radio"
                                                className="form-check-input"
                                                name="paymentMethod"
                                                checked={paymentMethod === "card"}
                                                onChange={() => setPaymentMethod("card")}
                                            />
                                            Credit / Debit Card
                                        </label>

                                        <label className="form-check-label">
                                            <input
                                                type="radio"
                                                className="form-check-input"
                                                name="paymentMethod"
                                                checked={paymentMethod === "upi"}
                                                onChange={() => setPaymentMethod("upi")}
                                            />
                                            UPI
                                        </label>

                                        <label className="form-check-label">
                                            <input
                                                type="radio"
                                                className="form-check-input"
                                                name="paymentMethod"
                                                checked={paymentMethod === "cod"}
                                                onChange={() => setPaymentMethod("cod")}
                                            />
                                            Cash on Delivery
                                        </label>
                                    </div>

                                    {message && (
                                        <div className="alert alert-success mt-4 mb-0 py-2">
                                            {message}
                                        </div>
                                    )}

                                    <button type="submit" className="btn checkout-order-btn mt-4">
                                        Place Order
                                    </button>
                                </form>
                            </section>
                        </div>

                        <div className="col-12 col-xl-5">
                            <section className="checkout-card">
                                <h5 className="checkout-section-title">Order Summary</h5>

                                <div className="checkout-summary-list">
                                    {checkoutItems.map((item) => (
                                        <div key={item.id} className="checkout-summary-row">
                                            <div className="checkout-item-main">
                                                <span className="checkout-item-icon">{item.icon}</span>
                                                <span className="checkout-item-name">{item.name}</span>
                                            </div>

                                            <div className="checkout-item-meta">
                                                <span className="checkout-item-qty">x{item.quantity}</span>
                                                <span className="checkout-item-price">{currency.format(item.price)}</span>
                                            </div>
                                        </div>
                                    ))}
                                </div>

                                <hr className="my-3" />

                                <div className="checkout-total-row">
                                    <span>Subtotal</span>
                                    <strong>{currency.format(subtotal)}</strong>
                                </div>

                                <div className="checkout-total-row">
                                    <span>Shipping</span>
                                    <strong>{currency.format(shipping)}</strong>
                                </div>

                                <div className="checkout-total-row checkout-grand-total">
                                    <span>Total</span>
                                    <strong>{currency.format(total)}</strong>
                                </div>
                            </section>
                        </div>
                    </div>
                </div>
            )}
        </CustomerLayout>
    );
}

export default CheckoutPage;
