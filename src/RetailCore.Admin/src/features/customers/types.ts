import type { DateTime, UUID } from "@/shared/types/common";

export interface UserDto {
  id: UUID;
  email: string;
  fullName: string;
  avatarUrl?: string | null;
  roles: string[];
  isActive: boolean;
  createdAt: DateTime;
}
