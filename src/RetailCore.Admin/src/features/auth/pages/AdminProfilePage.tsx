import {
  Avatar,
  Button,
  Card,
  Chip,
  Input,
  Label,
  TextField,
} from "@heroui/react";
import { User, Shield, Edit2, Lock, Smartphone } from "lucide-react";
import { useAuth } from "@/features/auth/hooks/useAuth";

export default function AdminProfilePage() {
  const { user } = useAuth();

  if (!user) {
    return (
      <div className="flex items-center justify-center h-screen text-sm text-zinc-500">
        <p>Loading user data...</p>
      </div>
    );
  }

  const formatDate = (dateString?: string) => {
    if (!dateString) return "N/A";
    return new Date(dateString).toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  };

  return (
    <div className="max-w-3xl mx-auto py-8 space-y-6">
      {/* PROFILE CARD */}
      <Card className="border bg-gray-50">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-zinc-200">
          <div className="flex items-center gap-4">
            <Avatar className="aspect-square w-16 h-16">
              <Avatar.Image src={user.avatarUrl} />
              <Avatar.Fallback className="border-none bg-linear-to-br from-blue-600 to-green-600 text-white">
                {user.fullName.split(" ").map((name) => name.at(0))}
              </Avatar.Fallback>
            </Avatar>

            <div>
              <p className="text-base font-semibold">{user.fullName}</p>
              <p className="text-sm text-zinc-500">{user.email}</p>
            </div>
          </div>

          <Button variant="secondary" size="sm">
            <Edit2 className="w-4 h-4" />
            Edit
          </Button>
        </div>

        {/* Content */}
        <div className="p-6 space-y-6">
          {/* Personal Info */}
          <div className="space-y-4">
            <h3 className="flex items-center gap-2 font-semibold text-sm text-zinc-700">
              <User className="w-4 h-4" />
              Personal Information
            </h3>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <TextField isReadOnly>
                <Label>Full Name</Label>
                <Input value={user.fullName} />
              </TextField>

              <TextField isReadOnly>
                <Label>Email</Label>
                <Input value={user.email} />
              </TextField>

              <TextField isReadOnly>
                <Label>User ID</Label>
                <Input value={user.id} />
              </TextField>

              <TextField isReadOnly>
                <Label>Created At</Label>
                <Input value={formatDate(user.createdAt)} />
              </TextField>
            </div>
          </div>

          {/* Roles */}
          <div className="border-t border-zinc-200 pt-4 space-y-3">
            <h3 className="flex items-center gap-2 font-semibold text-sm text-zinc-700">
              <Shield className="w-4 h-4" />
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

          {/* Status */}
          <div className="border-t border-zinc-200 pt-4">
            <h3 className="font-semibold text-sm text-zinc-700 mb-3">
              Account Status
            </h3>

            <div className="flex items-center gap-2 text-green-600 text-sm font-medium">
              <div className="w-2.5 h-2.5 rounded-full bg-green-500" />
              Active
            </div>
          </div>
        </div>
      </Card>

      {/* SECURITY CARD */}
      <Card className="border bg-gray-50">
        <div className="p-6 border-b border-zinc-200">
          <h3 className="font-semibold">Security</h3>
        </div>

        <div className="p-6 space-y-4">
          {/* Password */}
          <div className="flex items-start gap-3">
            <Lock className="w-5 h-5 mt-1 text-zinc-600" />

            <div className="flex-1">
              <p className="font-semibold text-sm">Change Password</p>
              <p className="text-sm text-zinc-500 mb-3">
                Update your password regularly to stay safe.
              </p>

              <Button size="sm">Change Password</Button>
            </div>
          </div>

          <div className="border-t border-zinc-200" />

          {/* 2FA */}
          <div className="flex items-start gap-3">
            <Smartphone className="w-5 h-5 mt-1 text-zinc-600" />

            <div className="flex-1">
              <p className="font-semibold text-sm">Two-Factor Authentication</p>
              <p className="text-sm text-zinc-500 mb-3">
                Add an extra layer of protection.
              </p>

              <Button size="sm" variant="secondary">
                Enable 2FA
              </Button>
            </div>
          </div>
        </div>
      </Card>
    </div>
  );
}
