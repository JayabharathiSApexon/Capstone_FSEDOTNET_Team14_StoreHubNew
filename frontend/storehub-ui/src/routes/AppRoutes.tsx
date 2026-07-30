import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import ProductList from "../pages/product/ProductList";
import InventoryList from "../pages/Inventory/InventoryList";
import OrderManagement from "../pages/order/OrderManagement";

function AppRoutes() {

    return (

        <BrowserRouter>

            <Routes>

                <Route
                    path="/"
                    element={<Navigate to="/products" />}
                />

                <Route
                    path="/products"
                    element={<ProductList />}
                />

                <Route
                    path="/inventory"
                    element={<InventoryList />}
                />

                <Route
                    path="/orders"
                    element={<OrderManagement />}
                />

            </Routes>

        </BrowserRouter>

    );

}

export default AppRoutes;