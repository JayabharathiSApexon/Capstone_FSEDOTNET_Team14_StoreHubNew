import { FaEdit, FaTrash } from "react-icons/fa";
import { ProductResponse } from "../../models/product/ProductResponse";

interface ProductTableProps {
    products: ProductResponse[];
}

function ProductTable({ products }: ProductTableProps) {
    return (
        <div className="table-responsive">
            <table className="table table-hover align-middle">
                <thead className="table-light">
                    <tr>
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
                                <td>{product.name}</td>
                                <td>{product.categoryName}</td>
                                <td>{product.brand}</td>
                                <td>₹{product.price}</td>
                                <td>{product.stockQuantity}</td>

                                <td className="text-center">
                                    <button
                                        className="btn btn-sm btn-outline-primary me-2"
                                        title="Edit"
                                    >
                                        <FaEdit />
                                    </button>

                                    <button
                                        className="btn btn-sm btn-outline-danger"
                                        title="Delete"
                                    >
                                        <FaTrash />
                                    </button>
                                </td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan={6} className="text-center">
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