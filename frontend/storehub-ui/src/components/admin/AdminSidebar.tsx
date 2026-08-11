import { NavLink } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import {
    FaShoppingBag,
    FaTachometerAlt,
    FaBoxOpen,
    FaShoppingCart,
    FaWarehouse,
    FaUsers,
    FaSignOutAlt,
} from "react-icons/fa";
import { logout } from "../../services/authService";

function AdminSidebar() {
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
                to="/admin/dashboard"
                className={getNavLinkClass}
            >
                <FaTachometerAlt className="me-2" />
                Dashboard
            </NavLink>

            <NavLink
                to="/admin/products"
                className={getNavLinkClass}
            >
                <FaBoxOpen className="me-2" />
                Products
            </NavLink>

            <NavLink
                to="/admin/orders"
                className={getNavLinkClass}
            >
                <FaShoppingCart className="me-2" />
                Orders
            </NavLink>

            <NavLink
                to="/admin/inventory"
                className={getNavLinkClass}
            >
                <FaWarehouse className="me-2" />
                Inventory
            </NavLink>

            <NavLink
                to="/admin/users"
                className={getNavLinkClass}
            >
                <FaUsers className="me-2" />
                Users
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

export default AdminSidebar;