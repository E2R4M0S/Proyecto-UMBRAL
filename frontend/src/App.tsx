import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { CreateOperatorPage } from './pages/admin/CreateOperatorPage';
import { NotFoundPage } from './pages/NotFoundPage';
import type { ReactNode } from 'react';

function DashboardPlaceholder({ title }: { title: string }) {
  return (
    <div
      style={{
        minHeight: '100vh',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        fontFamily: 'system-ui, Segoe UI, Roboto, sans-serif',
        color: '#6b6375',
        fontSize: '24px',
      }}
    >
      {title}
    </div>
  );
}

function ProtectedLayout({
  allowedRoles,
  title,
}: {
  allowedRoles: string[];
  title: string;
}): ReactNode {
  return (
    <ProtectedRoute allowedRoles={allowedRoles}>
      <DashboardPlaceholder title={title} />
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
            path="/admin/*"
            element={
              <ProtectedLayout
                allowedRoles={['Admin']}
                title="Panel de Administración"
              />
            }
          />
          <Route
            path="/operator/*"
            element={
              <ProtectedLayout
                allowedRoles={['Operator']}
                title="Panel de Operador"
              />
            }
          />
          <Route
            path="/participant/*"
            element={
              <ProtectedLayout
                allowedRoles={['Participant']}
                title="Panel de Participante"
              />
            }
          />
          <Route path="/" element={<Navigate to="/login" replace />} />
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
