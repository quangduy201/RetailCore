import type { DateTime, UUID } from "../../shared/types/common";
import type { CategoryStatus } from "../../shared/types/enums";

export interface CategorySummaryDto {
  id: UUID;
  name: string;
  slug: string;
  description?: string;
}

export interface CategoryDetailDto {
  id: UUID;
  name: string;
  slug: string;
  description?: string;
  status: CategoryStatus;
  createdAt: DateTime;
  updatedAt?: DateTime;
}

// Requests
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

export interface GetCategoriesRequest {
  keyword?: string;
  status?: CategoryStatus;
  pageNumber?: number;
  pageSize?: number;
}
