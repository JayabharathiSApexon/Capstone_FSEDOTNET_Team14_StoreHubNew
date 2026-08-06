interface QuantitySelectorProps {

    quantity: number;

    onQuantityChange: (quantity: number) => void;

}

function QuantitySelector({

    quantity,

    onQuantityChange

}: QuantitySelectorProps) {

    const decreaseQuantity = () => {

        if (quantity > 1) {

            onQuantityChange(quantity - 1);

        }

    };

    const increaseQuantity = () => {

        onQuantityChange(quantity + 1);

    };

    return (

        <div
            className="d-flex align-items-center justify-content-center"
        >

            <button
                type="button"
                className="btn btn-outline-secondary btn-sm"
                onClick={decreaseQuantity}
            >
                -
            </button>

            <span
                className="mx-3 fw-semibold"
                style={{
                    minWidth: "25px",
                    textAlign: "center"
                }}
            >
                {quantity}
            </span>

            <button
                type="button"
                className="btn btn-outline-secondary btn-sm"
                onClick={increaseQuantity}
            >
                +
            </button>

        </div>

    );

}

export default QuantitySelector;