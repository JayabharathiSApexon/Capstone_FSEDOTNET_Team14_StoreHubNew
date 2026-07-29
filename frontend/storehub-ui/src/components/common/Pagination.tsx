interface PaginationProps {
    currentPage: number;
    totalItems: number;
    itemsPerPage: number;
    onPageChange: (page: number) => void;
}

function Pagination({
    currentPage,
    totalItems,
    itemsPerPage,
    onPageChange
}: PaginationProps) {

    const totalPages = Math.ceil(totalItems / itemsPerPage);

    if (totalPages <= 1) return null;

    return (
        <div className="d-flex justify-content-between align-items-center mt-3">

            <small className="text-muted">
                Showing{" "}
                {(currentPage - 1) * itemsPerPage + 1}
                {" - "}
                {Math.min(currentPage * itemsPerPage, totalItems)}
                {" of "}
                {totalItems}
            </small>

            <nav>
                <ul className="pagination pagination-sm mb-0">

                    <li
                        className={`page-item ${currentPage === 1 ? "disabled" : ""}`}
                    >
                        <button
                            className="page-link"
                            onClick={() => onPageChange(currentPage - 1)}
                        >
                            Previous
                        </button>
                    </li>

                    {Array.from({ length: totalPages }, (_, index) => (

                        <li
                            key={index}
                            className={`page-item ${currentPage === index + 1 ? "active" : ""}`}
                        >
                            <button
                                className="page-link"
                                onClick={() => onPageChange(index + 1)}
                            >
                                {index + 1}
                            </button>
                        </li>

                    ))}

                    <li
                        className={`page-item ${currentPage === totalPages ? "disabled" : ""}`}
                    >
                        <button
                            className="page-link"
                            onClick={() => onPageChange(currentPage + 1)}
                        >
                            Next
                        </button>
                    </li>

                </ul>
            </nav>

        </div>
    );
}

export default Pagination;