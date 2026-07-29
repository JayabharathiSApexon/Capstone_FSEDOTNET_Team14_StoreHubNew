import { useEffect, useState } from "react";
import Layout from "../../components/common/Layout";
import Pagination from "../../components/common/Pagination";
import InventoryTable from "../../components/inventory/InventoryTable";
import { ProductResponse } from "../../models/product/ProductResponse";
import { getProducts } from "../../services/productService";

function InventoryList() {

    const [inventory, setInventory] =
        useState<ProductResponse[]>([]);

    const [loading, setLoading] =
        useState(true);

    const itemsPerPage = 10;

    const [currentPage, setCurrentPage] =
        useState(1);

    useEffect(() => {

        loadInventory();

    }, []);

    const loadInventory = async () => {

        try {

            const data = await getProducts();

            setInventory(data);

        }
        catch (error) {

            console.error(error);

        }
        finally {

            setLoading(false);

        }

    };

    const indexOfLastItem =
        currentPage * itemsPerPage;

    const indexOfFirstItem =
        indexOfLastItem - itemsPerPage;

    const currentInventory =
        inventory.slice(
            indexOfFirstItem,
            indexOfLastItem
        );

    return (

        <Layout>

            <div className="d-flex justify-content-between align-items-center mb-4">

                <div>

                    <h3 className="mb-0">
                        Inventory Management
                    </h3>

                    <small className="text-muted">
                        Monitor product inventory and stock levels
                    </small>

                </div>

            </div>

            <div className="card shadow-sm">

                <div className="card-body">

                    {

                        loading

                            ?

                            <p>Loading...</p>

                            :

                            <>

                                <InventoryTable
                                    inventory={currentInventory}
                                />

                                <Pagination
                                    currentPage={currentPage}
                                    totalItems={inventory.length}
                                    itemsPerPage={itemsPerPage}
                                    onPageChange={setCurrentPage}
                                />

                            </>

                    }

                </div>

            </div>

        </Layout>

    );

}

export default InventoryList;