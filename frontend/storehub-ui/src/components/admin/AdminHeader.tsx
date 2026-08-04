import { useNavigate } from "react-router-dom";
import { getCurrentUser, logout } from "../../services/authService";

function AdminHeader() {
    const navigate = useNavigate();
    const currentUser = getCurrentUser();

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    return (
        <nav className="navbar navbar-light bg-white border-bottom px-4 justify-content-between">
            <span className="fw-semibold">Admin Panel</span>

            <div className="d-flex align-items-center gap-3">
                <span className="text-muted small">
                    {currentUser?.fullName ?? "Admin"}
                </span>

                <button
                    type="button"
                    className="btn btn-outline-danger btn-sm"
                    onClick={handleLogout}
                >
                    Logout
                </button>
            </div>
        </nav>
    );
}

export default AdminHeader;