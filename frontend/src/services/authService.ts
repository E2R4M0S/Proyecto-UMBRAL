import { apiClient } from './apiClient';
import type { LoginRequest, LoginResponse } from '../types/auth';

const AUTH_TOKEN_KEY = 'authToken';

export async function login(
  email: string,
  password: string
): Promise<LoginResponse> {
  const body: LoginRequest = { email, password };
  const response = await apiClient.post<LoginResponse>('/auth/login', body);
  localStorage.setItem(AUTH_TOKEN_KEY, response.token);
  return response;
}

export function logout(): void {
  localStorage.removeItem(AUTH_TOKEN_KEY);
}

export function getToken(): string | null {
  return localStorage.getItem(AUTH_TOKEN_KEY);
}
