import type { DateTime, UUID } from "../../shared/types/common";
import type { BrandStatus } from "../../shared/types/enums";

export interface BrandSummaryDto {
  id: UUID;
  name: string;
  slug: string;
}

export interface BrandDetailDto {
  id: UUID;
  name: string;
  slug: string;
  description?: string;
  status: BrandStatus;
  createdAt: DateTime;
  updatedAt?: DateTime;
}

// Requests
export interface CreateBrandRequest {
  name: string;
  slug: string;
  description?: string;
}

export interface UpdateBrandRequest {
  name: string;
  slug: string;
  description?: string;
  status: BrandStatus;
}

export interface GetBrandsRequest {
  keyword?: string;
  status?: BrandStatus;
  pageNumber?: number;
  pageSize?: number;
}
