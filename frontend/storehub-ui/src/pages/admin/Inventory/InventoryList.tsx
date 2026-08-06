import Layout from "../../../components/admin/AdminLayout";
import Pagination from "../../../components/common/Pagination";
import InventoryTable from "../../../components/admin/inventory/InventoryTable";
import { useInventoryManagement } from "../../../hooks/admin/useInventoryManagement";

function InventoryList() {
    const {
        inventory,
        currentInventory,
        currentPage,
        itemsPerPage,
        loading,
        error,
        setCurrentPage
    } = useInventoryManagement();

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

                            error

                                ?

                                <div className="alert alert-danger mb-0">
                                    {error}
                                </div>

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