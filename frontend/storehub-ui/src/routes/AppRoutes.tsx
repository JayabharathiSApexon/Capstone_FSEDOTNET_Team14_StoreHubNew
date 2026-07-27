import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import ProductList from "../pages/product/ProductList";
import AddEditProduct from "../pages/product/AddEditProduct";

function AppRoutes() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Navigate to="/products" />} />
                <Route path="/products" element={<ProductList />} />
                <Route path="/products/add" element={<AddEditProduct />} />
            </Routes>
        </BrowserRouter>
    );
}

export default AppRoutes;