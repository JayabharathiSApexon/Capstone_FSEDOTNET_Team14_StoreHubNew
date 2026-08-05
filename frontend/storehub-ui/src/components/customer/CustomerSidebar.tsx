import { NavLink } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import {
    FaShoppingBag,
    FaHome,
    FaTags,
    FaShoppingCart,
    FaHeart,
    FaUser,
    FaSignOutAlt,
} from "react-icons/fa";
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
            style={{ width: "240px", minHeight: "100vh" }}
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
                to="/"
                className={getNavLinkClass}
            >
                <FaHome className="me-2" />
                Home
            </NavLink>

            <NavLink
                to="/categories"
                className={getNavLinkClass}
            >
                <FaTags className="me-2" />
                Categories
            </NavLink>

            <NavLink
                to="/my-orders"
                className={getNavLinkClass}
            >
                <FaShoppingCart className="me-2" />
                My Orders
            </NavLink>

            <NavLink
                to="/wishlist"
                className={getNavLinkClass}
            >
                <FaHeart className="me-2" />
                Wishlist
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
                    className="nav-link d-flex align-items-center mb-2 px-3 py-2 rounded text-white bg-transparent border-0 w-100"
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