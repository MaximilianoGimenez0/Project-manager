import { Navigate, Outlet, useLocation } from "react-router-dom";

import { useCurrentUser } from "@/hooks";

export default function ProtectedRoute() {
    const { isAuthenticated } = useCurrentUser();
    const location = useLocation();

    if (!isAuthenticated) {
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    return <Outlet />;
}
