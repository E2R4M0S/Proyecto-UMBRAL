import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { CreateOperatorPage } from './pages/admin/CreateOperatorPage';
import { CreateMissionPage } from './pages/admin/CreateMissionPage';
import { AdminDashboard } from './pages/admin/AdminDashboard';
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
            path="/operator/*"
            element={
              <ProtectedRoute allowedRoles={['Operator']}>
                <div style={{
                  minHeight: '100vh',
                  display: 'flex',
                  justifyContent: 'center',
                  alignItems: 'center',
                  fontFamily: 'system-ui, Segoe UI, Roboto, sans-serif',
                  color: '#6b6375',
                  fontSize: '24px',
                  background: '#f4f3ec',
                }}>
                  Panel de Operador
                </div>
              </ProtectedRoute>
            }
          />
          <Route
            path="/participant/*"
            element={
              <ProtectedRoute allowedRoles={['Participant']}>
                <div style={{
                  minHeight: '100vh',
                  display: 'flex',
                  justifyContent: 'center',
                  alignItems: 'center',
                  fontFamily: 'system-ui, Segoe UI, Roboto, sans-serif',
                  color: '#6b6375',
                  fontSize: '24px',
                  background: '#f4f3ec',
                }}>
                  Panel de Participante
                </div>
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
