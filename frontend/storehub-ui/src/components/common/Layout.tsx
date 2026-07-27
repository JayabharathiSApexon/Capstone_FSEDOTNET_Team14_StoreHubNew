import { ReactNode } from "react";
import Sidebar from "./Sidebar";
import Header from "./Header";

interface LayoutProps {
    children: ReactNode;
}

function Layout({ children }: LayoutProps) {
    return (
        <div className="d-flex">
            <Sidebar />

            <div className="flex-grow-1">
                <Header />

                <div className="container-fluid p-4">
                    {children}
                </div>
            </div>
        </div>
    );
}

export default Layout;