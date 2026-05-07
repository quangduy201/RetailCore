import type {
  DateTime,
  PaginationRequest,
  UUID,
} from "../../shared/types/common";
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
  description?: string | null;
  status: BrandStatus;
  createdAt: DateTime;
  updatedAt?: DateTime | null;
}

// Requests
export interface GetBrandsRequest extends PaginationRequest {
  keyword?: string;
  status?: BrandStatus;
}

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
