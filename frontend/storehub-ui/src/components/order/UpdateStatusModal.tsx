import React, { useEffect, useState } from "react";
import OrderResponse from "../../models/order/OrderResponse";

interface UpdateStatusModalProps {
    show: boolean;
    order: OrderResponse | null;
    onClose: () => void;
    onSave: (status: string) => void;
}

const UpdateStatusModal: React.FC<UpdateStatusModalProps> = ({
    show,
    order,
    onClose,
    onSave
}) => {

    const [status, setStatus] = useState("");

    useEffect(() => {
        if (order) {
            setStatus(order.status);
        }
    }, [order]);

    if (!show || !order) {
        return null;
    }

    return (
        <div
            className="modal fade show"
            style={{ display: "block", backgroundColor: "rgba(0,0,0,.5)" }}
        >
            <div className="modal-dialog modal-dialog-centered">
                <div className="modal-content">

                    <div className="modal-header">
                        <h5 className="modal-title">
                            Update Order Status
                        </h5>

                        <button
                            type="button"
                            className="btn-close"
                            onClick={onClose}
                        />
                    </div>

                    <div className="modal-body">

                        <div className="mb-3">

                            <label className="form-label">
                                Status
                            </label>

                            <select
                                className="form-select"
                                value={status}
                                onChange={(e) => setStatus(e.target.value)}
                            >
                                <option value="Pending">Pending</option>
                                <option value="Processing">Processing</option>
                                <option value="Shipped">Shipped</option>
                                <option value="Delivered">Delivered</option>
                                <option value="Cancelled">Cancelled</option>
                            </select>

                        </div>

                    </div>

                    <div className="modal-footer">

                        <button
                            className="btn btn-secondary"
                            onClick={onClose}
                        >
                            Cancel
                        </button>

                        <button
                            className="btn btn-primary"
                            onClick={() => onSave(status)}
                        >
                            Save
                        </button>

                    </div>

                </div>
            </div>
        </div>
    );
};

export default UpdateStatusModal;