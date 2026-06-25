import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import Navbar from "./components/Navbar";

import Marketplace from "./pages/Marketplace";
import PropertyDetail from "./pages/PropertyDetail";
import Auth from "./pages/Auth";

import TenantDashboard from "./pages/TenantDashboard";
import LandlordDashboard from "./pages/LandlordDashboard";

import KycVerification from "./pages/KycVerification";

import Wishlist from "./pages/Wishlist";
import Notifications from "./pages/Notifications";

import { AuthProvider, useAuth } from "./context/AuthContext";

const ProtectedRoute = ({ children, roles }) => {

    const { user } = useAuth();

    if (!user)
        return <Navigate to="/auth" replace />;

    if (roles && !roles.includes(user.role))
        return <Navigate to="/properties" replace />;

    return children;
};

export default function App() {

    return (

        <AuthProvider>

            <BrowserRouter>

                <div className="min-h-screen bg-[#F7FAFC]">

                    <Navbar />

                    <main className="max-w-7xl mx-auto px-4 py-8">

                        <Routes>

                            <Route
                                path="/"
                                element={<Navigate to="/properties" />}
                            />

                            <Route
                                path="/properties"
                                element={<Marketplace />}
                            />

                            <Route
                                path="/properties/:id"
                                element={<PropertyDetail />}
                            />

                            <Route
                                path="/auth"
                                element={<Auth />}
                            />

                            <Route
                                path="/wishlist"
                                element={
                                    <ProtectedRoute>
                                        <Wishlist />
                                    </ProtectedRoute>
                                }
                            />

                            <Route
                                path="/notifications"
                                element={
                                    <ProtectedRoute>
                                        <Notifications />
                                    </ProtectedRoute>
                                }
                            />

                            <Route
                                path="/dashboard/tenant"
                                element={
                                    <ProtectedRoute roles={["Guest"]}>
                                        <TenantDashboard />
                                    </ProtectedRoute>
                                }
                            />

                            <Route
                                path="/dashboard/landlord"
                                element={
                                    <ProtectedRoute roles={["Owner"]}>
                                        <LandlordDashboard />
                                    </ProtectedRoute>
                                }
                            />

                            <Route
                                path="/dashboard/kyc"
                                element={
                                    <ProtectedRoute>
                                        <KycVerification />
                                    </ProtectedRoute>
                                }
                            />

                        </Routes>

                    </main>

                </div>

            </BrowserRouter>

        </AuthProvider>

    );
}