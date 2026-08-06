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

        <nav
            className="navbar navbar-light bg-white border-bottom px-4"
            style={{
                height: "72px"
            }}
        >

            <div className="ms-auto d-flex align-items-center">

                <span className="text-muted me-3">

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