import { NavLink } from "react-router-dom";
import {
    FaTachometerAlt,
    FaBoxOpen,
    FaShoppingCart,
    FaWarehouse,
    FaUsers,
    FaChartBar,
    FaCog,
    FaSignOutAlt,
} from "react-icons/fa";

function Sidebar() {
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
            <h4 className="mb-4 text-center fw-bold">
                StoreHub
            </h4>

            <NavLink
                to="/dashboard"
                className={getNavLinkClass}
            >
                <FaTachometerAlt className="me-2" />
                Dashboard
            </NavLink>

            <NavLink
                to="/products"
                className={getNavLinkClass}
            >
                <FaBoxOpen className="me-2" />
                Products
            </NavLink>

            <NavLink
                to="/orders"
                className={getNavLinkClass}
            >
                <FaShoppingCart className="me-2" />
                Orders
            </NavLink>

            <NavLink
                to="/inventory"
                className={getNavLinkClass}
            >
                <FaWarehouse className="me-2" />
                Inventory
            </NavLink>

            <NavLink
                to="/users"
                className={getNavLinkClass}
            >
                <FaUsers className="me-2" />
                Users
            </NavLink>

            <NavLink
                to="/reports"
                className={getNavLinkClass}
            >
                <FaChartBar className="me-2" />
                Reports
            </NavLink>

            <NavLink
                to="/settings"
                className={getNavLinkClass}
            >
                <FaCog className="me-2" />
                Settings
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

export default Sidebar;