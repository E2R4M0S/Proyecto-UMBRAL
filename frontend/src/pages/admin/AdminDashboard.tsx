import { useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../../contexts/AuthContext';

const cardStyle: Record<string, string> = {
  background: '#fff',
  borderRadius: '12px',
  padding: '24px',
  cursor: 'pointer',
  border: '2px solid #e5e4e7',
  transition: 'border-color 0.2s, transform 0.2s',
  textDecoration: 'none',
  color: '#08060d',
  display: 'flex',
  flexDirection: 'column',
  gap: '8px',
};

export function AdminDashboard() {
  const navigate = useNavigate();
  const auth = useContext(AuthContext);

  const modules = [
    {
      title: 'Crear Operador',
      description: 'Registrar una nueva cuenta de operador en el sistema',
      path: '/admin/operators/create',
      icon: '👤',
    },
    {
      title: 'Crear Misión',
      description: 'Crear una nueva misión con narrativa y parámetros',
      path: '/admin/missions/create',
      icon: '📋',
    },
  ];

  return (
    <div
      style={{
        minHeight: '100vh',
        background: '#f4f3ec',
        fontFamily: 'system-ui, Segoe UI, Roboto, sans-serif',
      }}
    >
      {/* Header */}
      <header
        style={{
          background: '#fff',
          borderBottom: '1px solid #e5e4e7',
          padding: '16px 32px',
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
        }}
      >
        <h1
          style={{
            fontSize: '20px',
            fontWeight: 600,
            color: '#08060d',
            margin: 0,
          }}
        >
          Panel de Administración
        </h1>
        <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
          <span style={{ fontSize: '14px', color: '#6b6375' }}>
            {auth?.user?.name} ({auth?.user?.role})
          </span>
          <button
            onClick={() => {
              auth?.logout();
              navigate('/login', { replace: true });
            }}
            style={{
              padding: '8px 16px',
              fontSize: '14px',
              color: '#dc2626',
              background: '#fff',
              border: '1px solid #fecaca',
              borderRadius: '6px',
              cursor: 'pointer',
            }}
          >
            Cerrar sesión
          </button>
        </div>
      </header>

      {/* Content */}
      <main
        style={{
          maxWidth: '800px',
          margin: '0 auto',
          padding: '40px 24px',
        }}
      >
        <p
          style={{
            fontSize: '16px',
            color: '#6b6375',
            marginBottom: '32px',
          }}
        >
          Seleccioná una opción del panel para gestionar el sistema
        </p>

        <div
          style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))',
            gap: '16px',
          }}
        >
          {modules.map((mod) => (
            <div
              key={mod.path}
              style={cardStyle}
              onClick={() => navigate(mod.path)}
              onMouseEnter={(e) => {
                e.currentTarget.style.borderColor = '#aa3bff';
                e.currentTarget.style.transform = 'translateY(-2px)';
              }}
              onMouseLeave={(e) => {
                e.currentTarget.style.borderColor = '#e5e4e7';
                e.currentTarget.style.transform = 'translateY(0)';
              }}
            >
              <span style={{ fontSize: '28px' }}>{mod.icon}</span>
              <h3 style={{ margin: '8px 0 0', fontSize: '18px', fontWeight: 500 }}>
                {mod.title}
              </h3>
              <p
                style={{
                  margin: 0,
                  fontSize: '14px',
                  color: '#6b6375',
                  lineHeight: 1.4,
                }}
              >
                {mod.description}
              </p>
            </div>
          ))}
        </div>
      </main>
    </div>
  );
}
