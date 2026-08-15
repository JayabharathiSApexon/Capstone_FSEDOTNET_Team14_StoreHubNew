import { useEffect, useState } from "react";
import { Badge, Button, Card, Col, Row } from "react-bootstrap";
import { useNavigate, useParams } from "react-router-dom";

import CustomerLayout from "../../components/customer/CustomerLayout";
import MessageModal from "../../components/common/MessageModal";

import { ProductResponse } from "../../models/product/ProductResponse";
import { ProductImageResponse } from "../../models/product/ProductImageResponse";

import { getProductById } from "../../services/productService";
import { addToCart } from "../../services/cartService";

import { useCart } from "../../context/CartContext";

function ProductDetails() {

    const { id } = useParams<{ id: string }>();

    const navigate = useNavigate();

    const { loadCart } = useCart();

    const [product, setProduct] =
        useState<ProductResponse | null>(null);

    const [selectedImage, setSelectedImage] =
        useState<ProductImageResponse | null>(null);

    const [quantity, setQuantity] =
        useState(1);

    const [loading, setLoading] =
        useState(true);

    const [addingToCart, setAddingToCart] =
        useState(false);

    const [error, setError] =
        useState("");

    const [showMessageModal, setShowMessageModal] =
        useState(false);

    const [messageTitle, setMessageTitle] =
        useState("");

    const [messageText, setMessageText] =
        useState("");

    const [messageType, setMessageType] =
        useState<"success" | "danger">("success");


    /*
     * API base URL
     *
     * Example:
     * VITE_API_URL = https://localhost:7001/api
     *
     * Image URL returned from API:
     * /uploads/products/iphone.jpg
     *
     * Final URL:
     * https://localhost:7001/uploads/products/iphone.jpg
     */
    const apiBaseUrl =
        import.meta.env.VITE_API_URL.replace("/api", "");


    /*
     * Load product
     */
    useEffect(() => {

        if (!id) {

            setError("Product ID is missing.");

            setLoading(false);

            return;
        }

        loadProduct(id);

    }, [id]);


    const loadProduct = async (productId: string) => {

        try {

            setLoading(true);

            setError("");

            const productData =
                await getProductById(productId);

            setProduct(productData);


            /*
             * Select primary image first.
             * If primary image doesn't exist,
             * use the first image.
             */
            const primaryImage =
                productData.images?.find(
                    image => image.isPrimary
                ) ??
                productData.images?.[0] ??
                null;

            setSelectedImage(primaryImage);

        }
        catch (error) {

            console.error(
                "Failed to load product.",
                error
            );

            setError(
                "Failed to load product details."
            );

        }
        finally {

            setLoading(false);

        }

    };


    /*
     * Increase quantity
     */
    const handleIncreaseQuantity = () => {

        if (!product) {
            return;
        }

        if (quantity < product.stockQuantity) {

            setQuantity(previous => previous + 1);

        }

    };


    /*
     * Decrease quantity
     */
    const handleDecreaseQuantity = () => {

        if (quantity > 1) {

            setQuantity(previous => previous - 1);

        }

    };


    /*
     * Add product to cart
     */
    const handleAddToCart = async () => {

        if (!product) {
            return;
        }

        if (product.stockQuantity <= 0) {

            setMessageTitle("Error");

            setMessageText(
                "This product is currently out of stock."
            );

            setMessageType("danger");

            setShowMessageModal(true);

            return;
        }


        try {

            setAddingToCart(true);


            await addToCart({

                productId: product.id,

                quantity: quantity

            });


            /*
             * Reload cart from server.
             *
             * This updates the existing
             * CartContext and cart count.
             */
            await loadCart();


            setMessageTitle("Success");

            setMessageText(
                quantity === 1
                    ? `"${product.name}" has been added to your cart.`
                    : `${quantity} x "${product.name}" has been added to your cart.`
            );

            setMessageType("success");

            setShowMessageModal(true);


            /*
             * Reset quantity after successful
             * add to cart.
             */
            setQuantity(1);

        }
        catch (error) {

            console.error(
                "Unable to add product to cart.",
                error
            );

            setMessageTitle("Error");

            setMessageText(
                "Unable to add product to cart."
            );

            setMessageType("danger");

            setShowMessageModal(true);

        }
        finally {

            setAddingToCart(false);

        }

    };


    /*
     * Build image URL
     */
    const getImageUrl = (
        image: ProductImageResponse | null
    ) => {

        if (!image) {

            return "https://via.placeholder.com/600x500?text=No+Image";

        }

        /*
         * If backend already returns an absolute URL,
         * don't add the API base URL again.
         */
        if (
            image.imageUrl.startsWith("http://") ||
            image.imageUrl.startsWith("https://")
        ) {

            return image.imageUrl;

        }

        return `${apiBaseUrl}${image.imageUrl}`;

    };


    /*
     * Loading
     */
    if (loading) {

        return (

            <CustomerLayout>

                {() => (

                    <div className="container-fluid py-5">

                        <div className="text-center">

                            <h5>
                                Loading Product...
                            </h5>

                        </div>

                    </div>

                )}

            </CustomerLayout>

        );

    }


    /*
     * Error
     */
    if (error || !product) {

        return (

            <CustomerLayout>

                {() => (

                    <div className="container-fluid py-5">

                        <div className="alert alert-danger">

                            {error ||
                                "Product not found."}

                        </div>

                        <Button
                            variant="secondary"
                            onClick={() =>
                                navigate("/customer")
                            }
                        >
                            Back to Products
                        </Button>

                    </div>

                )}

            </CustomerLayout>

        );

    }


    return (

        <CustomerLayout showHeader={false}>

            {() => (

                <>

                    <div
                        className="container-fluid py-4"
                        style={{
                            maxWidth: "1600px",
                            margin: "0 auto"
                        }}
                    >

                        {/* Page Header */}

                        <div className="d-flex align-items-center justify-content-between mb-4">

                            {/* Left side */}

                            <div>

                                <h2 className="fw-bold mb-1">
                                    Product Details
                                </h2>

                                <p className="text-muted mb-0">
                                    View product information and add it to your cart.
                                </p>

                            </div>


                            {/* Right side */}

                            <Button
                                variant="outline-secondary"
                                size="lg"
                                onClick={() => navigate(-1)}
                            >
                                ← Back
                            </Button>

                        </div>


                        <Card
                            className="border-0 shadow-sm"
                        >

                            <Card.Body className="p-4">

                                <Row>

                                    {/* ========================= */}
                                    {/* Product Images */}
                                    {/* ========================= */}

                                    <Col
                                        lg={6}
                                        md={6}
                                        className="mb-4 mb-md-0"
                                    >

                                        <div
                                            className="position-relative border rounded p-3"
                                            style={{
                                                minHeight: "500px",
                                                backgroundColor: "#fff"
                                            }}
                                        >

                                            {product.isFeatured && (

                                                <Badge
                                                    bg="danger"
                                                    className="position-absolute top-0 end-0 m-3"
                                                    style={{
                                                        zIndex: 2
                                                    }}
                                                >
                                                    Featured
                                                </Badge>

                                            )}


                                            <div
                                                className="d-flex align-items-center justify-content-center"
                                                style={{
                                                    height: "450px"
                                                }}
                                            >

                                                <img
                                                    src={getImageUrl(
                                                        selectedImage
                                                    )}
                                                    alt={
                                                        product.name
                                                    }
                                                    className="img-fluid"
                                                    style={{
                                                        maxHeight: "430px",
                                                        maxWidth: "100%",
                                                        objectFit: "contain"
                                                    }}
                                                />

                                            </div>

                                        </div>


                                        {/* Image thumbnails */}

                                        {product.images &&
                                            product.images.length > 0 && (

                                                <div
                                                    className="d-flex gap-2 mt-3 flex-wrap"
                                                >

                                                    {product.images
                                                        .sort(
                                                            (a, b) =>
                                                                a.displayOrder -
                                                                b.displayOrder
                                                        )
                                                        .map(image => (

                                                            <button
                                                                type="button"
                                                                key={image.id}
                                                                onClick={() =>
                                                                    setSelectedImage(
                                                                        image
                                                                    )
                                                                }
                                                                className="p-1 bg-white rounded"
                                                                style={{
                                                                    width: "80px",
                                                                    height: "80px",
                                                                    border:
                                                                        selectedImage?.id === image.id
                                                                            ? "2px solid #0d6efd"
                                                                            : "1px solid #dee2e6"
                                                                }}
                                                            >

                                                                <img
                                                                    src={getImageUrl(
                                                                        image
                                                                    )}
                                                                    alt={
                                                                        product.name
                                                                    }
                                                                    style={{
                                                                        width: "100%",
                                                                        height: "100%",
                                                                        objectFit: "contain"
                                                                    }}
                                                                />

                                                            </button>

                                                        ))}

                                                </div>

                                            )}

                                    </Col>


                                    {/* ========================= */}
                                    {/* Product Information */}
                                    {/* ========================= */}

                                    <Col
                                        lg={6}
                                        md={6}
                                    >

                                        <div className="mb-2">

                                            <small className="text-muted">
                                                {product.categoryName}
                                            </small>

                                        </div>


                                        <h2 className="fw-bold mb-2">

                                            {product.name}

                                        </h2>


                                        {product.brand && (

                                            <p className="text-muted mb-3">

                                                Brand:
                                                {" "}
                                                <strong>
                                                    {product.brand}
                                                </strong>

                                            </p>

                                        )}


                                        <h3
                                            className="fw-bold text-primary mb-3"
                                        >

                                            ₹
                                            {product.price.toLocaleString(
                                                "en-IN"
                                            )}

                                        </h3>


                                        {/* Stock */}

                                        <div className="mb-4">

                                            {product.stockQuantity > 0 ? (

                                                <span className="text-primary">

                                                    In Stock:
                                                    {" "}
                                                    <strong>
                                                        {
                                                            product.stockQuantity
                                                        }
                                                    </strong>

                                                </span>

                                            ) : (

                                                <span className="text-danger fw-semibold">

                                                    Out of Stock

                                                </span>

                                            )}

                                        </div>


                                        <hr />


                                        {/* Description */}

                                        <div className="mb-4">

                                            <h5 className="fw-bold mb-3">

                                                Description

                                            </h5>

                                            <p
                                                className="text-muted"
                                                style={{
                                                    whiteSpace: "pre-line"
                                                }}
                                            >

                                                {product.description ||
                                                    "No description available."}

                                            </p>

                                        </div>


                                        {/* Quantity */}

                                        {product.stockQuantity > 0 && (

                                            <div className="mb-4">

                                                <label
                                                    className="form-label fw-semibold"
                                                >
                                                    Quantity
                                                </label>

                                                <div
                                                    className="d-flex align-items-center"
                                                    style={{
                                                        width: "150px"
                                                    }}
                                                >

                                                    <Button
                                                        variant="outline-secondary"
                                                        onClick={
                                                            handleDecreaseQuantity
                                                        }
                                                        disabled={
                                                            quantity <= 1 ||
                                                            addingToCart
                                                        }
                                                    >
                                                        -
                                                    </Button>


                                                    <div
                                                        className="border-top border-bottom d-flex align-items-center justify-content-center"
                                                        style={{
                                                            height: "38px",
                                                            width: "60px"
                                                        }}
                                                    >
                                                        {quantity}
                                                    </div>


                                                    <Button
                                                        variant="outline-secondary"
                                                        onClick={
                                                            handleIncreaseQuantity
                                                        }
                                                        disabled={
                                                            quantity >=
                                                            product.stockQuantity ||
                                                            addingToCart
                                                        }
                                                    >
                                                        +
                                                    </Button>

                                                </div>

                                            </div>

                                        )}


                                        {/* Add To Cart */}

                                        <Button
                                            variant="primary"
                                            size="lg"
                                            className="w-100"
                                            disabled={
                                                product.stockQuantity <= 0 ||
                                                addingToCart
                                            }
                                            onClick={
                                                handleAddToCart
                                            }
                                        >

                                            {addingToCart
                                                ? "Adding..."
                                                : product.stockQuantity > 0
                                                    ? "Add To Cart"
                                                    : "Out of Stock"}

                                        </Button>


                                        {/* View Cart */}

                                        <Button
                                            variant="outline-primary"
                                            size="lg"
                                            className="w-100 mt-2"
                                            onClick={() =>
                                                navigate(
                                                    "/shopping-cart"
                                                )
                                            }
                                        >
                                            View Shopping Cart
                                        </Button>

                                    </Col>

                                </Row>

                            </Card.Body>

                        </Card>

                    </div>


                    {/* Message Modal */}

                    <MessageModal
                        show={showMessageModal}
                        title={messageTitle}
                        message={messageText}
                        variant={messageType}
                        onClose={() =>
                            setShowMessageModal(false)
                        }
                    />

                </>

            )}

        </CustomerLayout>

    );

}

export default ProductDetails;