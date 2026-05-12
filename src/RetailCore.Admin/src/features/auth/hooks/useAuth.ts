import { useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "../store/authStore";
import { authApi } from "../api/auth.api";
import type { LoginRequest } from "../types";

export const useAuth = () => {
  const {
    accessToken,
    refreshToken,
    user,
    isLoading,
    error,
    setLoading,
    setError,
    logout: storeLogout,
    isAuthenticated,
    isAdmin,
  } = useAuthStore();

  return {
    accessToken,
    refreshToken,
    user,
    isLoading,
    error,
    setLoading,
    setError,
    logout: storeLogout,
    isAuthenticated: isAuthenticated(),
    isAdmin: isAdmin(),
  };
};

export const useLogin = () => {
  const navigate = useNavigate();
  const { setAuth, setLoading, setError, clearError } = useAuthStore();

  const login = useCallback(
    async (credentials: LoginRequest) => {
      try {
        setLoading(true);
        clearError();

        const response = await authApi.login(credentials);

        setAuth(response.accessToken, response.refreshToken, response.user);

        navigate("/admin");
        return response;
      } catch (err) {
        const errorMessage =
          err instanceof Error ? err.message : "Login failed";
        setError(errorMessage);
        throw err;
      } finally {
        setLoading(false);
      }
    },
    [navigate, setAuth, setLoading, setError, clearError],
  );

  return { login };
};

export const useLogout = () => {
  const navigate = useNavigate();
  const {
    logout: storeLogout,
    setLoading,
    setError,
    refreshToken,
  } = useAuthStore();

  const logout = useCallback(async () => {
    try {
      setLoading(true);
      console.log("Logging out:", refreshToken);
      await authApi.logout(refreshToken || "");
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : "Logout failed";
      console.error("Logout error:", errorMessage);
    } finally {
      storeLogout();
      setError(null);
      setLoading(false);
      console.log("Logged out:", refreshToken);
      navigate("/login");
    }
  }, [navigate, storeLogout, setLoading, setError]);

  return { logout };
};

export const useIsAuthorized = () => {
  const { isAuthenticated, isAdmin } = useAuth();

  return {
    canAccess: isAuthenticated,
    isAdmin,
    isAuthorized: isAuthenticated && isAdmin,
  };
};
