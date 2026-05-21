import {
  createContext,
  useState,
  useEffect,
  useCallback,
  type ReactNode,
} from 'react';
import type { User } from '../types/auth';
import * as authService from '../services/authService';

export interface AuthContextType {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<User>;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextType | null>(null);

interface AuthProviderProps {
  children: ReactNode;
}

function decodeTokenPayload(
  token: string
): Record<string, string> | null {
  try {
    const payload = token.split('.')[1];
    return JSON.parse(atob(payload)) as Record<string, string>;
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const storedToken = authService.getToken();
    if (storedToken) {
      const payload = decodeTokenPayload(storedToken);
      if (payload?.sub) {
        setUser({
          id: payload.sub,
          name: payload.name ?? '',
          email: payload.email ?? '',
          role: payload.role ?? '',
        });
        setToken(storedToken);
      } else {
        authService.logout();
      }
    }
    setIsLoading(false);
  }, []);

  const login = useCallback(
    async (email: string, password: string): Promise<User> => {
      const response = await authService.login(email, password);
      const userData: User = {
        id: response.userId,
        name: response.userName,
        email: response.email,
        role: response.role,
      };
      setUser(userData);
      setToken(response.token);
      return userData;
    },
    []
  );

  const logout = useCallback(() => {
    authService.logout();
    setUser(null);
    setToken(null);
  }, []);

  const value: AuthContextType = {
    user,
    token,
    isAuthenticated: !!user && !!token,
    isLoading,
    login,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
