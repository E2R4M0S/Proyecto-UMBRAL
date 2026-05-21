export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  userId: string;
  userName: string;
  email: string;
  role: string;
}

export interface RegisterParticipantRequest {
  name: string;
  alias: string;
  email: string;
  password: string;
}

export interface CreateOperatorRequest {
  name: string;
  email: string;
  password: string;
}

export interface CreateOperatorResponse {
  userId: string;
}

export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
}
