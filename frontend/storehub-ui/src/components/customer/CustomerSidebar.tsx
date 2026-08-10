import { NavLink, useNavigate } from "react-router-dom";
import { FaShoppingBag, FaHome, FaShoppingCart, FaUser, FaSignOutAlt, } from "react-icons/fa";
import { logout } from "../../services/authService";

function CustomerSidebar() {

    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    const getNavLinkClass = ({ isActive }: { isActive: boolean }) =>
        `nav-link d-flex align-items-center mb-2 px-3 py-2 rounded ${isActive
            ? "bg-primary text-white"
            : "text-white"
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
                className={getNavLinkClass}
            >
                <FaHome className="me-2" />
                Home
            </NavLink>

            <NavLink
                to="/shopping-cart"
                className={getNavLinkClass}
            >
                <FaShoppingCart className="me-2" />
                Shopping Cart
            </NavLink>

            <NavLink
                to="/my-orders"
                className={getNavLinkClass}
            >
                <FaShoppingCart className="me-2" />
                My Orders
            </NavLink>

            <NavLink
                to="/profile"
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