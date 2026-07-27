import { NavLink } from "react-router-dom";
import {
    FaTachometerAlt,
    FaBoxOpen,
    FaTags,
    FaSignOutAlt,
} from "react-icons/fa";

function Sidebar() {
    return (
        <div
            className="bg-dark text-white d-flex flex-column p-3"
            style={{ width: "240px", minHeight: "100vh" }}
        >
            <h4 className="mb-4 text-center">StoreHub</h4>

            <NavLink to="/" className="nav-link text-white mb-2">
                <FaTachometerAlt className="me-2" />
                Dashboard
            </NavLink>

            <NavLink to="/products" className="nav-link text-white mb-2">
                <FaBoxOpen className="me-2" />
                Products
            </NavLink>

            <NavLink to="/categories" className="nav-link text-white mb-2">
                <FaTags className="me-2" />
                Categories
            </NavLink>

            <div className="mt-auto">
                <NavLink to="/logout" className="nav-link text-white">
                    <FaSignOutAlt className="me-2" />
                    Logout
                </NavLink>
            </div>
        </div>
    );
}

export default Sidebar;