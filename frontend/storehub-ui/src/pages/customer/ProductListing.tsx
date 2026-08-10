import CustomerLayout from "../../components/customer/CustomerLayout";
import ProductGrid from "../../components/customer/product/ProductGrid";
import ProductFilters from "../../components/customer/product/ProductFilters";
import { useProductListing } from "../../hooks/customer/useProductListing";

function ProductListing() {
    const {
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
    } = useProductListing();

    return (

        <CustomerLayout>

            {(searchTerm) => {

                const filteredProducts = getFilteredProducts(searchTerm);

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