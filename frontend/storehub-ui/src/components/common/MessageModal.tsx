import { Modal, Button } from "react-bootstrap";

interface MessageModalProps {
    show: boolean;
    title: string;
    message: string;
    variant?: "success" | "danger" | "warning" | "info";
    onClose: () => void;
}

function MessageModal({
    show,
    title,
    message,
    variant = "success",
    onClose
}: MessageModalProps) {

    return (
        <Modal
            show={show}
            onHide={onClose}
            centered
            backdrop="static"
        >
            <Modal.Header closeButton>
                <Modal.Title className={`text-${variant}`}>
                    {title}
                </Modal.Title>
            </Modal.Header>

            <Modal.Body>
                <div className="text-center">

                    <h5 className={`text-${variant} mb-3`}>
                        {variant === "success" ? "✔ Success" : "✖ Error"}
                    </h5>

                    <p>{message}</p>

                </div>
            </Modal.Body>

            <Modal.Footer>
                <Button
                    variant={variant}
                    onClick={onClose}
                >
                    OK
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export default MessageModal;