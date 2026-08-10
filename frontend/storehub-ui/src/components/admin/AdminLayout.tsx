import { ReactNode } from "react";
import AdminSidebar from "../admin/AdminSidebar";
import Header from "./AdminHeader";

interface AdminLayoutProps {
    children: ReactNode;
    showHeader?: boolean;
}

function AdminLayout({
    children,
    showHeader = true
}: AdminLayoutProps) {

    return (

        <div className="d-flex">

            <AdminSidebar />

            <div className="flex-grow-1">

                {showHeader && <Header />}

                <div className="container-fluid p-4">

                    {children}

                </div>

            </div>

        </div>

    );

}

export default AdminLayout;