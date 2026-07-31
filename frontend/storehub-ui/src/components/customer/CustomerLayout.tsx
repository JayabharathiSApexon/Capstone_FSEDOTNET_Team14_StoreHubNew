import { ReactNode } from "react";
import CustomerSidebar from "./CustomerSidebar";
import CustomerHeader from "./CustomerHeader";

interface CustomerLayoutProps {
    children: ReactNode;
}

function CustomerLayout({
    children
}: CustomerLayoutProps) {

    return (

        <div className="d-flex">

            <CustomerSidebar />

            <div className="flex-grow-1">

                <CustomerHeader />

                <div className="container-fluid p-4">

                    {children}

                </div>

            </div>

        </div>

    );
}

export default CustomerLayout;