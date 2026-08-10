import { useEffect, useState } from "react";
import CustomerLayout from "../../components/customer/CustomerLayout";
import ProductGrid from "../../components/customer/product/ProductGrid";
import ProductFilters from "../../components/customer/product/ProductFilters";
import Pagination from "../../components/common/Pagination";
import MessageModal from "../../components/common/MessageModal";
import { ProductResponse } from "../../models/product/ProductResponse";
import { CategoryResponse } from "../../models/category/CategoryResponse";
import { getProducts } from "../../services/productService";
import { getCategories } from "../../services/categoryService";
import { addToCart } from "../../services/cartService";
import { useCart } from "../../context/CartContext";

function ProductListing() {

    const [products, setProducts] = useState<ProductResponse[]>([]);
    const [categories, setCategories] = useState<CategoryResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [selectedCategory, setSelectedCategory] = useState("");
    const [minPrice, setMinPrice] = useState("");
    const [maxPrice, setMaxPrice] = useState("");
    const [sortBy, setSortBy] = useState("");
    const itemsPerPage = 8;
    const [currentPage, setCurrentPage] = useState(1);
    const [showMessageModal, setShowMessageModal] = useState(false);
    const [messageTitle, setMessageTitle] = useState("");
    const [messageText, setMessageText] = useState("");
    const [messageType, setMessageType] = useState<"success" | "danger">("success");

    // Shared Cart Context
    const { setCart } = useCart();

    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {

        try {

            const [productData, categoryData] =
                await Promise.all([
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

    const handleAddToCart = async (
        product: ProductResponse
    ) => {

        try {

            // POST returns the latest cart
            const updatedCart = await addToCart({

                productId: product.id,

                quantity: 1

            });

            // Update shared cart context immediately
            setCart(updatedCart);

            setMessageTitle("Success");

            setMessageText(
                `"${product.name}" has been added to your cart.`
            );

            setMessageType("success");

            setShowMessageModal(true);

        }
        catch (error) {

            console.error(error);

            setMessageTitle("Error");

            setMessageText(
                "Unable to add product to cart."
            );

            setMessageType("danger");

            setShowMessageModal(true);

        }

    };

    return (

        <CustomerLayout>

            {(searchTerm) => {

                const filteredProducts = products

                    .filter(product => {

                        if (!searchTerm.trim()) {
                            return true;
                        }

                        return (

                            product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||

                            product.description?.toLowerCase().includes(searchTerm.toLowerCase()) ||

                            product.brand?.toLowerCase().includes(searchTerm.toLowerCase())

                        );

                    })

                    .filter(product =>

                        selectedCategory ? product.categoryId === selectedCategory : true

                    )

                    .filter(product =>

                        minPrice ? product.price >= Number(minPrice)  : true

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

                const indexOfLastItem = currentPage * itemsPerPage;

                const indexOfFirstItem = indexOfLastItem - itemsPerPage;

                const pagedProducts =
                    filteredProducts.slice(
                        indexOfFirstItem,
                        indexOfLastItem
                    );

                return (

                    <>

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
                                    onCategoryChange={(value) => {

                                        setSelectedCategory(value);
                                        setCurrentPage(1);

                                    }}
                                    onMinPriceChange={(value) => {

                                        setMinPrice(value);
                                        setCurrentPage(1);

                                    }}
                                    onMaxPriceChange={(value) => {

                                        setMaxPrice(value);
                                        setCurrentPage(1);

                                    }}
                                    onSortChange={(value) => {

                                        setSortBy(value);
                                        setCurrentPage(1);

                                    }}
                                />

                            </div>

                            {

                                loading

                                    ?

                                    <div className="text-center py-5">

                                        <h5>Loading Products...</h5>

                                    </div>

                                    :

                                    error

                                        ?

                                        <div className="alert alert-danger">

                                            {error}

                                        </div>

                                        :

                                        <>

                                            <ProductGrid
                                                products={pagedProducts}
                                                onAddToCart={handleAddToCart}
                                            />

                                            <Pagination
                                                currentPage={currentPage}
                                                totalItems={filteredProducts.length}
                                                itemsPerPage={itemsPerPage}
                                                onPageChange={setCurrentPage}
                                            />

                                        </>

                            }

                        </div>

                        <MessageModal
                            show={showMessageModal}
                            title={messageTitle}
                            message={messageText}
                            variant={messageType}
                            onClose={() => setShowMessageModal(false)}
                        />

                    </>

                );

            }}

        </CustomerLayout>

    );

}

export default ProductListing;