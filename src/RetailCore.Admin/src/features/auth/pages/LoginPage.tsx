import { useForm } from "react-hook-form";
import { Navigate } from "react-router-dom";
import { useLogin, useAuth } from "../hooks/useAuth";
import type { LoginRequest } from "../types";
import {
  Button,
  FieldError,
  Input,
  Label,
  Spinner,
  TextField,
} from "@heroui/react";

export default function LoginPage() {
  const { isAuthenticated, isLoading } = useAuth();
  const { login } = useLogin();

  const {
    register,
    handleSubmit,
    formState: { errors },
    setError,
  } = useForm<LoginRequest>({
    mode: "onBlur",
  });

  if (isAuthenticated) {
    return <Navigate to="/admin/dashboard" replace />;
  }

  const onSubmit = async (data: LoginRequest) => {
    try {
      await login(data);
    } catch (err) {
      const errorMessage =
        err instanceof Error ? err.message : "Login failed. Please try again.";
      setError("root", { message: errorMessage });
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 to-slate-800 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Card */}
        <div className="bg-white rounded-lg shadow-xl p-8">
          {/* Header */}
          <div className="mb-8 text-center">
            <h1 className="text-3xl font-bold text-slate-900 mb-2">
              RetailCore Admin
            </h1>
            <p className="text-slate-600">
              Sign in to RetailCore Admin dashboard
            </p>
          </div>

          {/* Form */}
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
            {/* Email */}
            <TextField isInvalid={!!errors.email} isRequired name="email">
              <Label>Email Address</Label>
              <Input
                type="email"
                placeholder="abc@example.com"
                {...register("email", {
                  required: "Email is required",
                  pattern: {
                    value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                    message: "Invalid email address",
                  },
                })}
              />
              {errors.email ? (
                <FieldError>{errors.email.message}</FieldError>
              ) : null}
            </TextField>

            {/* Password */}
            <TextField isInvalid={!!errors.password} isRequired name="password">
              <Label>Password</Label>
              <Input
                type="password"
                placeholder="••••••••"
                {...register("password", {
                  required: "Password is required",
                  minLength: {
                    value: 6,
                    message: "Password must be at least 6 characters",
                  },
                })}
              />
              {errors.password ? (
                <FieldError>{errors.password.message}</FieldError>
              ) : null}
            </TextField>

            {/* Root Error */}
            {errors.root && (
              <div className="bg-red-50 border border-red-200 rounded-lg p-3">
                <p className="text-sm text-red-700">{errors.root.message}</p>
              </div>
            )}

            {/* Submit Button */}
            <Button
              type="submit"
              isPending={isLoading}
              className="w-full rounded-md"
            >
              {isLoading && <Spinner color="current" size="sm" />}
              {isLoading ? "Signing in..." : "Sign In"}
            </Button>
          </form>
        </div>
      </div>
    </div>
  );
}
