import { useNavigate } from "react-router-dom";
import { Card, Button, Badge } from "react-bootstrap";
import { ProductResponse } from "../../../models/product/ProductResponse";

interface ProductCardProps {
    product: ProductResponse;
    onAddToCart: (product: ProductResponse) => void;
}

function ProductCard({
    product,
    onAddToCart
}: ProductCardProps) {

    const navigate = useNavigate();

    const primaryImage =
        product.images.find(image => image.isPrimary) ??
        product.images[0];

    const apiBaseUrl = import.meta.env.VITE_API_URL.replace("/api", "");

    const imageUrl = primaryImage
        ? `${apiBaseUrl}${primaryImage.imageUrl}`
        : "https://via.placeholder.com/300x220?text=No+Image";

    return (

        <Card
            className="h-100 border-0 shadow-sm position-relative"
            style={{
                cursor: "pointer",
                transition: "all 0.2s ease"
            }}
        >

            {product.isFeatured && (

                <Badge
                    bg="danger"
                    className="position-absolute top-0 end-0 m-2"
                >
                    Featured
                </Badge>

            )}

            <div
                onClick={() =>
                    navigate(`/customer/products/${product.id}`)
                }
                style={{
                    cursor: "pointer"
                }}
            >
                <Card.Img
                    variant="top"
                    src={imageUrl}
                    alt={product.name}
                    style={{
                        height: "220px",
                        width: "100%",
                        objectFit: "contain",
                        padding: "10px",
                        backgroundColor: "#fff"
                    }}
                />
            </div>

            <Card.Body className="d-flex flex-column">

                <h6
                    className="fw-semibold mb-1 text-primary"
                    onClick={() =>
                        navigate(`/customer/products/${product.id}`)
                    }
                    style={{
                        cursor: "pointer"
                    }}
                >
                    {product.name}
                </h6>

                <small className="text-muted mb-2">
                    {product.categoryName}
                </small>

                <div className="fw-bold fs-5 text-dark mb-1">
                    ₹{product.price.toLocaleString("en-IN")}
                </div>

                <small
                    className={
                        product.stockQuantity > 0
                            ? "text-primary mb-3"
                            : "text-danger mb-3"
                    }
                >
                    Stock: {product.stockQuantity}
                </small>

                <Button
                    variant="primary"
                    size="sm"
                    className="mt-auto w-100"
                    onClick={() => onAddToCart(product)}
                >
                    Add To Cart
                </Button>

            </Card.Body>

        </Card>

    );
}

export default ProductCard;