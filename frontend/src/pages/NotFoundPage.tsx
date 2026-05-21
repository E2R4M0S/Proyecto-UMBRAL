import { Link } from 'react-router-dom';

export function NotFoundPage() {
  return (
    <div
      style={{
        minHeight: '100vh',
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
        fontFamily: 'system-ui, Segoe UI, Roboto, sans-serif',
        color: '#6b6375',
        textAlign: 'center',
        padding: '20px',
      }}
    >
      <h1
        style={{
          fontSize: '72px',
          fontWeight: 500,
          color: '#08060d',
          margin: '0',
          lineHeight: 1,
        }}
      >
        404
      </h1>
      <p style={{ fontSize: '18px', margin: '16px 0 32px' }}>
        Página no encontrada
      </p>
      <Link
        to="/"
        style={{
          color: '#aa3bff',
          textDecoration: 'none',
          fontSize: '16px',
          fontWeight: 500,
        }}
      >
        Volver al inicio
      </Link>
    </div>
  );
}
