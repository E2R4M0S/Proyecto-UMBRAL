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

export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
}
