    import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
    import ProductList from "../pages/admin/product/ProductList";
    import InventoryList from "../pages/admin/Inventory/InventoryList";
    import OrderManagement from "../pages/admin/order/OrderManagement";
    import ProductListing from "../pages/customer/ProductListing";

    function AppRoutes() {

        return (

            <BrowserRouter>

                <Routes>

                    {/* <Route
                        path="/customer"
                        element={<ProductListing />}
                    /> */}

                    <Route
                        path="/"
                        element={<Navigate to="/admin/products" />}
                    />

                    <Route
                        path="/admin/products"
                        element={<ProductList />}
                    />

                    <Route
                        path="/admin/inventory"
                        element={<InventoryList />}
                    />

                    <Route
                        path="/admin/orders"
                        element={<OrderManagement />}
                    />

                </Routes>

            </BrowserRouter>

        );

    }

    export default AppRoutes;