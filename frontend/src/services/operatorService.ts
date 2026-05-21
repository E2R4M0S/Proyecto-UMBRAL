import { apiClient } from './apiClient';
import type {
  CreateOperatorRequest,
  CreateOperatorResponse,
} from '../types/auth';

export async function createOperator(
  data: CreateOperatorRequest
): Promise<CreateOperatorResponse> {
  return apiClient.post<CreateOperatorResponse>('/operators', data);
}
