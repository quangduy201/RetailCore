import type { DateTime, PaginationRequest, UUID } from "@/shared/types/common";
import type { CategoryStatus } from "@/shared/types/enums";

export interface CategorySummaryDto {
  id: UUID;
  name: string;
  slug: string;
  description?: string | null;
}

export interface CategoryDetailDto {
  id: UUID;
  name: string;
  slug: string;
  description?: string | null;
  status: CategoryStatus;
  createdAt: DateTime;
  updatedAt?: DateTime | null;
}

// Requests
export interface GetCategoriesRequest extends PaginationRequest {
  keyword?: string;
  status?: CategoryStatus;
}
export interface CreateCategoryRequest {
  name: string;
  slug: string;
  description?: string;
}

export interface UpdateCategoryRequest {
  name: string;
  slug: string;
  description?: string;
  status: CategoryStatus;
}
