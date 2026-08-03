import { ReactNode, useState } from "react";
import CustomerHeader from "./CustomerHeader";
import CustomerSidebar from "./CustomerSidebar";

interface CustomerLayoutProps {
    children: (searchTerm: string) => ReactNode;
}

function CustomerLayout({ children }: CustomerLayoutProps) {

    const [searchTerm, setSearchTerm] = useState("");

    return (
        <div className="d-flex">

            <CustomerSidebar />

            <div
                className="flex-grow-1"
                style={{
                    backgroundColor: "#f4f6fb",
                    minHeight: "100vh"
                }}
            >
                <CustomerHeader
                    searchTerm={searchTerm}
                    onSearchChange={setSearchTerm}
                />

                <main className="p-4">
                    {children(searchTerm)}
                </main>

            </div>

        </div>
    );
}

export default CustomerLayout;