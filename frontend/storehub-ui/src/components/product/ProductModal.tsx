import { Modal } from "react-bootstrap";
import ProductForm from "./ProductForm";
import { ProductRequest } from "../../models/product/ProductRequest";
import { CategoryResponse } from "../../models/category/CategoryResponse";

interface ProductModalProps {
    show: boolean;
    title: string;
    product?: ProductRequest;
    categories: CategoryResponse[];
    onSubmit: (product: ProductRequest) => void;
    onClose: () => void;
}

function ProductModal({
    show,
    title,
    product,
    categories,
    onSubmit,
    onClose
}: ProductModalProps) {


    return (
        <Modal
            show={show}
            onHide={onClose}
            centered
            backdrop="static"
            dialogClassName="modal-md"
        >
            <Modal.Header closeButton>
                <Modal.Title>
                    {title}
                </Modal.Title>
            </Modal.Header>

            <Modal.Body>

                <ProductForm
                    product={product}
                    categories={categories}
                    onSubmit={onSubmit}
                    onCancel={onClose}
                />

            </Modal.Body>
        </Modal>
    );
}

export default ProductModal;