interface StatusBadgeProps {
    status: string;
}

function StatusBadge({
    status
}: StatusBadgeProps) {

    let badgeClass = "badge bg-secondary";

    switch (status) {

        case "Pending":
            badgeClass = "badge bg-warning text-dark";
            break;

        case "Processing":
            badgeClass = "badge bg-info text-dark";
            break;

        case "Shipped":
            badgeClass = "badge bg-primary";
            break;

        case "Delivered":
            badgeClass = "badge bg-success";
            break;

        case "Cancelled":
            badgeClass = "badge bg-danger";
            break;

    }

    return (
        <span className={badgeClass}>
            {status}
        </span>
    );
}

export default StatusBadge;