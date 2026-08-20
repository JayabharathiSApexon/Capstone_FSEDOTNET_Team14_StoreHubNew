import { FormEvent, useMemo, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { IoBagHandle } from "react-icons/io5";
import {
    forgotPassword,
    login,
    register,
    saveSessionLoginPreference
} from "../../services/authService";
import { getApiErrorMessage } from "../../utils/apiError";
import "./AuthPage.css";

type AuthMode = "login" | "register" | "forgot-password";

interface AuthPageProps {
    mode: AuthMode;
}

function AuthPage({ mode }: AuthPageProps) {
    const navigate = useNavigate();

    const isLogin = mode === "login";
    const isForgotPassword = mode === "forgot-password";

    const [fullName, setFullName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [phoneNumber, setPhoneNumber] = useState("");
    const [rememberMe, setRememberMe] = useState(false);
    const [error, setError] = useState("");
    const [info, setInfo] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);

    const heading = useMemo(() => {
        if (isForgotPassword) {
            return "Forgot Password";
        }

        return isLogin ? "Welcome Back!" : "Create Account";
    }, [isLogin, isForgotPassword]);

    const subHeading = useMemo(() => {
        if (isForgotPassword) {
            return "Set a new password for your account";
        }

        return isLogin
            ? "Sign in to continue to your account"
            : "Sign up to get started";
    }, [isLogin, isForgotPassword]);

    const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        setError("");
        setInfo("");

        if (isForgotPassword) {
            if (password !== confirmPassword) {
                setError("New password and confirm password do not match.");
                return;
            }

            try {
                setIsSubmitting(true);

                const response = await forgotPassword({
                    email,
                    newPassword: password,
                    confirmPassword: confirmPassword
                });

                setInfo(response.message);

                setTimeout(() => {
                    navigate("/login");
                }, 1200);
            }
            catch (requestError: any) {
                setError(getApiErrorMessage(requestError, "Unable to reset password."));
            }
            finally {
                setIsSubmitting(false);
            }

            return;
        }

        if (!isLogin) {
            if (!fullName.trim()) {
                setError("Full name is required.");
                return;
            }

            if (password !== confirmPassword) {
                setError("Passwords do not match.");
                return;
            }
        }

        try {
            setIsSubmitting(true);

            const authResponse = isLogin
                ? await login({ email, password })
                : await register({
                    fullName,
                    email,
                    password,
                    phoneNumber
                });

            if (!rememberMe && isLogin) {
                saveSessionLoginPreference();
            }

            navigate(authResponse.isAdmin ? "/admin/dashboard" : "/customer");
        }
        catch (requestError: any) {
            setError(getApiErrorMessage(requestError, "Authentication failed. Please try again."));
        }
        finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="auth-root">
            <div className="auth-shell container-fluid py-4">
                <div className="auth-grid row g-4 justify-content-center">
                    <div className="col-12 col-lg-6">
                        <section className="auth-card">
                            <header className="auth-card-header">
                                <h2>
                                    {isLogin ? "Login" : (isForgotPassword ? "Forgot Password" : "Register")}
                                </h2>
                            </header>

                            <div className="auth-card-body">
                                <div className="auth-form-pane">
                                    <div className="auth-brand">
                                        <IoBagHandle />
                                        <span>StoreHub</span>
                                    </div>

                                    <h3>{heading}</h3>
                                    <p>{subHeading}</p>

                                    {error && (
                                        <div className="alert alert-danger py-2 mb-3">
                                            {error}
                                        </div>
                                    )}

                                    {info && (
                                        <div className="alert alert-success py-2 mb-3">
                                            {info}
                                        </div>
                                    )}

                                    <form onSubmit={handleSubmit} className="auth-form">
                                        {!isLogin && !isForgotPassword && (
                                            <div className="mb-3">
                                                <label className="form-label">Full Name</label>
                                                <input
                                                    type="text"
                                                    className="form-control"
                                                    placeholder="Enter your full name"
                                                    value={fullName}
                                                    onChange={event => setFullName(event.target.value)}
                                                    required
                                                />
                                            </div>
                                        )}

                                        <div className="mb-3">
                                            <label className="form-label">
                                                {isLogin ? "Username or Email" : "Email"}
                                            </label>
                                            <input
                                                type={isLogin ? "text" : "email"}
                                                className="form-control"
                                                placeholder={isLogin ? "Enter your username or email" : "Enter your email"}
                                                value={email}
                                                onChange={event => setEmail(event.target.value)}
                                                required
                                            />
                                        </div>

                                        {!isLogin && !isForgotPassword && (
                                            <div className="mb-3">
                                                <label className="form-label">Phone Number</label>
                                                <input
                                                    type="text"
                                                    className="form-control"
                                                    placeholder="Optional"
                                                    value={phoneNumber}
                                                    onChange={event => setPhoneNumber(event.target.value)}
                                                />
                                            </div>
                                        )}

                                        <div className="mb-3">
                                            <label className="form-label">
                                                {isForgotPassword ? "New Password" : "Password"}
                                            </label>
                                            <input
                                                type="password"
                                                className="form-control"
                                                placeholder={isForgotPassword ? "Enter your new password" : "Enter your password"}
                                                value={password}
                                                onChange={event => setPassword(event.target.value)}
                                                minLength={6}
                                                required
                                            />
                                        </div>

                                        {!isLogin && (
                                            <div className="mb-3">
                                                <label className="form-label">
                                                    {isForgotPassword ? "Confirm New Password" : "Confirm Password"}
                                                </label>
                                                <input
                                                    type="password"
                                                    className="form-control"
                                                    placeholder={isForgotPassword ? "Confirm your new password" : "Confirm your password"}
                                                    value={confirmPassword}
                                                    onChange={event => setConfirmPassword(event.target.value)}
                                                    minLength={6}
                                                    required
                                                />
                                            </div>
                                        )}

                                        {isLogin && (
                                            <div className="auth-meta-row mb-3">
                                                <label className="form-check-label d-flex align-items-center gap-2">
                                                    <input
                                                        type="checkbox"
                                                        className="form-check-input"
                                                        checked={rememberMe}
                                                        onChange={event => setRememberMe(event.target.checked)}
                                                    />
                                                    Remember me
                                                </label>

                                                <Link to="/forgot-password" className="auth-link">
                                                    Forgot Password?
                                                </Link>
                                            </div>
                                        )}

                                        <button
                                            type="submit"
                                            className="btn auth-submit-btn"
                                            disabled={isSubmitting}
                                        >
                                            {isSubmitting
                                                ? "Please wait..."
                                                : (isLogin ? "Login" : (isForgotPassword ? "Reset Password" : "Register"))}
                                        </button>
                                    </form>

                                    <div className="auth-switch-text mt-4">
                                        {isForgotPassword ? "Back to " : (isLogin ? "Don't have an account? " : "Already have an account? ")}
                                        <Link
                                            to={isForgotPassword ? "/login" : (isLogin ? "/register" : "/login")}
                                            className="auth-link fw-semibold"
                                        >
                                            {isForgotPassword ? "Login" : (isLogin ? "Register" : "Login")}
                                        </Link>
                                    </div>
                                </div>

                                <div className="auth-illustration-pane">
                                    <div className="auth-orb" />
                                    <div className="auth-figure">
                                        <div className="auth-figure-head" />
                                        <div className="auth-figure-body" />
                                        <div className="auth-figure-leg-left" />
                                        <div className="auth-figure-leg-right" />
                                    </div>
                                </div>
                            </div>
                        </section>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default AuthPage;