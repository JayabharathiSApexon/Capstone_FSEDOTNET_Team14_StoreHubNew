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

    if (totalPages <= 1) {
        return null;
    }

    const maxVisiblePages = 5;

    let startPage = Math.max(
        1,
        currentPage - Math.floor(maxVisiblePages / 2)
    );

    let endPage = startPage + maxVisiblePages - 1;

    if (endPage > totalPages) {

        endPage = totalPages;

        startPage = Math.max(
            1,
            endPage - maxVisiblePages + 1
        );
    }

    return (

        <div className="d-flex justify-content-between align-items-center mt-4">

            <small className="text-muted">

                Showing{" "}

                {(currentPage - 1) * itemsPerPage + 1}

                {" - "}

                {Math.min(
                    currentPage * itemsPerPage,
                    totalItems
                )}

                {" of "}

                {totalItems}

            </small>

            <nav>

                <ul className="pagination pagination-sm mb-0">

                    <li
                        className={`page-item ${currentPage === 1
                                ? "disabled"
                                : ""
                            }`}
                    >

                        <button
                            className="page-link"
                            onClick={() =>
                                onPageChange(currentPage - 1)
                            }
                        >
                            Previous
                        </button>

                    </li>

                    {Array.from(
                        {
                            length:
                                endPage - startPage + 1
                        },
                        (_, index) => {

                            const page =
                                startPage + index;

                            return (

                                <li
                                    key={page}
                                    className={`page-item ${currentPage === page
                                            ? "active"
                                            : ""
                                        }`}
                                >

                                    <button
                                        className="page-link"
                                        onClick={() =>
                                            onPageChange(page)
                                        }
                                    >
                                        {page}
                                    </button>

                                </li>

                            );

                        }
                    )}

                    <li
                        className={`page-item ${currentPage === totalPages
                                ? "disabled"
                                : ""
                            }`}
                    >

                        <button
                            className="page-link"
                            onClick={() =>
                                onPageChange(currentPage + 1)
                            }
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