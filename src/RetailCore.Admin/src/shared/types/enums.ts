export const CategoryStatus = {
  Active: 1,
  Inactive: 2,
  Archived: 3,
} as const;

export const BrandStatus = {
  Active: 1,
  Inactive: 2,
} as const;

export const ProductStatus = {
  Draft: 0,
  Active: 1,
  Inactive: 2,
  Archived: 3,
} as const;

export const ProductVariantStatus = {
  Draft: 0,
  Active: 1,
  Inactive: 2,
  Discontinued: 3,
} as const;

export type CategoryStatus =
  (typeof CategoryStatus)[keyof typeof CategoryStatus];
export type BrandStatus = (typeof BrandStatus)[keyof typeof BrandStatus];
export type ProductStatus = (typeof ProductStatus)[keyof typeof ProductStatus];
export type ProductVariantStatus =
  (typeof ProductVariantStatus)[keyof typeof ProductVariantStatus];
