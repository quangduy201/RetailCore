import { http } from "@/shared/services/http";
import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  RefreshTokenRequest,
  User,
  LogoutRequest,
  UpdateProfileRequest,
} from "../types";

const API_BASE_URL = "/auth";

export const authApi = {
  login: async (data: LoginRequest): Promise<AuthResponse> => {
    const response = await http.post<AuthResponse>(
      `${API_BASE_URL}/login`,
      data,
    );
    return response.data;
  },

  register: async (data: RegisterRequest): Promise<AuthResponse> => {
    const response = await http.post<AuthResponse>(
      `${API_BASE_URL}/register`,
      data,
    );
    return response.data;
  },

  refresh: async (refreshToken: string): Promise<AuthResponse> => {
    const response = await http.post<AuthResponse>(`${API_BASE_URL}/refresh`, {
      refreshToken,
    } as RefreshTokenRequest);
    return response.data;
  },

  logout: async (refreshToken: string): Promise<void> => {
    await http.post(`${API_BASE_URL}/logout`, {
      refreshToken,
    } as LogoutRequest);
  },

  logoutAll: async (): Promise<void> => {
    await http.post(`${API_BASE_URL}/logout-all`);
  },

  me: async (): Promise<User> => {
    const response = await http.get<User>(`${API_BASE_URL}/me`);
    return response.data;
  },

  updateProfile: async (data: UpdateProfileRequest): Promise<User> => {
    const response = await http.put<User>(`${API_BASE_URL}/me`, data);
    return response.data;
  },
};
