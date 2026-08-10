import { useEffect, useMemo, useState } from "react";
import { CategoryResponse } from "../../models/category/CategoryResponse";
import { ProductResponse } from "../../models/product/ProductResponse";
import { getCategories } from "../../services/categoryService";
import { getProducts } from "../../services/productService";
import { useAsyncState } from "../common/useAsyncState";

export const useProductListing = () => {
    const [products, setProducts] = useState<ProductResponse[]>([]);
    const [categories, setCategories] = useState<CategoryResponse[]>([]);

    const [selectedCategory, setSelectedCategory] = useState("");
    const [minPrice, setMinPrice] = useState("");
    const [maxPrice, setMaxPrice] = useState("");
    const [sortBy, setSortBy] = useState("");

    const { loading, error, runSafely } = useAsyncState(true);

    useEffect(() => {
        void loadData();
    }, []);

    const loadData = async () => {
        const data = await runSafely(
            async () => await Promise.all([getProducts(), getCategories()]),
            {
                defaultErrorMessage: "Failed to load products.",
                onError: errorValue => console.error(errorValue)
            }
        );

        if (!data) {
            return;
        }

        const [productData, categoryData] = data;
        setProducts(productData.filter(product => product.isActive));
        setCategories(categoryData);
    };

    const getFilteredProducts = useMemo(() => {
        return (searchTerm: string) => {
            return products
                .filter(product => {
                    if (!searchTerm.trim()) {
                        return true;
                    }

                    const normalizedSearch = searchTerm.toLowerCase();

                    return (
                        product.name.toLowerCase().includes(normalizedSearch) ||
                        product.description?.toLowerCase().includes(normalizedSearch) ||
                        product.brand?.toLowerCase().includes(normalizedSearch)
                    );
                })
                .filter(product =>
                    selectedCategory
                        ? product.categoryId === selectedCategory
                        : true
                )
                .filter(product =>
                    minPrice
                        ? product.price >= Number(minPrice)
                        : true
                )
                .filter(product =>
                    maxPrice
                        ? product.price <= Number(maxPrice)
                        : true
                )
                .sort((a, b) => {
                    switch (sortBy) {
                        case "priceLow":
                            return a.price - b.price;
                        case "priceHigh":
                            return b.price - a.price;
                        case "name":
                            return a.name.localeCompare(b.name);
                        default:
                            return 0;
                    }
                });
        };
    }, [maxPrice, minPrice, products, selectedCategory, sortBy]);

    return {
        categories,
        loading,
        error,
        selectedCategory,
        minPrice,
        maxPrice,
        sortBy,
        setSelectedCategory,
        setMinPrice,
        setMaxPrice,
        setSortBy,
        getFilteredProducts
    };
};
