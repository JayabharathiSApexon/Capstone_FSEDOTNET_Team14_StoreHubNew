import { ReactNode } from "react";
import AdminSidebar from "../admin/AdminSidebar";
import Header from "./AdminHeader";

interface AdminLayoutProps {
    children: ReactNode;
}

function AdminLayout({ children }: AdminLayoutProps) {
    return (
        <div className="d-flex">
            <AdminSidebar />

            <div className="flex-grow-1">
                <Header />

                <div className="container-fluid p-4">
                    {children}
                </div>
            </div>
        </div>
    );
}

export default AdminLayout;