import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { CreateOperatorPage } from './pages/admin/CreateOperatorPage';
import { CreateMissionPage } from './pages/admin/CreateMissionPage';
import { AdminDashboard } from './pages/admin/AdminDashboard';
import { MissionCatalogPage } from './pages/catalog/MissionCatalogPage';
import { MissionDetailPage } from './pages/catalog/MissionDetailPage';
import { OperatorDashboard } from './pages/operator/OperatorDashboard';
import { ParticipantDashboard } from './pages/participant/ParticipantDashboard';
import { NotFoundPage } from './pages/NotFoundPage';

function ProtectedLayout({
  allowedRoles,
  children,
}: {
  allowedRoles: string[];
  children: React.ReactNode;
}) {
  return (
    <ProtectedRoute allowedRoles={allowedRoles}>
      {children}
    </ProtectedRoute>
  );
}

export function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route
            path="/admin/operators/create"
            element={
              <ProtectedRoute allowedRoles={['Admin']}>
                <CreateOperatorPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin/dashboard"
            element={
              <ProtectedLayout allowedRoles={['Admin']}>
                <AdminDashboard />
              </ProtectedLayout>
            }
          />
          <Route
            path="/admin/missions/create"
            element={
              <ProtectedRoute allowedRoles={['Admin']}>
                <CreateMissionPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin/*"
            element={
              <ProtectedLayout allowedRoles={['Admin']}>
                <AdminDashboard />
              </ProtectedLayout>
            }
          />
          <Route
            path="/missions"
            element={
              <ProtectedRoute allowedRoles={['Admin', 'Operator', 'Participant']}>
                <MissionCatalogPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/missions/:id"
            element={
              <ProtectedRoute allowedRoles={['Admin', 'Operator', 'Participant']}>
                <MissionDetailPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/operator/dashboard"
            element={
              <ProtectedRoute allowedRoles={['Operator']}>
                <OperatorDashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/operator/*"
            element={
              <ProtectedRoute allowedRoles={['Operator']}>
                <OperatorDashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/participant/dashboard"
            element={
              <ProtectedRoute allowedRoles={['Participant']}>
                <ParticipantDashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/participant/*"
            element={
              <ProtectedRoute allowedRoles={['Participant']}>
                <ParticipantDashboard />
              </ProtectedRoute>
            }
          />
          <Route path="/" element={<Navigate to="/login" replace />} />
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
