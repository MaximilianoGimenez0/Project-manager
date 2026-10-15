import { lazy, Suspense } from "react";
import { Routes, Route, Navigate } from "react-router-dom";

import { MainLayout } from "@/layouts";
import ProtectedRoute from "./ProtectedRoute";

// Route-level code splitting
const Home = lazy(() => import("@/pages/Home"));
const About = lazy(() => import("@/pages/About"));
const Login = lazy(() => import("@/pages/Login"));
const CreateProject = lazy(() => import("@/pages/CreateProject"));
const PendingProjectsPage = lazy(() => import("@/pages/PendingProjectsPage"));
const ProjectDetailPage = lazy(() => import("@/pages/ProjectDetailPage"));
const ProjectsSearchPage = lazy(() => import("@/pages/ProjectsSearchPage"));
const NotFound = lazy(() => import("@/pages/NotFound"));

function PageLoader() {
  return (
    <div
      style={{
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        flex: 1,
        minHeight: "60vh",
        width: "100%",
      }}
      role="status"
      aria-live="polite"
    >
      <div
        style={{
          width: "40px",
          height: "40px",
          border: "3px solid var(--border-color)",
          borderTopColor: "var(--primary-color)",
          borderRadius: "50%",
          animation: "spin 0.8s linear infinite",
        }}
        aria-hidden="true"
      />
    </div>
  );
}

export default function AppRouter() {
  return (
    <Suspense fallback={<PageLoader />}>
      <Routes>
        <Route path="/" element={<MainLayout />}>
          {/* Public Routes */}
          <Route index element={<Home />} />
          <Route path="about" element={<About />} />
          <Route path="login" element={<Login />} />

          {/* Protected Routes requiring authentication */}
          <Route element={<ProtectedRoute />}>
            <Route path="create-project" element={<CreateProject />} />
            <Route
              path="createProject"
              element={<Navigate to="/create-project" replace />}
            />
            <Route path="pending-projects" element={<PendingProjectsPage />} />
            <Route path="search" element={<ProjectsSearchPage />} />
            <Route path="project/:id" element={<ProjectDetailPage />} />
            <Route path="projects/:id" element={<ProjectDetailPage />} />
          </Route>

          {/* Catch-all Route */}
          <Route path="*" element={<NotFound />} />
        </Route>
      </Routes>
    </Suspense>
  );
}
