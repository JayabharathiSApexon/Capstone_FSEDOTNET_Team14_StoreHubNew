import { FaEdit, FaTrash } from "react-icons/fa";
import { ProductResponse } from "../../models/product/ProductResponse";

interface ProductTableProps {
    products: ProductResponse[];
    onEdit: (product: ProductResponse) => void;
    onDelete: (id: string, name: string) => void;
}

function ProductTable({ products, onEdit, onDelete }: ProductTableProps) {
    return (
        <div className="table-responsive">
            <table className="table table-hover align-middle">
                <thead className="table-light">
                    <tr>
                        <th>Image</th>
                        <th>Product</th>
                        <th>Category</th>
                        <th>Brand</th>
                        <th>Price</th>
                        <th>Stock</th>
                        <th className="text-center">Actions</th>
                    </tr>
                </thead>

                <tbody>
                    {products.length > 0 ? (
                        products.map((product) => (
                            <tr key={product.id}>

                                <td>
                                    {product.images.length > 0 ? (
                                        <img
                                            src={`http://localhost:5103${product.images[0].imageUrl}`}
                                            alt={product.name}
                                            width="60"
                                            height="60"
                                            style={{
                                                objectFit: "cover",
                                                borderRadius: "8px"
                                            }}
                                        />
                                    ) : (
                                        <span className="text-muted">No Image</span>
                                    )}
                                </td>

                                <td>{product.name}</td>
                                <td>{product.categoryName}</td>
                                <td>{product.brand}</td>
                                <td>₹{product.price}</td>
                                <td>{product.stockQuantity}</td>

                                <td className="text-center">
                                    <button
                                        className="btn btn-sm btn-outline-primary me-2"
                                        title="Edit"
                                        onClick={() => onEdit(product)}
                                    >
                                        <FaEdit />
                                    </button>

                                    <button
                                        className="btn btn-sm btn-outline-danger"
                                        title="Delete"
                                        onClick={() => onDelete(product.id, product.name)}
                                    >
                                        <FaTrash />
                                    </button>
                                </td>

                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan={7} className="text-center">
                                No products found.
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
}

export default ProductTable;