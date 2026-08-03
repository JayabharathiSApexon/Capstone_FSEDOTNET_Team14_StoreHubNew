import { Row, Col } from "react-bootstrap";
import ProductCard from "./ProductCard";
import { ProductResponse } from "../../../models/product/ProductResponse";

interface ProductGridProps {
    products: ProductResponse[];
}

function ProductGrid({ products }: ProductGridProps) {
    return (
        <Row>

            {products.map(product => (

                <Col
                    key={product.id}
                    xl={3}
                    lg={4}
                    md={6}
                    sm={6}
                    xs={12}
                    className="mb-4"
                >
                    <ProductCard product={product} />
                </Col>

            ))}

        </Row>
    );
}

export default ProductGrid;