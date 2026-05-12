import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { AuthState, User } from "../types";

interface AuthStore extends AuthState {
  setAccessToken: (token: string) => void;
  setRefreshToken: (token: string) => void;
  setUser: (user: User) => void;
  setLoading: (loading: boolean) => void;
  setError: (error: string | null) => void;
  setAuth: (accessToken: string, refreshToken: string, user: User) => void;
  logout: () => void;
  clearError: () => void;
  isAuthenticated: () => boolean;
  isAdmin: () => boolean;
}

export const useAuthStore = create<AuthStore>()(
  persist(
    (set, get) => ({
      accessToken: null,
      refreshToken: null,
      user: null,
      isLoading: false,
      error: null,

      setAccessToken: (token) => set({ accessToken: token }),
      setRefreshToken: (token) => set({ refreshToken: token }),
      setUser: (user) => set({ user }),
      setLoading: (loading) => set({ isLoading: loading }),
      setError: (error) => set({ error }),
      setAuth: (accessToken, refreshToken, user) =>
        set({ accessToken, refreshToken, user }),
      logout: () =>
        set({
          accessToken: null,
          refreshToken: null,
          user: null,
          error: null,
          isLoading: false,
        }),
      clearError: () => set({ error: null }),
      isAuthenticated: (): boolean => {
        const state = get();
        return !!state.accessToken && !!state.user;
      },
      isAdmin: (): boolean => {
        const state = get();
        return (
          state.user?.roles.includes("Admin") ||
          state.user?.roles.includes("admin") ||
          false
        );
      },
    }),
    {
      name: "auth-store",
      partialize: (state) => ({
        accessToken: state.accessToken,
        refreshToken: state.refreshToken,
        user: state.user,
      }),
    },
  ),
);
