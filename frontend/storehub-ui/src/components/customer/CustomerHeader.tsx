import {
    FaShoppingCart,
    FaBell,
    FaUserCircle
} from "react-icons/fa";

function CustomerHeader() {

    return (

        <nav className="navbar navbar-light bg-white border-bottom px-4">

            <div className="d-flex align-items-center w-100">

                <input
                    type="text"
                    className="form-control me-3"
                    placeholder="Search products..."
                    style={{ maxWidth: "400px" }}
                />

                <div className="ms-auto d-flex align-items-center">

                    <FaShoppingCart
                        size={20}
                        className="me-4"
                    />

                    <FaBell
                        size={20}
                        className="me-4"
                    />

                    <FaUserCircle
                        size={24}
                        className="me-2"
                    />

                    <span>
                        Guest
                    </span>

                </div>

            </div>

        </nav>

    );
}

export default CustomerHeader;