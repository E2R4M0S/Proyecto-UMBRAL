export interface CreateMissionRequest {
  title: string;
  description?: string;
  difficulty: string;
  timeMinutes: number;
}

export interface CreateMissionResponse {
  missionId: string;
}

export interface MissionListItem {
  id: string;
  title: string;
  difficulty: string;
  status: string;
}

export interface MissionDetail {
  id: string;
  title: string;
  description: string | null;
  difficulty: string;
  timeMinutes: number;
  status: string;
  createdAt: string;
  updatedAt: string | null;
}

export interface PaginatedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
