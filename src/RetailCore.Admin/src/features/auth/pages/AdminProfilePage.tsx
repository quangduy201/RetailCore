import { useEffect, useState } from "react";
import {
  Avatar,
  Button,
  Card,
  Chip,
  FieldError,
  Input,
  Label,
  Spinner,
  TextField,
} from "@heroui/react";
import {
  AlertCircle,
  CheckCircle2,
  Edit2,
  Save,
  Shield,
  User,
  X,
} from "lucide-react";

import { authApi } from "@/features/auth/api/auth.api";
import { useAuth } from "@/features/auth/hooks/useAuth";
import { useAuthStore } from "@/features/auth/store/authStore";

const getInitials = (fullName: string) =>
  fullName
    .trim()
    .split(/\s+/)
    .slice(0, 2)
    .map((name) => name.at(0))
    .join("")
    .toUpperCase();

const formatDate = (dateString?: string) => {
  if (!dateString) {
    return "N/A";
  }

  return new Date(dateString).toLocaleDateString("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
};

export default function AdminProfilePage() {
  const { user } = useAuth();
  const setUser = useAuthStore((state) => state.setUser);

  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [fullName, setFullName] = useState("");
  const [avatarUrl, setAvatarUrl] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  useEffect(() => {
    if (!user) {
      return;
    }

    setFullName(user.fullName);
    setAvatarUrl(user.avatarUrl ?? "");
  }, [user]);

  if (!user) {
    return (
      <div className="flex h-screen items-center justify-center text-sm text-zinc-500">
        <Spinner />
      </div>
    );
  }

  const handleCancel = () => {
    setFullName(user.fullName);
    setAvatarUrl(user.avatarUrl ?? "");
    setError(null);
    setSuccess(null);
    setIsEditing(false);
  };

  const handleSave = async () => {
    if (!fullName.trim()) {
      setError("Full name is required.");
      return;
    }

    try {
      setIsSaving(true);
      setError(null);
      setSuccess(null);

      const updatedUser = await authApi.updateProfile({
        fullName: fullName.trim(),
        avatarUrl: avatarUrl.trim() || null,
      });

      setUser(updatedUser);
      setIsEditing(false);
      setSuccess("Profile updated successfully.");
    } catch {
      setError("Unable to update profile. Please try again.");
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className="mx-auto max-w-3xl space-y-6 py-8">
      <Card className="border bg-gray-50">
        <div className="flex flex-col gap-4 border-b border-zinc-200 p-6 sm:flex-row sm:items-center sm:justify-between">
          <div className="flex min-w-0 items-center gap-4">
            <Avatar className="size-16">
              {user.avatarUrl && <Avatar.Image src={user.avatarUrl} />}
              <Avatar.Fallback className="border-none bg-linear-to-br from-blue-600 to-green-600 text-white">
                {getInitials(user.fullName)}
              </Avatar.Fallback>
            </Avatar>

            <div className="min-w-0">
              <p className="truncate text-base font-semibold">{user.fullName}</p>
              <p className="truncate text-sm text-zinc-500">{user.email}</p>
            </div>
          </div>

          {isEditing ? (
            <div className="flex gap-2">
              <Button variant="secondary" size="sm" onPress={handleCancel}>
                <X className="size-4" />
                Cancel
              </Button>
              <Button size="sm" isPending={isSaving} onPress={handleSave}>
                <Save className="size-4" />
                Save
              </Button>
            </div>
          ) : (
            <Button
              variant="secondary"
              size="sm"
              onPress={() => setIsEditing(true)}
            >
              <Edit2 className="size-4" />
              Edit
            </Button>
          )}
        </div>

        <div className="space-y-6 p-6">
          {error && (
            <div className="flex gap-2 rounded-md border border-danger-200 bg-danger-50 p-3 text-sm text-danger-700">
              <AlertCircle className="mt-0.5 size-4" />
              {error}
            </div>
          )}

          {success && (
            <div className="flex gap-2 rounded-md border border-success-200 bg-success-50 p-3 text-sm text-success-700">
              <CheckCircle2 className="mt-0.5 size-4" />
              {success}
            </div>
          )}

          <div className="space-y-4">
            <h3 className="flex items-center gap-2 text-sm font-semibold text-zinc-700">
              <User className="size-4" />
              Personal Information
            </h3>

            <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
              <TextField isInvalid={isEditing && !fullName.trim()} isReadOnly={!isEditing}>
                <Label>Full Name</Label>
                <Input
                  value={fullName}
                  onChange={(event) => setFullName(event.target.value)}
                />
                {isEditing && !fullName.trim() && (
                  <FieldError>Full name is required.</FieldError>
                )}
              </TextField>

              <TextField isReadOnly>
                <Label>Email</Label>
                <Input value={user.email} />
              </TextField>

              <TextField isReadOnly={!isEditing}>
                <Label>Avatar URL</Label>
                <Input
                  value={avatarUrl}
                  placeholder="https://..."
                  onChange={(event) => setAvatarUrl(event.target.value)}
                />
              </TextField>

              <TextField isReadOnly>
                <Label>Created At</Label>
                <Input value={formatDate(user.createdAt)} />
              </TextField>
            </div>
          </div>

          <div className="space-y-3 border-t border-zinc-200 pt-4">
            <h3 className="flex items-center gap-2 text-sm font-semibold text-zinc-700">
              <Shield className="size-4" />
              Roles
            </h3>

            <div className="flex flex-wrap gap-2">
              {user.roles?.length ? (
                user.roles.map((role) => (
                  <Chip key={role} className="bg-blue-100 text-blue-700">
                    {role}
                  </Chip>
                ))
              ) : (
                <span className="text-sm text-zinc-500">No roles assigned</span>
              )}
            </div>
          </div>

          <div className="border-t border-zinc-200 pt-4">
            <h3 className="mb-3 text-sm font-semibold text-zinc-700">
              Account Status
            </h3>

            <div
              className={`flex items-center gap-2 text-sm font-medium ${
                user.isActive ? "text-green-600" : "text-amber-600"
              }`}
            >
              <div
                className={`size-2.5 rounded-full ${
                  user.isActive ? "bg-green-500" : "bg-amber-500"
                }`}
              />
              {user.isActive ? "Active" : "Inactive"}
            </div>
          </div>
        </div>
      </Card>
    </div>
  );
}
