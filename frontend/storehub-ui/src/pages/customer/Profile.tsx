import { useEffect, useState } from "react";
import {
    FaUserCircle,
    FaEdit,
    FaSave,
    FaTimes
} from "react-icons/fa";

import CustomerLayout from "../../components/customer/CustomerLayout";

import {
    getProfile,
    updateProfile
} from "../../services/profile/profileService";

import type { ProfileResponse } from "../../models/profile/ProfileResponse";
import type { UpdateProfileRequest } from "../../models/profile/UpdateProfileRequest";

function Profile() {

    const [profile, setProfile] = useState<ProfileResponse | null>(null);

    const [formData, setFormData] = useState<UpdateProfileRequest>({
        fullName: "",
        email: "",
        phoneNumber: ""
    });

    const [isEditing, setIsEditing] = useState(false);

    const [loading, setLoading] = useState(true);

    const [saving, setSaving] = useState(false);

    const [message, setMessage] = useState("");

    const [error, setError] = useState("");


    // Load profile when the page is opened
    useEffect(() => {

        const loadProfile = async () => {

            try {

                setLoading(true);
                setError("");

                const data = await getProfile();

                setProfile(data);

                setFormData({
                    fullName: data.fullName,
                    email: data.email,
                    phoneNumber: data.phoneNumber
                });

            } catch (err) {

                console.error(
                    "Failed to load profile:",
                    err
                );

                setError(
                    "Unable to load profile."
                );

            } finally {

                setLoading(false);

            }

        };

        loadProfile();

    }, []);


    // Automatically hide success message after 3 seconds
    useEffect(() => {

        if (!message) {
            return;
        }

        const timer = setTimeout(() => {

            setMessage("");

        }, 3000);

        return () => {

            clearTimeout(timer);

        };

    }, [message]);


    // Handle input changes
    const handleChange = (
        e: React.ChangeEvent<HTMLInputElement>
    ) => {

        const {
            name,
            value
        } = e.target;

        setFormData(previous => ({
            ...previous,
            [name]: value
        }));

    };


    // Enable editing
    const handleEdit = () => {

        setMessage("");

        setError("");

        setIsEditing(true);

    };


    // Cancel editing
    const handleCancel = () => {

        if (!profile) {
            return;
        }

        setFormData({
            fullName: profile.fullName,
            email: profile.email,
            phoneNumber: profile.phoneNumber
        });

        setMessage("");

        setError("");

        setIsEditing(false);

    };


    // Save profile
    const handleSave = async () => {

        if (!formData.fullName.trim()) {

            setError(
                "Full name is required."
            );

            return;

        }


        if (!formData.email.trim()) {

            setError(
                "Email is required."
            );

            return;

        }


        try {

            setSaving(true);

            setMessage("");

            setError("");


            await updateProfile({
                fullName: formData.fullName.trim(),
                email: formData.email.trim(),
                phoneNumber: formData.phoneNumber.trim()
            });


            // Get the latest data from the API
            const updatedProfile =
                await getProfile();


            setProfile(updatedProfile);


            setFormData({
                fullName: updatedProfile.fullName,
                email: updatedProfile.email,
                phoneNumber: updatedProfile.phoneNumber
            });


            setIsEditing(false);


            setMessage(
                "Profile updated successfully."
            );

        } catch (err) {

            console.error(
                "Failed to update profile:",
                err
            );

            setError(
                "Unable to update profile."
            );

        } finally {

            setSaving(false);

        }

    };


    return (

        <CustomerLayout showHeader={false}>

            {() => (

                <div className="container-fluid py-4">

                    {/* Page Header */}

                    <div className="d-flex justify-content-between align-items-center mb-4">

                        <div>

                            <h3 className="fw-bold mb-1">
                                My Profile
                            </h3>

                            <p className="text-muted mb-0">
                                Manage your personal information
                            </p>

                        </div>


                        {!isEditing && (

                            <button
                                type="button"
                                className="btn btn-primary d-flex align-items-center"
                                onClick={handleEdit}
                            >

                                <FaEdit className="me-2" />

                                Edit Profile

                            </button>

                        )}

                    </div>


                    {/* Loading */}

                    {loading && (

                        <div className="card shadow-sm border-0">

                            <div className="card-body text-center py-5">

                                <div
                                    className="spinner-border text-primary"
                                    role="status"
                                >
                                    <span className="visually-hidden">
                                        Loading...
                                    </span>
                                </div>

                                <p className="text-muted mt-3 mb-0">
                                    Loading profile...
                                </p>

                            </div>

                        </div>

                    )}


                    {/* Error */}

                    {!loading && error && (

                        <div
                            className="alert alert-danger"
                            role="alert"
                        >

                            {error}

                        </div>

                    )}


                    {/* Profile */}

                    {!loading && profile && (

                        <div className="row">

                            {/* Personal Information */}

                            <div className="col-lg-8">

                                <div className="card shadow-sm border-0">

                                    {/* Card Header */}

                                    <div className="card-header bg-white py-3">

                                        <div className="d-flex align-items-center">

                                            <FaUserCircle
                                                size={45}
                                                className="text-primary me-3"
                                            />

                                            <div>

                                                <h5 className="mb-0 fw-semibold">
                                                    Personal Information
                                                </h5>

                                                <small className="text-muted">
                                                    Your account details
                                                </small>

                                            </div>

                                        </div>

                                    </div>


                                    {/* Card Body */}

                                    <div className="card-body p-4">


                                        {/* Success Message */}

                                        {message && (

                                            <div
                                                className="alert alert-success"
                                                role="alert"
                                            >

                                                {message}

                                            </div>

                                        )}


                                        {/* Full Name */}

                                        <div className="mb-3">

                                            <label
                                                htmlFor="fullName"
                                                className="form-label fw-semibold"
                                            >
                                                Full Name
                                            </label>

                                            <input
                                                id="fullName"
                                                name="fullName"
                                                type="text"
                                                className="form-control"
                                                value={formData.fullName}
                                                onChange={handleChange}
                                                disabled={!isEditing}
                                            />

                                        </div>


                                        {/* Email */}

                                        <div className="mb-3">

                                            <label
                                                htmlFor="email"
                                                className="form-label fw-semibold"
                                            >
                                                Email Address
                                            </label>

                                            <input
                                                id="email"
                                                name="email"
                                                type="email"
                                                className="form-control"
                                                value={formData.email}
                                                onChange={handleChange}
                                                disabled={!isEditing}
                                            />

                                        </div>


                                        {/* Phone Number */}

                                        <div className="mb-4">

                                            <label
                                                htmlFor="phoneNumber"
                                                className="form-label fw-semibold"
                                            >
                                                Phone Number
                                            </label>

                                            <input
                                                id="phoneNumber"
                                                name="phoneNumber"
                                                type="text"
                                                className="form-control"
                                                value={formData.phoneNumber}
                                                onChange={handleChange}
                                                disabled={!isEditing}
                                            />

                                        </div>


                                        {/* Save / Cancel */}

                                        {isEditing && (

                                            <div className="d-flex gap-2">

                                                {/* Save */}

                                                <button
                                                    type="button"
                                                    className="btn btn-primary d-flex align-items-center"
                                                    onClick={handleSave}
                                                    disabled={saving}
                                                >

                                                    {saving ? (

                                                        <>

                                                            <span
                                                                className="spinner-border spinner-border-sm me-2"
                                                                role="status"
                                                            />

                                                            Saving...

                                                        </>

                                                    ) : (

                                                        <>

                                                            <FaSave className="me-2" />

                                                            Save Changes

                                                        </>

                                                    )}

                                                </button>


                                                {/* Cancel */}

                                                <button
                                                    type="button"
                                                    className="btn btn-outline-secondary d-flex align-items-center"
                                                    onClick={handleCancel}
                                                    disabled={saving}
                                                >

                                                    <FaTimes className="me-2" />

                                                    Cancel

                                                </button>

                                            </div>

                                        )}

                                    </div>

                                </div>

                            </div>


                            {/* Account Information */}

                            <div className="col-lg-4 mt-4 mt-lg-0">

                                <div className="card shadow-sm border-0">

                                    <div className="card-header bg-white py-3">

                                        <h5 className="mb-0 fw-semibold">
                                            Account Information
                                        </h5>

                                    </div>


                                    <div className="card-body">

                                        {/* Account Type */}

                                        <div className="mb-3">

                                            <small className="text-muted d-block">
                                                Account Type
                                            </small>

                                            <span className="fw-semibold">

                                                {profile.isAdmin
                                                    ? "Administrator"
                                                    : "Customer"}

                                            </span>

                                        </div>


                                        {/* Member Since */}

                                        <div>

                                            <small className="text-muted d-block">
                                                Member Since
                                            </small>

                                            <span className="fw-semibold">

                                                {new Date(
                                                    profile.createdDate
                                                ).toLocaleDateString()}

                                            </span>

                                        </div>

                                    </div>

                                </div>

                            </div>

                        </div>

                    )}

                </div>

            )}

        </CustomerLayout>

    );

}

export default Profile;