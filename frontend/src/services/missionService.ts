import { apiClient } from './apiClient';
import type { CreateMissionRequest, CreateMissionResponse } from '../types/missions';

const BASE_PATH = '/missions';

export async function createMission(
  data: CreateMissionRequest
): Promise<CreateMissionResponse> {
  return apiClient.post<CreateMissionResponse>(BASE_PATH, data);
}
