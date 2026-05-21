import { useState, type FormEvent } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const ROLE_DASHBOARD: Record<string, string> = {
  Admin: '/admin/dashboard',
  Operator: '/operator/dashboard',
  Participant: '/participant/dashboard',
};

const inputStyle: Record<string, string> = {
  width: '100%',
  padding: '12px 16px',
  fontSize: '16px',
  border: '2px solid #e5e4e7',
  borderRadius: '8px',
  outline: 'none',
  boxSizing: 'border-box',
  transition: 'border-color 0.2s',
  background: '#fff',
  color: '#08060d',
};

export function LoginPage() {
  const navigate = useNavigate();
  const { login } = useAuth();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<{ email?: string; password?: string }>({});
  const [submitting, setSubmitting] = useState(false);

  function validate(): boolean {
    const errors: { email?: string; password?: string } = {};

    if (!email.trim()) {
      errors.email = 'El correo electrónico es obligatorio';
    } else if (!EMAIL_REGEX.test(email.trim())) {
      errors.email = 'Ingrese un correo electrónico válido';
    }

    if (!password) {
      errors.password = 'La contraseña es obligatoria';
    }

    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);

    if (!validate()) return;

    setSubmitting(true);
    try {
      const user = await login(email.trim(), password);
      const target = ROLE_DASHBOARD[user.role] ?? '/login';
      navigate(target, { replace: true });
    } catch {
      setError('Credenciales inválidas');
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div style={{
      minHeight: '100vh',
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      background: '#f4f3ec',
      fontFamily: 'system-ui, Segoe UI, Roboto, sans-serif',
    }}>
      <div style={{
        background: '#fff',
        borderRadius: '16px',
        padding: '48px 40px',
        width: '100%',
        maxWidth: '420px',
        boxShadow: '0 10px 15px -3px rgba(0,0,0,0.1), 0 4px 6px -2px rgba(0,0,0,0.05)',
      }}>
        <h1 style={{
          fontSize: '28px',
          fontWeight: 500,
          color: '#08060d',
          margin: '0 0 8px',
          textAlign: 'center',
        }}>
          Iniciar sesión
        </h1>
        <p style={{
          fontSize: '16px',
          color: '#6b6375',
          textAlign: 'center',
          margin: '0 0 32px',
        }}>
          Ingrese sus credenciales para acceder al sistema
        </p>

        {error && (
          <div style={{
            background: '#fef2f2',
            color: '#dc2626',
            padding: '12px 16px',
            borderRadius: '8px',
            marginBottom: '20px',
            fontSize: '14px',
            textAlign: 'center',
            border: '1px solid #fecaca',
          }}>
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} noValidate>
          <div style={{ marginBottom: '20px' }}>
            <label
              htmlFor="email"
              style={{
                display: 'block',
                marginBottom: '6px',
                fontSize: '14px',
                fontWeight: 500,
                color: '#08060d',
              }}
            >
              Correo electrónico
            </label>
            <input
              id="email"
              type="email"
              value={email}
              onChange={(e) => { setEmail(e.target.value); setFieldErrors((prev) => ({ ...prev, email: undefined })); }}
              placeholder="usuario@ejemplo.com"
              style={{
                ...inputStyle,
                borderColor: fieldErrors.email ? '#dc2626' : '#e5e4e7',
              }}
              disabled={submitting}
              autoComplete="email"
            />
            {fieldErrors.email && (
              <p style={{ color: '#dc2626', fontSize: '13px', margin: '4px 0 0' }}>
                {fieldErrors.email}
              </p>
            )}
          </div>

          <div style={{ marginBottom: '24px' }}>
            <label
              htmlFor="password"
              style={{
                display: 'block',
                marginBottom: '6px',
                fontSize: '14px',
                fontWeight: 500,
                color: '#08060d',
              }}
            >
              Contraseña
            </label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={(e) => { setPassword(e.target.value); setFieldErrors((prev) => ({ ...prev, password: undefined })); }}
              placeholder="••••••••"
              style={{
                ...inputStyle,
                borderColor: fieldErrors.password ? '#dc2626' : '#e5e4e7',
              }}
              disabled={submitting}
              autoComplete="current-password"
            />
            {fieldErrors.password && (
              <p style={{ color: '#dc2626', fontSize: '13px', margin: '4px 0 0' }}>
                {fieldErrors.password}
              </p>
            )}
          </div>

          <button
            type="submit"
            disabled={submitting}
            style={{
              width: '100%',
              padding: '12px',
              fontSize: '16px',
              fontWeight: 500,
              color: '#fff',
              background: submitting ? '#9ca3af' : '#aa3bff',
              border: 'none',
              borderRadius: '8px',
              cursor: submitting ? 'not-allowed' : 'pointer',
              transition: 'background 0.2s',
            }}
            onMouseEnter={(e) => {
              if (!submitting) e.currentTarget.style.background = '#9333ea';
            }}
            onMouseLeave={(e) => {
              if (!submitting) e.currentTarget.style.background = '#aa3bff';
            }}
          >
            {submitting ? 'Ingresando...' : 'Ingresar'}
          </button>
        </form>

        <p
          style={{
            textAlign: 'center',
            marginTop: '24px',
            fontSize: '14px',
            color: '#6b6375',
          }}
        >
          ¿No tiene una cuenta?{' '}
          <Link
            to="/register"
            style={{
              color: '#aa3bff',
              textDecoration: 'none',
              fontWeight: 500,
            }}
          >
            Registrarse
          </Link>
        </p>
      </div>
    </div>
  );
}
