import { ReactNode, useState } from "react";
import CustomerHeader from "./CustomerHeader";
import CustomerSidebar from "./CustomerSidebar";

interface CustomerLayoutProps {

    children: (searchTerm: string) => ReactNode;

    showHeader?: boolean;

    isShoppingCart?: boolean;
}

function CustomerLayout({

    children,

    showHeader = true,

    isShoppingCart = false

}: CustomerLayoutProps) {

    const [searchTerm, setSearchTerm] = useState("");

    return (

        <div>

            <CustomerSidebar />

            {
                showHeader && (

                    <CustomerHeader
                        searchTerm={searchTerm}
                        onSearchChange={setSearchTerm}
                        isShoppingCart={isShoppingCart}
                    />

                )
            }

            <div
                style={{
                    marginLeft: "240px",
                    marginTop: showHeader
                        ? "72px"
                        : "0px",
                    backgroundColor: "#f4f6fb",
                    minHeight: "100vh"
                }}
            >

                <main className="px-4 pt-2 pb-4">

                    {children(searchTerm)}

                </main>

            </div>

        </div>

    );

}

export default CustomerLayout;