import {
    FaShoppingCart,
    FaBell,
    FaUserCircle,
    FaSearch
} from "react-icons/fa";

interface CustomerHeaderProps {
    searchTerm: string;
    onSearchChange: (value: string) => void;
}

function CustomerHeader({
    searchTerm,
    onSearchChange
}: CustomerHeaderProps) {

    return (
        <nav className="navbar navbar-light bg-white border-bottom px-4 py-3">

            <div className="d-flex align-items-center w-100">

                <div
                    className="position-relative"
                    style={{ maxWidth: "500px", width: "100%" }}
                >
                    <FaSearch
                        className="position-absolute text-secondary"
                        style={{
                            left: "16px",
                            top: "50%",
                            transform: "translateY(-50%)",
                            pointerEvents: "none"
                        }}
                    />

                    <input
                        type="text"
                        className="form-control shadow-sm"
                        placeholder="Search products..."
                        value={searchTerm}
                        onChange={(e) => onSearchChange(e.target.value)}
                        style={{
                            borderRadius: "25px",
                            paddingLeft: "45px",
                            height: "48px"
                        }}
                    />
                </div>

                <div className="ms-auto d-flex align-items-center">

                    <FaShoppingCart size={20} className="me-4" />
                    <FaBell size={20} className="me-4" />
                    <FaUserCircle size={24} className="me-2" />

                    <span>Guest</span>

                </div>

            </div>

        </nav>
    );
}

export default CustomerHeader;