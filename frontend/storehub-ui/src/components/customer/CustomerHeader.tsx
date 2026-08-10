import { FaShoppingCart, FaBell, FaUserCircle, FaSearch } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { getCurrentUser, logout } from "../../services/authService";
import { useCart } from "../../context/CartContext";



interface CustomerHeaderProps {

    searchTerm: string;

    onSearchChange: (value: string) => void;

    isShoppingCart?: boolean;
}


function CustomerHeader({

    searchTerm,

    onSearchChange,

    isShoppingCart = false

}: CustomerHeaderProps) {

    const navigate = useNavigate();

    const currentUser = getCurrentUser();

    const {
        cartCount
    } = useCart();


    const handleLogout = () => {

        logout();

        navigate("/login");

    };


    return (

        <nav
            className="navbar navbar-light bg-white border-bottom px-4"
            style={{
                position: "fixed",
                top: 0,
                left: "240px",
                right: 0,
                height: "72px",
                zIndex: 999,
                backgroundColor: "#fff"
            }}
        >

            <div className="d-flex align-items-center w-100">

                {
                    isShoppingCart ? (

                        <>
                            {/* Shopping Cart Page */}

                            <span className="fw-semibold fs-5">

                                Shopping Cart

                            </span>


                            <div
                                className="ms-auto position-relative"
                                style={{
                                    cursor: "pointer"
                                }}
                                onClick={() =>
                                    navigate("/shopping-cart")
                                }
                            >

                                <FaShoppingCart
                                    size={24}
                                />

                                {
                                    cartCount > 0 && (

                                        <span
                                            className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger"
                                        >
                                            {cartCount}
                                        </span>

                                    )
                                }

                            </div>

                        </>

                    ) : (

                        <>
                            {/* Normal Customer Pages */}

                            <div
                                className="position-relative"
                                style={{
                                    maxWidth: "520px",
                                    width: "100%"
                                }}
                            >

                                <FaSearch
                                    className="position-absolute text-secondary"
                                    style={{
                                        left: "16px",
                                        top: "50%",
                                        transform: "translateY(-50%)"
                                    }}
                                />

                                <input
                                    type="text"
                                    className="form-control shadow-sm"
                                    placeholder="Search products..."
                                    value={searchTerm}
                                    onChange={(e) =>
                                        onSearchChange(e.target.value)
                                    }
                                    style={{
                                        height: "48px",
                                        borderRadius: "25px",
                                        paddingLeft: "45px"
                                    }}
                                />

                            </div>


                            <div className="ms-auto d-flex align-items-center">

                                {/* Cart */}

                                <div
                                    className="position-relative me-4"
                                    style={{
                                        cursor: "pointer"
                                    }}
                                    onClick={() =>
                                        navigate("/shopping-cart")
                                    }
                                >

                                    <FaShoppingCart
                                        size={22}
                                    />

                                    {
                                        cartCount > 0 && (

                                            <span
                                                className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger"
                                            >
                                                {cartCount}
                                            </span>

                                        )
                                    }

                                </div>


                                {/* Notification */}

                                <FaBell
                                    size={20}
                                    className="me-4"
                                />


                                {/* User */}

                                <FaUserCircle
                                    size={24}
                                    className="me-2"
                                />

                                <span className="me-3">

                                    {currentUser?.fullName ?? "Guest"}

                                </span>


                                {/* Logout */}

                                <button
                                    className="btn btn-outline-danger btn-sm"
                                    onClick={handleLogout}
                                >
                                    Logout
                                </button>

                            </div>

                        </>

                    )
                }

            </div>

        </nav>

    );

}


export default CustomerHeader;