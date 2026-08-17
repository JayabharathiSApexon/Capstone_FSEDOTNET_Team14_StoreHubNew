import { FaUserCircle } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { getCurrentUser, logout } from "../../services/authService";

function ProfileHeader() {
    const navigate = useNavigate();
    const currentUser = getCurrentUser();

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    return (
        <div
            style={{
                position: "fixed",
                top: 0,
                left: "240px",
                right: 0,
                height: "72px",
                zIndex: 999,
                backgroundColor: "#fff",
                borderBottom: "1px solid #e9ecef",
                display: "flex",
                alignItems: "center",
                paddingLeft: "24px",
                paddingRight: "24px"
            }}
        >
            <div className="ms-auto d-flex align-items-center gap-3">
                <FaUserCircle size={24} style={{ color: "#333" }} />
                
                <span style={{ fontSize: "16px", fontWeight: "500", color: "#333" }}>
                    {currentUser?.fullName ?? "Guest"}
                </span>

                <button
                    className="btn btn-outline-danger btn-sm"
                    onClick={handleLogout}
                    style={{
                        borderColor: "#dc3545",
                        color: "#dc3545",
                        padding: "6px 16px",
                        fontSize: "14px"
                    }}
                    onMouseEnter={(e) => {
                        e.currentTarget.style.backgroundColor = "#dc3545";
                        e.currentTarget.style.color = "#fff";
                    }}
                    onMouseLeave={(e) => {
                        e.currentTarget.style.backgroundColor = "#fff";
                        e.currentTarget.style.color = "#dc3545";
                    }}
                >
                    Logout
                </button>
            </div>
        </div>
    );
}

export default ProfileHeader;
