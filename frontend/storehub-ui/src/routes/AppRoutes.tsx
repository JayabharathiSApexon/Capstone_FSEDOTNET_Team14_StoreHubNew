import { BrowserRouter, Navigate, Route, Routes, useLocation } from "react-router-dom";
import ProductList from "../pages/admin/product/ProductList";
import InventoryList from "../pages/admin/Inventory/InventoryList";
import OrderManagement from "../pages/admin/order/OrderManagement";
import UserManagement from "../pages/admin/user/UserManagement";
import AdminDashboard from "../pages/admin/dashboard/AdminDashboard";
import ProductListing from "../pages/customer/ProductListing";
import MyOrders from "../pages/customer/MyOrders";
import OrderDetails from "../pages/customer/OrderDetails";
import AuthPage from "../pages/auth/AuthPage";
import ShoppingCart from "../pages/customer/ShoppingCart";
import CheckoutPage from "../pages/customer/CheckoutPage";
import Profile from "../pages/customer/Profile";
import { getCurrentUser, isAuthenticated } from "../services/authService";

function AppRoutesContent() {
    const location = useLocation();

    const authenticated = isAuthenticated();
    const currentUser = getCurrentUser();
    const isAdmin = currentUser?.isAdmin ?? false;
    const defaultRoute = isAdmin ? "/admin/dashboard" : "/customer";

    return (

        <Routes key={location.pathname}>

            <Route
                path="/login"
                element={authenticated ? <Navigate to={defaultRoute} /> : <AuthPage mode="login" />}
            />

            <Route
                path="/register"
                element={authenticated ? <Navigate to={defaultRoute} /> : <AuthPage mode="register" />}
            />

            <Route
                path="/forgot-password"
                element={authenticated ? <Navigate to={defaultRoute} /> : <AuthPage mode="forgot-password" />}
            />

            <Route
                path="/customer"
                element={authenticated ? <ProductListing /> : <Navigate to="/login" />}
            />

            <Route
                path="/my-orders"
                element={authenticated ? <MyOrders /> : <Navigate to="/login" />}
            />

            <Route
                path="/customer/orders/:id"
                element={authenticated ? <OrderDetails /> : <Navigate to="/login" />}
            />

            <Route
                path="/shopping-cart"
                element={
                    authenticated
                        ? <ShoppingCart />
                        : <Navigate to="/login" />
                }
            />

            <Route
                path="/checkout"
                element={
                    authenticated
                        ? <CheckoutPage />
                        : <Navigate to="/login" />
                }
            />

            <Route
                path="/profile"
                element={ authenticated ? <Profile /> : <Navigate to="/login" /> }
            />

            <Route
                path="/"
                element={<Navigate to={authenticated ? defaultRoute : "/login"} />}
            />

            <Route
                path="/admin/products"
                element={authenticated
                    ? (isAdmin ? <ProductList /> : <Navigate to="/customer" />)
                    : <Navigate to="/login" />}
            />

            <Route
                path="/admin/dashboard"
                element={authenticated
                    ? (isAdmin ? <AdminDashboard /> : <Navigate to="/customer" />)
                    : <Navigate to="/login" />}
            />

            <Route
                path="/admin/inventory"
                element={authenticated
                    ? (isAdmin ? <InventoryList /> : <Navigate to="/customer" />)
                    : <Navigate to="/login" />}
            />

            <Route
                path="/admin/orders"
                element={
                    authenticated
                        ? (isAdmin ? <OrderManagement /> : <Navigate to="/customer" />)
                        : <Navigate to="/login" />
                }
            />

            <Route
                path="/admin/users"
                element={authenticated
                    ? (isAdmin ? <UserManagement /> : <Navigate to="/customer" />)
                    : <Navigate to="/login" />}
            />

        </Routes>

    );

}

function AppRoutes() {
    return (
        <BrowserRouter>
            <AppRoutesContent />
        </BrowserRouter>
    );
}

export default AppRoutes;
