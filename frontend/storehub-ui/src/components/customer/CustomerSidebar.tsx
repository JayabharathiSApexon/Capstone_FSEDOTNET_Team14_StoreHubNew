import { NavLink, useLocation, useNavigate } from "react-router-dom";
import {
    FaShoppingBag,
    FaHome,
    FaShoppingCart,
    FaUser,
    FaSignOutAlt,
    FaClipboardList
} from "react-icons/fa";
import { logout } from "../../services/authService";

function CustomerSidebar() {
    const navigate = useNavigate();
    const location = useLocation();

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    const orderDetailsMatch = location.pathname.match(
        /^\/customer\/orders\/([^/]+)$/
    );

    const orderId = orderDetailsMatch?.[1];

    const productDetailsMatch = location.pathname.match(
        /^\/customer\/products\/([^/]+)$/
    );

    const productId = productDetailsMatch?.[1];

    const getNavLinkClass = ({ isActive }: { isActive: boolean }) =>
        `nav-link d-flex align-items-center mb-2 px-3 py-2 rounded ${isActive ? "bg-primary text-white" : "text-white"
        }`;

    return (
        <div
            className="bg-dark text-white d-flex flex-column p-3"
            style={{
                position: "fixed",
                top: 0,
                left: 0,
                width: "240px",
                height: "100vh",
                overflowY: "auto",
                zIndex: 1000
            }}
        >
            <div className="d-flex align-items-center justify-content-center mb-4">
                <FaShoppingBag
                    className="me-2 text-primary"
                    size={24}
                />

                <h4 className="fw-bold mb-0">
                    StoreHub
                </h4>
            </div>

            <NavLink
                to="/customer"
                end
                className={getNavLinkClass}
            >
                <FaHome className="me-2" />
                Home
            </NavLink>

            {productId && (
                <NavLink
                    to={`/customer/products/${productId}`}
                    end
                    className={getNavLinkClass}
                >
                    <FaClipboardList className="me-2" />
                    Product Details
                </NavLink>
            )}

            <NavLink
                to="/shopping-cart"
                end
                className={getNavLinkClass}
            >
                <FaShoppingCart className="me-2" />
                Shopping Cart
            </NavLink>

            <NavLink
                to="/my-orders"
                end
                className={getNavLinkClass}
            >
                <FaShoppingCart className="me-2" />
                My Orders
            </NavLink>

            {orderId && (
                <NavLink
                    to={`/customer/orders/${orderId}`}
                    end
                    className={getNavLinkClass}
                >
                    <FaClipboardList className="me-2" />
                    Order Details
                </NavLink>
            )}

            <NavLink
                to="/profile"
                end
                className={getNavLinkClass}
            >
                <FaUser className="me-2" />
                Profile
            </NavLink>

            <div className="mt-auto">
                <button
                    type="button"
                    className="nav-link d-flex align-items-center px-3 py-2 rounded text-white bg-transparent border-0 w-100"
                    onClick={handleLogout}
                >
                    <FaSignOutAlt className="me-2" />
                    Logout
                </button>
            </div>
        </div>
    );
}

export default CustomerSidebar;