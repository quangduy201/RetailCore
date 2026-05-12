import axios, { AxiosError } from "axios";
import type { InternalAxiosRequestConfig } from "axios";
import { env } from "../config/env";
import { useAuthStore } from "@/features/auth/store/authStore";

export const http = axios.create({
  baseURL: env.apiUrl,
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 30000,
});

// Token refresh mutex
let isRefreshing = false;
let refreshSubscribers: Array<(token: string) => void> = [];

const subscribeTokenRefresh = (callback: (token: string) => void) => {
  refreshSubscribers.push(callback);
};

const onRefreshed = (token: string) => {
  refreshSubscribers.forEach((callback) => callback(token));
  refreshSubscribers = [];
};

// Request interceptor: attach access token
http.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const store = useAuthStore.getState();
  const token = store.accessToken;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

// Response interceptor:  handle token refresh on 401
http.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & {
      _retry?: boolean;
    };

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        // Queue the request
        return new Promise((resolve) => {
          subscribeTokenRefresh((token: string) => {
            originalRequest.headers.Authorization = `Bearer ${token}`;
            resolve(http(originalRequest));
          });
        });
      }

      isRefreshing = true;
      originalRequest._retry = true;

      try {
        const store = useAuthStore.getState();
        const refreshToken = store.refreshToken;

        if (!refreshToken) {
          store.logout();
          globalThis.location.href = "/login";
          throw new Error("No refresh token available");
        }

        const response = await axios.post(
          `${env.apiUrl}/auth/refresh`,
          { refreshToken },
          { timeout: 30000 },
        );

        const { accessToken: newAccessToken, refreshToken: newRefreshToken } =
          response.data;

        store.setAccessToken(newAccessToken);
        store.setRefreshToken(newRefreshToken);

        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;

        onRefreshed(newAccessToken);

        return http(originalRequest);
      } catch (refreshError) {
        const store = useAuthStore.getState();
        store.logout();
        globalThis.location.href = "/login";
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  },
);
