import { useState, useEffect, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { AuthContext } from '../../contexts/AuthContext';
import { ApiError } from '../../services/apiClient';
import { getMissionById } from '../../services/missionService';
import type { MissionDetail } from '../../types/missions';

function difficultyBadgeStyle(difficulty: string): Record<string, string | number> {
  const colors: Record<string, string> = {
    Facil: '#16a34a',
    Media: '#ca8a04',
    Dificil: '#dc2626',
  };
  return {
    display: 'inline-block',
    padding: '4px 12px',
    fontSize: '13px',
    fontWeight: 500,
    borderRadius: '999px',
    color: colors[difficulty] ?? '#6b6375',
    background: `${colors[difficulty] ?? '#6b6375'}1a`,
  };
}

function statusBadgeStyle(status: string): Record<string, string | number> {
  const colors: Record<string, string> = {
    Activa: '#16a34a',
    Borrador: '#6b6375',
    Inactiva: '#dc2626',
  };
  return {
    display: 'inline-block',
    padding: '4px 12px',
    fontSize: '13px',
    fontWeight: 500,
    borderRadius: '999px',
    color: colors[status] ?? '#6b6375',
    background: `${colors[status] ?? '#6b6375'}1a`,
  };
}

function formatDate(iso: string): string {
  const date = new Date(iso);
  return date.toLocaleDateString('es-AR', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

export function MissionDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const auth = useContext(AuthContext);

  const [mission, setMission] = useState<MissionDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [notFound, setNotFound] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    const missionId: string = id;

    async function fetchDetail() {
      setLoading(true);
      setNotFound(false);
      setError(null);
      try {
        const data = await getMissionById(missionId);
        setMission(data);
      } catch (err: unknown) {
        if (err instanceof ApiError && err.status === 404) {
          setNotFound(true);
        } else {
          const message =
            err instanceof Error ? err.message : 'Error al cargar misión';
          setError(message);
        }
      } finally {
        setLoading(false);
      }
    }

    fetchDetail();
  }, [id]);

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
          Detalle de Misión
        </h1>
        <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
          <span style={{ fontSize: '14px', color: '#6b6375' }}>
            {auth?.user?.name} ({auth?.user?.role})
          </span>
          <button
            onClick={() => navigate('/missions')}
            style={{
              padding: '8px 16px',
              fontSize: '14px',
              color: '#aa3bff',
              background: '#fff',
              border: '1px solid #e5e4e7',
              borderRadius: '6px',
              cursor: 'pointer',
            }}
          >
            Volver al catálogo
          </button>
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
          maxWidth: '720px',
          margin: '0 auto',
          padding: '32px 24px',
        }}
      >
        {/* Back link */}
        <button
          onClick={() => navigate('/missions')}
          style={{
            display: 'inline-flex',
            alignItems: 'center',
            gap: '4px',
            padding: '6px 12px',
            fontSize: '14px',
            color: '#6b6375',
            background: 'transparent',
            border: 'none',
            cursor: 'pointer',
            marginBottom: '24px',
          }}
        >
          ← Volver al catálogo
        </button>

        {/* Loading */}
        {loading && (
          <div
            style={{
              textAlign: 'center',
              padding: '48px',
              color: '#6b6375',
              fontSize: '16px',
            }}
          >
            Cargando misión...
          </div>
        )}

        {/* Error */}
        {error && (
          <div
            style={{
              background: '#fef2f2',
              color: '#dc2626',
              padding: '12px 16px',
              borderRadius: '8px',
              marginBottom: '20px',
              fontSize: '14px',
              border: '1px solid #fecaca',
            }}
          >
            {error}
          </div>
        )}

        {/* Not found */}
        {notFound && !loading && (
          <div
            style={{
              textAlign: 'center',
              padding: '48px',
              color: '#6b6375',
              fontSize: '18px',
              background: '#fff',
              borderRadius: '12px',
              border: '2px solid #e5e4e7',
            }}
          >
            <p style={{ fontSize: '48px', margin: '0 0 16px', color: '#e5e4e7' }}>
              404
            </p>
            <p style={{ margin: 0 }}>Misión no encontrada</p>
          </div>
        )}

        {/* Detail card */}
        {mission && !loading && (
          <div
            style={{
              background: '#fff',
              borderRadius: '16px',
              border: '2px solid #e5e4e7',
              padding: '32px',
            }}
          >
            {/* Title + badges */}
            <div
              style={{
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'flex-start',
                flexWrap: 'wrap',
                gap: '12px',
                marginBottom: '24px',
              }}
            >
              <h2
                style={{
                  fontSize: '24px',
                  fontWeight: 600,
                  color: '#08060d',
                  margin: 0,
                }}
              >
                {mission.title}
              </h2>
              <div style={{ display: 'flex', gap: '8px' }}>
                <span style={difficultyBadgeStyle(mission.difficulty)}>
                  {mission.difficulty}
                </span>
                <span style={statusBadgeStyle(mission.status)}>
                  {mission.status}
                </span>
              </div>
            </div>

            {/* Description */}
            {mission.description && (
              <p
                style={{
                  fontSize: '15px',
                  color: '#4b4452',
                  lineHeight: 1.6,
                  margin: '0 0 24px',
                }}
              >
                {mission.description}
              </p>
            )}

            {/* Metadata fields */}
            <div
              style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
                gap: '16px',
                borderTop: '1px solid #e5e4e7',
                paddingTop: '24px',
              }}
            >
              <div>
                <p
                  style={{
                    fontSize: '12px',
                    fontWeight: 600,
                    color: '#6b6375',
                    textTransform: 'uppercase',
                    letterSpacing: '0.5px',
                    margin: '0 0 4px',
                  }}
                >
                  Tiempo estimado
                </p>
                <p style={{ fontSize: '16px', color: '#08060d', margin: 0 }}>
                  {mission.timeMinutes} minutos
                </p>
              </div>

              <div>
                <p
                  style={{
                    fontSize: '12px',
                    fontWeight: 600,
                    color: '#6b6375',
                    textTransform: 'uppercase',
                    letterSpacing: '0.5px',
                    margin: '0 0 4px',
                  }}
                >
                  Creada el
                </p>
                <p style={{ fontSize: '14px', color: '#08060d', margin: 0 }}>
                  {formatDate(mission.createdAt)}
                </p>
              </div>

              {mission.updatedAt && (
                <div>
                  <p
                    style={{
                      fontSize: '12px',
                      fontWeight: 600,
                      color: '#6b6375',
                      textTransform: 'uppercase',
                      letterSpacing: '0.5px',
                      margin: '0 0 4px',
                    }}
                  >
                    Actualizada el
                  </p>
                  <p style={{ fontSize: '14px', color: '#08060d', margin: 0 }}>
                    {formatDate(mission.updatedAt)}
                  </p>
                </div>
              )}
            </div>
          </div>
        )}
      </main>
    </div>
  );
}
