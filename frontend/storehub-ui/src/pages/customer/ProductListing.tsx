import { useEffect, useState } from "react";

import CustomerLayout from "../../components/customer/CustomerLayout";
import ProductGrid from "../../components/customer/product/ProductGrid";
import ProductFilters from "../../components/customer/product/ProductFilters";

import { ProductResponse } from "../../models/product/ProductResponse";
import { CategoryResponse } from "../../models/category/CategoryResponse";

import { getProducts } from "../../services/productService";
import { getCategories } from "../../services/categoryService";

function ProductListing() {

    const [products, setProducts] = useState<ProductResponse[]>([]);
    const [categories, setCategories] = useState<CategoryResponse[]>([]);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [selectedCategory, setSelectedCategory] = useState("");
    const [minPrice, setMinPrice] = useState("");
    const [maxPrice, setMaxPrice] = useState("");
    const [sortBy, setSortBy] = useState("");

    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {

        try {

            const [productData, categoryData] = await Promise.all([
                getProducts(),
                getCategories()
            ]);

            setProducts(
                productData.filter(product => product.isActive)
            );

            setCategories(categoryData);

        }
        catch (err) {

            console.error(err);
            setError("Failed to load products.");

        }
        finally {

            setLoading(false);

        }

    };

    return (

        <CustomerLayout>

            {(searchTerm) => {

                const filteredProducts = products
                    .filter(product => {

                        if (!searchTerm.trim()) return true;

                        return (
                            product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                            product.description?.toLowerCase().includes(searchTerm.toLowerCase()) ||
                            product.brand?.toLowerCase().includes(searchTerm.toLowerCase())
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

                return (

                    <div className="container-fluid py-4">

                        <h2 className="fw-bold mb-4">
                            All Products
                        </h2>

                        <div className="mb-4">

                            <ProductFilters
                                categories={categories}
                                selectedCategory={selectedCategory}
                                minPrice={minPrice}
                                maxPrice={maxPrice}
                                sortBy={sortBy}
                                onCategoryChange={setSelectedCategory}
                                onMinPriceChange={setMinPrice}
                                onMaxPriceChange={setMaxPrice}
                                onSortChange={setSortBy}
                            />

                        </div>

                        {loading && (

                            <div className="text-center py-5">

                                <h5>Loading Products...</h5>

                            </div>

                        )}

                        {!loading && error && (

                            <div className="alert alert-danger">

                                {error}

                            </div>

                        )}

                        {!loading && !error && (

                            <ProductGrid
                                products={filteredProducts}
                            />

                        )}

                    </div>

                );

            }}

        </CustomerLayout>

    );

}

export default ProductListing;