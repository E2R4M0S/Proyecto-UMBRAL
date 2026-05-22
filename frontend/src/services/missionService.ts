import { apiClient } from './apiClient';
import type {
  CreateMissionRequest,
  CreateMissionResponse,
  MissionListItem,
  MissionDetail,
  PaginatedResult,
} from '../types/missions';

const BASE_PATH = '/missions';

export async function createMission(
  data: CreateMissionRequest
): Promise<CreateMissionResponse> {
  return apiClient.post<CreateMissionResponse>(BASE_PATH, data);
}

export interface GetMissionsParams {
  page?: number;
  pageSize?: number;
  difficulty?: string;
  status?: string;
  search?: string;
}

export async function getMissions(
  params: GetMissionsParams = {}
): Promise<PaginatedResult<MissionListItem>> {
  const query = new URLSearchParams();
  if (params.page !== undefined) query.set('page', String(params.page));
  if (params.pageSize !== undefined) query.set('pageSize', String(params.pageSize));
  if (params.difficulty) query.set('difficulty', params.difficulty);
  if (params.status) query.set('status', params.status);
  if (params.search) query.set('search', params.search);

  const qs = query.toString();
  return apiClient.get<PaginatedResult<MissionListItem>>(
    `${BASE_PATH}${qs ? `?${qs}` : ''}`
  );
}

export async function getMissionById(id: string): Promise<MissionDetail> {
  return apiClient.get<MissionDetail>(`${BASE_PATH}/${id}`);
}
