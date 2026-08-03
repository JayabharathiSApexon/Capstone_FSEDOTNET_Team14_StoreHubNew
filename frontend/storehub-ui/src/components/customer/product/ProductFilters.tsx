import { CategoryResponse } from "../../../models/category/CategoryResponse";

interface ProductFiltersProps {

    categories: CategoryResponse[];

    selectedCategory: string;

    minPrice: string;

    maxPrice: string;

    sortBy: string;

    onCategoryChange: (value: string) => void;

    onMinPriceChange: (value: string) => void;

    onMaxPriceChange: (value: string) => void;

    onSortChange: (value: string) => void;
}

function ProductFilters({

    categories,

    selectedCategory,

    minPrice,

    maxPrice,

    sortBy,

    onCategoryChange,

    onMinPriceChange,

    onMaxPriceChange,

    onSortChange

}: ProductFiltersProps) {

    return (

        <div className="d-flex align-items-center gap-3 flex-wrap mb-4">

            <select
                className="form-select"
                style={{ width: "220px" }}
                value={selectedCategory}
                onChange={(e) => onCategoryChange(e.target.value)}
            >
                <option value="">All Categories</option>

                {categories.map(category => (

                    <option
                        key={category.id}
                        value={category.id}
                    >
                        {category.name}
                    </option>

                ))}

            </select>

            <input
                type="number"
                className="form-control"
                placeholder="₹ Min"
                style={{ width: "120px" }}
                value={minPrice}
                onChange={(e) => onMinPriceChange(e.target.value)}
            />

            <input
                type="number"
                className="form-control"
                placeholder="₹ Max"
                style={{ width: "120px" }}
                value={maxPrice}
                onChange={(e) => onMaxPriceChange(e.target.value)}
            />

            <select
                className="form-select"
                style={{ width: "180px" }}
                value={sortBy}
                onChange={(e) => onSortChange(e.target.value)}
            >
                <option value="">Sort By</option>
                <option value="priceLow">Price: Low to High</option>
                <option value="priceHigh">Price: High to Low</option>
                <option value="name">Name</option>
            </select>

        </div>

    );
}

export default ProductFilters;