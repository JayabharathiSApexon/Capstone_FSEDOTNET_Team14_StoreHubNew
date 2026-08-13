import { Modal, Button } from "react-bootstrap";

interface ConfirmModalProps {
    show: boolean;
    title: string;
    message: string;
    confirmText?: string;
    onConfirm: () => void;
    onCancel: () => void;
}

function ConfirmModal({
    show,
    title,
    message,
    confirmText = "Confirm",
    onConfirm,
    onCancel
}: ConfirmModalProps) {

    return (
        <Modal
            show={show}
            onHide={onCancel}
            centered
            backdrop="static"
        >
            <Modal.Header closeButton>
                <Modal.Title>
                    {title}
                </Modal.Title>
            </Modal.Header>

            <Modal.Body>
                {message}
            </Modal.Body>

            <Modal.Footer>

                <Button
                    variant="secondary"
                    onClick={onCancel}
                >
                    No
                </Button>

                <Button
                    variant="danger"
                    onClick={onConfirm}
                >
                    {confirmText}
                </Button>

            </Modal.Footer>
        </Modal>
    );
}

export default ConfirmModal;