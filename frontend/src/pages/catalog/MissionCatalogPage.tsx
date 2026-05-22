import { useState, useEffect, useCallback, useContext } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { AuthContext } from '../../contexts/AuthContext';
import { getMissions, type GetMissionsParams } from '../../services/missionService';
import type { MissionListItem } from '../../types/missions';

const DIFFICULTY_OPTIONS = [
  { label: 'Todas', value: '' },
  { label: 'Fácil', value: 'Facil' },
  { label: 'Media', value: 'Media' },
  { label: 'Difícil', value: 'Dificil' },
] as const;

const STATUS_OPTIONS = [
  { label: 'Todos', value: '' },
  { label: 'Borrador', value: 'Borrador' },
  { label: 'Activa', value: 'Activa' },
  { label: 'Inactiva', value: 'Inactiva' },
] as const;

const PAGE_SIZE = 10;

const inputStyle: Record<string, string> = {
  width: '100%',
  padding: '10px 14px',
  fontSize: '14px',
  border: '2px solid #e5e4e7',
  borderRadius: '8px',
  outline: 'none',
  boxSizing: 'border-box',
  background: '#fff',
  color: '#08060d',
};

const selectStyle: Record<string, string> = {
  ...inputStyle,
  width: 'auto',
  minWidth: '140px',
};

const cellStyle: Record<string, string> = {
  padding: '12px 16px',
  fontSize: '14px',
  borderBottom: '1px solid #e5e4e7',
  textAlign: 'left',
  color: '#08060d',
};

function difficultyBadgeStyle(difficulty: string): Record<string, string | number> {
  const colors: Record<string, string> = {
    Facil: '#16a34a',
    Media: '#ca8a04',
    Dificil: '#dc2626',
  };
  return {
    display: 'inline-block',
    padding: '3px 10px',
    fontSize: '12px',
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
    padding: '3px 10px',
    fontSize: '12px',
    fontWeight: 500,
    borderRadius: '999px',
    color: colors[status] ?? '#6b6375',
    background: `${colors[status] ?? '#6b6375'}1a`,
  };
}

