import type { DateTime, UUID } from "../../shared/types/common";
import type {
  ProductStatus,
  ProductVariantStatus,
} from "../../shared/types/enums";

// Attribute Value
export interface ProductAttributeValueDto {
  id: UUID;
  value: string;
}

// Attribute
export interface ProductAttributeDto {
  id: UUID;
  name: string;
  values: ProductAttributeValueDto[];
}

// Variant Attribute
export interface ProductVariantAttributeDto {
  attributeId: UUID;
  attributeName: string;
  attributeValueId: UUID;
  attributeValue: string;
}

// Variant Image
export interface ProductVariantImageDto {
  id: UUID;
  url: string;
  sortOrder: number;
  isPrimary: boolean;
}

// Variant
export interface ProductVariantDto {
  id: UUID;
  sku: string;
  name?: string;
  description?: string;
  price: number;
  compareAtPrice?: number;
  discountPercentage?: number;
  stock: number;
  status: ProductVariantStatus;
  images: ProductVariantImageDto[];
  attributes: ProductVariantAttributeDto[];
}

// Product Detail
export interface ProductDetailDto {
  id: UUID;
  name: string;
  slug: string;
  shortDescription?: string;
  description?: string;

  brandId: UUID;
  brandName: string;
  categoryId: UUID;
  categoryName: string;

  status: ProductStatus;

  attributes: ProductAttributeDto[];
  variants: ProductVariantDto[];

  createdAt: DateTime;
  updatedAt?: DateTime;
}

// Product Management
export interface ProductManagementDto {
  id: UUID;
  name: string;
  slug: string;
  shortDescription?: string;
  description?: string;

  brandId: UUID;
  brandName: string;
  categoryId: UUID;
  categoryName: string;

  status: ProductStatus;

  variantCount: number;
  stock: number;
  minPrice?: number;
  maxPrice?: number;
  thumbnailUrl?: string;

  createdAt: DateTime;
  updatedAt?: DateTime;
}

// Requests

// Create Product
export interface CreateProductRequest {
  name: string;
  slug: string;
  shortDescription?: string;
  description?: string;
  brandId: UUID;
  categoryId: UUID;
  attributes: CreateProductAttributeRequest[];
  variants: CreateProductVariantRequest[];
}

// Update Product
export interface UpdateProductRequest {
  name: string;
  slug: string;
  shortDescription?: string;
  description?: string;
  brandId: UUID;
  categoryId: UUID;
  status: ProductStatus;
}

// Attribute
export interface CreateProductAttributeRequest {
  name: string;
  values: CreateProductAttributeValueRequest[];
}

export interface UpdateProductAttributeRequest {
  id: UUID;
  name: string;
}

// Attribute Value
export interface CreateProductAttributeValueRequest {
  value: string;
}

export interface UpdateProductAttributeValueRequest {
  id: UUID;
  value: string;
}

// Variant
export interface CreateProductVariantRequest {
  sku: string;
  name?: string;
  description?: string;
  price: number;
  compareAtPrice?: number;
  stock: number;
  status: ProductVariantStatus;
  attributeValueIds: UUID[];
  images: CreateProductVariantImageRequest[];
}

export interface UpdateProductVariantRequest {
  sku: string;
  name?: string;
  description?: string;
  price: number;
  compareAtPrice?: number;
  stock: number;
  status: ProductVariantStatus;
  attributeValueIds: UUID[];
  images: UpdateProductVariantImageRequest[];
}

// Variant Image
export interface CreateProductVariantImageRequest {
  url: string;
  sortOrder: number;
  isPrimary: boolean;
}

export interface UpdateProductVariantImageRequest {
  id?: UUID;
  url: string;
  sortOrder: number;
  isPrimary: boolean;
}

// Query
export interface GetProductsRequest {
  keyword?: string;
  brandId?: UUID;
  categoryId?: UUID;
  status?: ProductStatus;
  pageNumber?: number;
  pageSize?: number;
}
