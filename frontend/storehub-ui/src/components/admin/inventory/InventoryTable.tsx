import { ProductResponse } from "../../../models/product/ProductResponse";

interface InventoryTableProps {
    inventory: ProductResponse[];
}

function InventoryTable({ inventory }: InventoryTableProps) {

    const getBadge = (stockQuantity: number) => {

        if (stockQuantity === 0) {
            return (
                <span className="badge bg-danger">
                    Out of Stock
                </span>
            );
        }

        if (stockQuantity <= 5) {
            return (
                <span className="badge bg-warning text-dark">
                    Low Stock
                </span>
            );
        }

        return (
            <span className="badge bg-success">
                In Stock
            </span>
        );
    };

    return (

        <div className="table-responsive">

            <table className="table table-hover align-middle">

                <thead className="table-light">

                    <tr>

                        <th>Product</th>

                        <th>Category</th>

                        <th className="text-center">
                            Stock
                        </th>

                        <th className="text-center">
                            Status
                        </th>

                    </tr>

                </thead>

                <tbody>

                    {inventory.length > 0 ? (

                        inventory.map(item => (

                            <tr key={item.id}>

                                <td>{item.name}</td>

                                <td>{item.categoryName}</td>

                                <td className="text-center">
                                    {item.stockQuantity}
                                </td>

                                <td className="text-center">
                                    {getBadge(item.stockQuantity)}
                                </td>

                            </tr>

                        ))

                    ) : (

                        <tr>

                            <td
                                colSpan={4}
                                className="text-center"
                            >
                                No inventory found.
                            </td>

                        </tr>

                    )}

                </tbody>

            </table>

        </div>

    );
}

export default InventoryTable;