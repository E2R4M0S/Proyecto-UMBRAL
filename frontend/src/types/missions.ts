export interface CreateMissionRequest {
  title: string;
  description?: string;
  difficulty: string;
  timeMinutes: number;
}

export interface CreateMissionResponse {
  missionId: string;
}
