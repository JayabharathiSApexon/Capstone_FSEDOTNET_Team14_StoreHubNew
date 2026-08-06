import { useEffect, useMemo, useState } from "react";
import { ProductResponse } from "../../models/product/ProductResponse";
import { getProducts } from "../../services/productService";
import { useAsyncState } from "../common/useAsyncState";

export const useInventoryManagement = () => {
    const [inventory, setInventory] = useState<ProductResponse[]>([]);
    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 10;

    const { loading, error, runSafely } = useAsyncState(true);

    useEffect(() => {
        void loadInventory();
    }, []);

    const loadInventory = async () => {
        const data = await runSafely(
            async () => await getProducts(),
            {
                defaultErrorMessage: "Failed to load inventory.",
                onError: errorValue => console.error(errorValue)
            }
        );

        if (data) {
            setInventory(data);
        }
    };

    const currentInventory = useMemo(() => {
        const indexOfLastItem = currentPage * itemsPerPage;
        const indexOfFirstItem = indexOfLastItem - itemsPerPage;
        return inventory.slice(indexOfFirstItem, indexOfLastItem);
    }, [currentPage, inventory]);

    return {
        inventory,
        currentInventory,
        currentPage,
        itemsPerPage,
        loading,
        error,
        setCurrentPage,
        reloadInventory: loadInventory
    };
};
