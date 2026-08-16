import { useEffect, useMemo, useState } from "react";
import MyOrderResponse from "../../models/order/MyOrderResponse";
import { getOrders, updateOrderStatus } from "../../services/orderService";
import { useAsyncState } from "../common/useAsyncState";

export const useOrderManagement = () => {
    const [orders, setOrders] = useState<MyOrderResponse[]>([]);
    const { loading, runSafely } = useAsyncState(true);
    const [showModal, setShowModal] = useState(false);
    const [selectedOrder, setSelectedOrder] = useState<MyOrderResponse | null>(null);
    const [statusFilter, setStatusFilter] = useState("All");
    const [currentPage, setCurrentPage] = useState(1);

    const itemsPerPage = 5;

    useEffect(() => {
        void loadOrders();
    }, []);

    const loadOrders = async () => {
        const data = await runSafely(
            async () => await getOrders(),
            {
                defaultErrorMessage: "Failed to load orders.",
                onError: error => console.error(error)
            }
        );

        if (data) {
            setOrders(data);
        }
    };

    const handleUpdateStatus = (order: MyOrderResponse) => {
        setSelectedOrder(order);
        setShowModal(true);
    };

    const handleSave = async (status: string) => {
        if (!selectedOrder) {
            return;
        }

        await updateOrderStatus({
            orderId: selectedOrder.id,
            status
        });

        setShowModal(false);
        await loadOrders();
    };

    const filteredOrders = useMemo(() => {
        if (statusFilter === "All") {
            return orders;
        }

        return orders.filter(x => x.status === statusFilter);
    }, [orders, statusFilter]);

    const pagedOrders = useMemo(() => {
        return filteredOrders.slice(
            (currentPage - 1) * itemsPerPage,
            currentPage * itemsPerPage
        );
    }, [currentPage, filteredOrders]);

    const onChangeStatusFilter = (value: string) => {
        setStatusFilter(value);
        setCurrentPage(1);
    };

    return {
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
    };
};
