import Layout from "../../../components/admin/AdminLayout";
import Pagination from "../../../components/common/Pagination";
import OrderTable from "../../../components/admin/order/OrderTable";
import UpdateStatusModal from "../../../components/admin/order/UpdateStatusModal";
import { useOrderManagement } from "../../../hooks/admin/useOrderManagement";

function OrderManagement() {
    const {
        loading,
        showModal,
        selectedOrder,
        statusFilter,
        currentPage,
        itemsPerPage,
        pagedOrders,
        filteredOrders,
        setCurrentPage,
        setShowModal,
        handleUpdateStatus,
        handleSave,
        onChangeStatusFilter
    } = useOrderManagement();

    return (

        <Layout showHeader={false}>

            <div className="d-flex justify-content-between align-items-center mb-4">

                <div>

                    <h4 className="mb-0">

                        Order Management

                    </h4>

                    <small className="text-muted">
                        Manage customer orders
                    </small>

                </div>

                <div style={{ width: "180px" }}>

                    <select

                        className="form-select"

                        value={statusFilter}

                        onChange={(e) => {
                            onChangeStatusFilter(e.target.value);

                        }}

                    >

                        <option value="All">
                            All Status
                        </option>

                        <option value="Pending">
                            Pending
                        </option>

                        <option value="Processing">
                            Processing
                        </option>

                        <option value="Shipped">
                            Shipped
                        </option>

                        <option value="Delivered">
                            Delivered
                        </option>

                        <option value="Cancelled">
                            Cancelled
                        </option>

                    </select>

                </div>

            </div>

            <div className="card shadow-sm">

                <div className="card-body">

                    {
                        loading
                            ? <p>Loading...</p>
                            : <>
                                <OrderTable
                                    orders={pagedOrders}
                                    onUpdateStatus={handleUpdateStatus}
                                />

                                <Pagination
                                    currentPage={currentPage}
                                    totalItems={filteredOrders.length}
                                    itemsPerPage={itemsPerPage}
                                    onPageChange={setCurrentPage}
                                />
                            </>
                    }

                </div>

            </div>

            <UpdateStatusModal
                show={showModal}
                order={selectedOrder}
                onClose={() => setShowModal(false)}
                onSave={handleSave}
            />

        </Layout>

    );

}

export default OrderManagement;