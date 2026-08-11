import Layout from "../../../components/admin/AdminLayout";
import { useUserManagement } from "../../../hooks/admin/useUserManagement";

function UserManagement() {
    const { users, loading, error } = useUserManagement();

    return (
        <Layout>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="mb-0">User Management</h3>
                    <small className="text-muted">View all registered users and their roles</small>
                </div>
            </div>

            <div className="card shadow-sm">
                <div className="card-body">
                    {loading && <p>Loading users...</p>}

                    {!loading && error && (
                        <div className="alert alert-danger mb-0">{error}</div>
                    )}

                    {!loading && !error && (
                        <div className="table-responsive">
                            <table className="table table-hover align-middle mb-0">
                                <thead className="table-light">
                                    <tr>
                                        <th>Name</th>
                                        <th>Email</th>
                                        <th>Role</th>
                                        <th>Status</th>
                                        <th>Created</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {users.length === 0 && (
                                        <tr>
                                            <td colSpan={5} className="text-center py-4 text-muted">
                                                No users found.
                                            </td>
                                        </tr>
                                    )}

                                    {users.map(user => (
                                        <tr key={user.id}>
                                            <td>{user.fullName}</td>
                                            <td>{user.email}</td>
                                            <td>
                                                <span className={`badge ${user.isAdmin ? "bg-primary" : "bg-secondary"}`}>
                                                    {user.role}
                                                </span>
                                            </td>
                                            <td>
                                                <span className={`badge ${user.isActive ? "bg-success" : "bg-danger"}`}>
                                                    {user.isActive ? "Active" : "Inactive"}
                                                </span>
                                            </td>
                                            <td>{new Date(user.createdDate).toLocaleString()}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            </div>
        </Layout>
    );
}

export default UserManagement;