export function MissionCatalogPage() {
  const navigate = useNavigate();
  const auth = useContext(AuthContext);
  const [searchParams, setSearchParams] = useSearchParams();

  // Read initial filters from URL params
  const [search, setSearch] = useState(searchParams.get('search') ?? '');
  const [difficulty, setDifficulty] = useState(searchParams.get('difficulty') ?? '');
  const [status, setStatus] = useState(searchParams.get('status') ?? '');
  const [page, setPage] = useState(Number(searchParams.get('page')) || 1);

  const [missions, setMissions] = useState<MissionListItem[]>([]);
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Debounced search value
  const [debouncedSearch, setDebouncedSearch] = useState(search);

  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearch(search);
    }, 300);
    return () => clearTimeout(timer);
  }, [search]);

  // Reset to page 1 when filters change
  useEffect(() => {
    setPage(1);
  }, [debouncedSearch, difficulty, status]);

  // Sync URL params
  useEffect(() => {
    const params: Record<string, string> = {};
    if (debouncedSearch) params.search = debouncedSearch;
    if (difficulty) params.difficulty = difficulty;
    if (status) params.status = status;
    if (page > 1) params.page = String(page);
    setSearchParams(params, { replace: true });
  }, [debouncedSearch, difficulty, status, page, setSearchParams]);

  const fetchMissions = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const params: GetMissionsParams = {
        page,
        pageSize: PAGE_SIZE,
      };
      if (debouncedSearch) params.search = debouncedSearch;
      if (difficulty) params.difficulty = difficulty;
      if (status) params.status = status;

      const result = await getMissions(params);
      setMissions(result.items);
      setTotalPages(result.totalPages);
      setTotalCount(result.totalCount);
    } catch (err: unknown) {
      const message =
        err instanceof Error ? err.message : 'Error al cargar misiones';
      setError(message);
    } finally {
      setLoading(false);
    }
  }, [page, debouncedSearch, difficulty, status]);

  useEffect(() => {
    fetchMissions();
  }, [fetchMissions]);

  function dashboardPath(): string {
    const role = auth?.user?.role;
    if (role === 'Admin') return '/admin/dashboard';
    if (role === 'Operator') return '/operator/dashboard';
    if (role === 'Participant') return '/participant/dashboard';
    return '/login';
  }

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
          Catálogo de Misiones
        </h1>
        <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
          <span style={{ fontSize: '14px', color: '#6b6375' }}>
            {auth?.user?.name} ({auth?.user?.role})
          </span>
          <button
            onClick={() => navigate(dashboardPath())}
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
            Volver al panel
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
          maxWidth: '960px',
          margin: '0 auto',
          padding: '32px 24px',
        }}
      >
        {/* Filters */}
        <div
          style={{
            display: 'flex',
            gap: '12px',
            flexWrap: 'wrap',
            marginBottom: '24px',
            alignItems: 'flex-end',
          }}
        >
          {/* Search */}
          <div style={{ flex: '1 1 200px', minWidth: '180px' }}>
            <label
              htmlFor="catalog-search"
              style={{
                display: 'block',
                marginBottom: '4px',
                fontSize: '13px',
                fontWeight: 500,
                color: '#6b6375',
              }}
            >
              Buscar
            </label>
            <input
              id="catalog-search"
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Buscar por título..."
              style={inputStyle}
            />
          </div>

          {/* Difficulty filter */}
          <div>
            <label
              htmlFor="catalog-difficulty"
              style={{
                display: 'block',
                marginBottom: '4px',
                fontSize: '13px',
                fontWeight: 500,
                color: '#6b6375',
              }}
            >
              Dificultad
            </label>
            <select
              id="catalog-difficulty"
              value={difficulty}
              onChange={(e) => setDifficulty(e.target.value)}
              style={selectStyle}
            >
              {DIFFICULTY_OPTIONS.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
          </div>

          {/* Status filter */}
          <div>
            <label
              htmlFor="catalog-status"
              style={{
                display: 'block',
                marginBottom: '4px',
                fontSize: '13px',
                fontWeight: 500,
                color: '#6b6375',
              }}
            >
              Estado
            </label>
            <select
              id="catalog-status"
              value={status}
              onChange={(e) => setStatus(e.target.value)}
              style={selectStyle}
            >
              {STATUS_OPTIONS.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
          </div>
        </div>

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
            Cargando misiones...
          </div>
        )}

        {/* Table */}
        {!loading && !error && (
          <>
            {missions.length === 0 ? (
              <div
                style={{
                  textAlign: 'center',
                  padding: '48px',
                  color: '#6b6375',
                  fontSize: '16px',
                  background: '#fff',
                  borderRadius: '12px',
                  border: '2px solid #e5e4e7',
                }}
              >
                No se encontraron misiones con los filtros seleccionados.
              </div>
            ) : (
              <>
                <div
                  style={{
                    background: '#fff',
                    borderRadius: '12px',
                    border: '2px solid #e5e4e7',
                    overflow: 'hidden',
                  }}
                >
                  <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                    <thead>
                      <tr style={{ background: '#faf9f7' }}>
                        <th
                          style={{
                            ...cellStyle,
                            fontWeight: 600,
                            color: '#6b6375',
                            fontSize: '12px',
                            textTransform: 'uppercase',
                            letterSpacing: '0.5px',
                          }}
                        >
                          Título
                        </th>
                        <th
                          style={{
                            ...cellStyle,
                            fontWeight: 600,
                            color: '#6b6375',
                            fontSize: '12px',
                            textTransform: 'uppercase',
                            letterSpacing: '0.5px',
                          }}
                        >
                          Dificultad
                        </th>
                        <th
                          style={{
                            ...cellStyle,
                            fontWeight: 600,
                            color: '#6b6375',
                            fontSize: '12px',
                            textTransform: 'uppercase',
                            letterSpacing: '0.5px',
                          }}
                        >
                          Estado
                        </th>
                      </tr>
                    </thead>
                    <tbody>
                      {missions.map((mission) => (
                        <tr
                          key={mission.id}
                          onClick={() => navigate(`/missions/${mission.id}`)}
                          style={{
                            cursor: 'pointer',
                            transition: 'background 0.15s',
                          }}
                          onMouseEnter={(e) => {
                            e.currentTarget.style.background = '#faf9f7';
                          }}
                          onMouseLeave={(e) => {
                            e.currentTarget.style.background = 'transparent';
                          }}
                        >
                          <td
                            style={{
                              ...cellStyle,
                              fontWeight: 500,
                            }}
                          >
                            {mission.title}
                          </td>
                          <td style={cellStyle}>
                            <span style={difficultyBadgeStyle(mission.difficulty)}>
                              {mission.difficulty}
                            </span>
                          </td>
                          <td style={cellStyle}>
                            <span style={statusBadgeStyle(mission.status)}>
                              {mission.status}
                            </span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>

                {/* Pagination */}
                {totalPages > 1 && (
                  <div
                    style={{
                      display: 'flex',
                      justifyContent: 'center',
                      alignItems: 'center',
                      gap: '8px',
                      marginTop: '24px',
                    }}
                  >
                    <button
                      onClick={() => setPage((p) => Math.max(1, p - 1))}
                      disabled={page <= 1}
                      style={{
                        padding: '8px 16px',
                        fontSize: '14px',
                        color: page <= 1 ? '#ccc' : '#08060d',
                        background: '#fff',
                        border: '1px solid #e5e4e7',
                        borderRadius: '6px',
                        cursor: page <= 1 ? 'not-allowed' : 'pointer',
                      }}
                    >
                      Anterior
                    </button>

                    <span
                      style={{
                        fontSize: '14px',
                        color: '#6b6375',
                        padding: '0 12px',
                      }}
                    >
                      Página {page} de {totalPages}
                      <span style={{ marginLeft: '8px', color: '#aaa' }}>
                        ({totalCount} misiones)
                      </span>
                    </span>

                    <button
                      onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                      disabled={page >= totalPages}
                      style={{
                        padding: '8px 16px',
                        fontSize: '14px',
                        color: page >= totalPages ? '#ccc' : '#08060d',
                        background: '#fff',
                        border: '1px solid #e5e4e7',
                        borderRadius: '6px',
                        cursor: page >= totalPages ? 'not-allowed' : 'pointer',
                      }}
                    >
                      Siguiente
                    </button>
                  </div>
                )}
              </>
            )}
          </>
        )}
      </main>
    </div>
  );
}
