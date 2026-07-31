import { NavLink } from "react-router-dom";
import {
    FaShoppingBag,
    FaHome,
    FaBoxOpen,
    FaTags,
    FaShoppingCart,
    FaHeart,
    FaUser,
    FaSignOutAlt,
} from "react-icons/fa";

function CustomerSidebar() {

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
                to="/products"
                className={getNavLinkClass}
            >
                <FaBoxOpen className="me-2" />
                Products
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

                <NavLink
                    to="/logout"
                    className={getNavLinkClass}
                >
                    <FaSignOutAlt className="me-2" />
                    Logout
                </NavLink>

            </div>

        </div>
    );
}

export default CustomerSidebar;