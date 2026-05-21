import { useState, type FormEvent } from 'react';
import { Link } from 'react-router-dom';
import * as operatorService from '../../services/operatorService';

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

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

export function CreateOperatorPage() {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<{ userId: string } | null>(null);
  const [fieldErrors, setFieldErrors] = useState<
    Record<string, string | undefined>
  >({});
  const [submitting, setSubmitting] = useState(false);

  function validate(): boolean {
    const errors: Record<string, string | undefined> = {};

    if (!name.trim()) {
      errors.name = 'El nombre es obligatorio';
    }

    if (!email.trim()) {
      errors.email = 'El correo electrónico es obligatorio';
    } else if (!EMAIL_REGEX.test(email.trim())) {
      errors.email = 'Ingrese un correo electrónico válido';
    }

    if (!password) {
      errors.password = 'La contraseña es obligatoria';
    } else if (password.length < 8) {
      errors.password = 'La contraseña debe tener al menos 8 caracteres';
    }

    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  }

  function clearFieldError(field: string) {
    setFieldErrors((prev) => ({ ...prev, [field]: undefined }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    if (!validate()) return;

    setSubmitting(true);
    try {
      const result = await operatorService.createOperator({
        name: name.trim(),
        email: email.trim(),
        password,
      });
      setSuccess({ userId: result.userId });
      setName('');
      setEmail('');
      setPassword('');
    } catch (err: unknown) {
      const message =
        err instanceof Error ? err.message : 'Error al crear operador';
      setError(message);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div
      style={{
        minHeight: '100vh',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        background: '#f4f3ec',
        fontFamily: 'system-ui, Segoe UI, Roboto, sans-serif',
      }}
    >
      <div
        style={{
          background: '#fff',
          borderRadius: '16px',
          padding: '48px 40px',
          width: '100%',
          maxWidth: '420px',
          boxShadow:
            '0 10px 15px -3px rgba(0,0,0,0.1), 0 4px 6px -2px rgba(0,0,0,0.05)',
        }}
      >
        <h1
          style={{
            fontSize: '28px',
            fontWeight: 500,
            color: '#08060d',
            margin: '0 0 8px',
            textAlign: 'center',
          }}
        >
          Crear Operador
        </h1>
        <p
          style={{
            fontSize: '16px',
            color: '#6b6375',
            textAlign: 'center',
            margin: '0 0 32px',
          }}
        >
          Administrador — cree una nueva cuenta de operador
        </p>

        {error && (
          <div
            style={{
              background: '#fef2f2',
              color: '#dc2626',
              padding: '12px 16px',
              borderRadius: '8px',
              marginBottom: '20px',
              fontSize: '14px',
              textAlign: 'center',
              border: '1px solid #fecaca',
            }}
          >
            {error}
          </div>
        )}

        {success && (
          <div
            style={{
              background: '#f0fdf4',
              color: '#16a34a',
              padding: '12px 16px',
              borderRadius: '8px',
              marginBottom: '20px',
              fontSize: '14px',
              textAlign: 'center',
              border: '1px solid #bbf7d0',
            }}
          >
            Operador creado exitosamente (ID: {success.userId})
          </div>
        )}

        <form onSubmit={handleSubmit} noValidate>
          {/* Name */}
          <div style={{ marginBottom: '20px' }}>
            <label
              htmlFor="op-name"
              style={{
                display: 'block',
                marginBottom: '6px',
                fontSize: '14px',
                fontWeight: 500,
                color: '#08060d',
              }}
            >
              Nombre
            </label>
            <input
              id="op-name"
              type="text"
              value={name}
              onChange={(e) => {
                setName(e.target.value);
                clearFieldError('name');
              }}
              placeholder="Nombre del operador"
              style={{
                ...inputStyle,
                borderColor: fieldErrors.name ? '#dc2626' : '#e5e4e7',
              }}
              disabled={submitting}
              autoComplete="name"
            />
            {fieldErrors.name && (
              <p
                style={{
                  color: '#dc2626',
                  fontSize: '13px',
                  margin: '4px 0 0',
                }}
              >
                {fieldErrors.name}
              </p>
            )}
          </div>

          {/* Email */}
          <div style={{ marginBottom: '20px' }}>
            <label
              htmlFor="op-email"
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
              id="op-email"
              type="email"
              value={email}
              onChange={(e) => {
                setEmail(e.target.value);
                clearFieldError('email');
              }}
              placeholder="operador@ejemplo.com"
              style={{
                ...inputStyle,
                borderColor: fieldErrors.email ? '#dc2626' : '#e5e4e7',
              }}
              disabled={submitting}
              autoComplete="email"
            />
            {fieldErrors.email && (
              <p
                style={{
                  color: '#dc2626',
                  fontSize: '13px',
                  margin: '4px 0 0',
                }}
              >
                {fieldErrors.email}
              </p>
            )}
          </div>

          {/* Password */}
          <div style={{ marginBottom: '24px' }}>
            <label
              htmlFor="op-password"
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
              id="op-password"
              type="password"
              value={password}
              onChange={(e) => {
                setPassword(e.target.value);
                clearFieldError('password');
              }}
              placeholder="••••••••"
              style={{
                ...inputStyle,
                borderColor: fieldErrors.password ? '#dc2626' : '#e5e4e7',
              }}
              disabled={submitting}
              autoComplete="new-password"
            />
            {fieldErrors.password && (
              <p
                style={{
                  color: '#dc2626',
                  fontSize: '13px',
                  margin: '4px 0 0',
                }}
              >
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
            {submitting ? 'Creando...' : 'Crear Operador'}
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
          <Link
            to="/admin/dashboard"
            style={{
              color: '#aa3bff',
              textDecoration: 'none',
              fontWeight: 500,
            }}
          >
            Volver al panel
          </Link>
        </p>
      </div>
    </div>
  );
}
