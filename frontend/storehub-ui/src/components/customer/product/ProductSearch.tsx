interface ProductSearchProps {
    searchTerm: string;
    onSearchChange: (value: string) => void;
}

function ProductSearch({
    searchTerm,
    onSearchChange
}: ProductSearchProps) {

    return (

        <input
            type="text"
            className="form-control"
            placeholder="Search products..."
            value={searchTerm}
            onChange={(e) => onSearchChange(e.target.value)}
        />

    );
}

export default ProductSearch;