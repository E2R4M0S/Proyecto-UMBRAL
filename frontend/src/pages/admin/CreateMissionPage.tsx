import { useState, type FormEvent } from 'react';
import { Link } from 'react-router-dom';
import * as missionService from '../../services/missionService';

const DIFFICULTY_OPTIONS = [
  { label: 'Fácil', value: 'Facil' },
  { label: 'Media', value: 'Media' },
  { label: 'Difícil', value: 'Dificil' },
] as const;

const TIME_OPTIONS = [
  { label: '13 minutos', value: 13 },
  { label: '30 minutos', value: 30 },
  { label: '60 minutos', value: 60 },
  { label: '90 minutos', value: 90 },
] as const;

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

export function CreateMissionPage() {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [difficulty, setDifficulty] = useState('Facil');
  const [timeMinutes, setTimeMinutes] = useState(13);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<{ missionId: string } | null>(null);
  const [fieldErrors, setFieldErrors] = useState<
    Record<string, string | undefined>
  >({});
  const [submitting, setSubmitting] = useState(false);

  function validate(): boolean {
    const errors: Record<string, string | undefined> = {};

    if (!title.trim()) {
      errors.title = 'El título es obligatorio';
    } else if (title.trim().length > 150) {
      errors.title = 'El título no puede superar los 150 caracteres';
    }

    if (description.trim().length > 500) {
      errors.description = 'La descripción no puede superar los 500 caracteres';
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
      const result = await missionService.createMission({
        title: title.trim(),
        description: description.trim() || undefined,
        difficulty,
        timeMinutes,
      });
      setSuccess({ missionId: result.missionId });
      setTitle('');
      setDescription('');
      setDifficulty('Facil');
      setTimeMinutes(13);
    } catch (err: unknown) {
      const message =
        err instanceof Error ? err.message : 'Error al crear misión';
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
          Crear Misión
        </h1>
        <p
          style={{
            fontSize: '16px',
            color: '#6b6375',
            textAlign: 'center',
            margin: '0 0 32px',
          }}
        >
          Administrador — cree una nueva misión con narrativa y parámetros
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
            Misión creada exitosamente (ID: {success.missionId})
          </div>
        )}

        <form onSubmit={handleSubmit} noValidate>
          {/* Título */}
          <div style={{ marginBottom: '20px' }}>
            <label
              htmlFor="ms-title"
              style={{
                display: 'block',
                marginBottom: '6px',
                fontSize: '14px',
                fontWeight: 500,
                color: '#08060d',
              }}
            >
              Título
            </label>
            <input
              id="ms-title"
              type="text"
              value={title}
              onChange={(e) => {
                setTitle(e.target.value);
                clearFieldError('title');
              }}
              placeholder="Título de la misión"
              maxLength={150}
              style={{
                ...inputStyle,
                borderColor: fieldErrors.title ? '#dc2626' : '#e5e4e7',
              }}
              disabled={submitting}
              autoComplete="off"
            />
            {fieldErrors.title && (
              <p
                style={{
                  color: '#dc2626',
                  fontSize: '13px',
                  margin: '4px 0 0',
                }}
              >
                {fieldErrors.title}
              </p>
            )}
          </div>

          {/* Descripción */}
          <div style={{ marginBottom: '20px' }}>
            <label
              htmlFor="ms-description"
              style={{
                display: 'block',
                marginBottom: '6px',
                fontSize: '14px',
                fontWeight: 500,
                color: '#08060d',
              }}
            >
              Descripción
            </label>
            <textarea
              id="ms-description"
              value={description}
              onChange={(e) => {
                setDescription(e.target.value);
                clearFieldError('description');
              }}
              placeholder="Descripción de la misión (opcional)"
              maxLength={500}
              rows={4}
              style={{
                ...inputStyle,
                resize: 'vertical',
                borderColor: fieldErrors.description ? '#dc2626' : '#e5e4e7',
              }}
              disabled={submitting}
            />
            {fieldErrors.description && (
              <p
                style={{
                  color: '#dc2626',
                  fontSize: '13px',
                  margin: '4px 0 0',
                }}
              >
                {fieldErrors.description}
              </p>
            )}
          </div>

          {/* Dificultad */}
          <div style={{ marginBottom: '20px' }}>
            <label
              htmlFor="ms-difficulty"
              style={{
                display: 'block',
                marginBottom: '6px',
                fontSize: '14px',
                fontWeight: 500,
                color: '#08060d',
              }}
            >
              Dificultad
            </label>
            <select
              id="ms-difficulty"
              value={difficulty}
              onChange={(e) => setDifficulty(e.target.value)}
              style={inputStyle}
              disabled={submitting}
            >
              {DIFFICULTY_OPTIONS.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
          </div>

          {/* Tiempo */}
          <div style={{ marginBottom: '24px' }}>
            <label
              htmlFor="ms-time"
              style={{
                display: 'block',
                marginBottom: '6px',
                fontSize: '14px',
                fontWeight: 500,
                color: '#08060d',
              }}
            >
              Tiempo
            </label>
            <select
              id="ms-time"
              value={timeMinutes}
              onChange={(e) => setTimeMinutes(Number(e.target.value))}
              style={inputStyle}
              disabled={submitting}
            >
              {TIME_OPTIONS.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
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
            {submitting ? 'Creando...' : 'Crear Misión'}
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
