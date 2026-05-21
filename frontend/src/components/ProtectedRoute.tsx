import { Navigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import type { ReactNode } from 'react';

interface ProtectedRouteProps {
  allowedRoles: string[];
  children: ReactNode;
}

export function ProtectedRoute({ allowedRoles, children }: ProtectedRouteProps) {
  const { isAuthenticated, user, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        height: '100vh',
        color: '#6b6375',
        fontFamily: 'system-ui, sans-serif',
        fontSize: '18px',
      }}>
        Cargando...
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (user && !allowedRoles.includes(user.role)) {
    // Redirect to the user's own dashboard based on role
    const dashboards: Record<string, string> = {
      Admin: '/admin/dashboard',
      Operator: '/operator/dashboard',
      Participant: '/participant/dashboard',
    };
    const target = dashboards[user.role] ?? '/login';
    return <Navigate to={target} replace />;
  }

  return <>{children}</>;
}
