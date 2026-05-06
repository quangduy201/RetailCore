export const CategoryStatus = {
  Active: 0,
  Inactive: 1,
} as const;

export const BrandStatus = {
  Active: 0,
  Inactive: 1,
};

export const ProductStatus = {
  Draft: 0,
  Published: 1,
  Archived: 2,
} as const;

export const ProductVariantStatus = {
  Active: 0,
  Inactive: 1,
} as const;

export type CategoryStatus =
  (typeof CategoryStatus)[keyof typeof CategoryStatus];
export type BrandStatus = (typeof BrandStatus)[keyof typeof BrandStatus];
export type ProductStatus = (typeof ProductStatus)[keyof typeof ProductStatus];
export type ProductVariantStatus =
  (typeof ProductVariantStatus)[keyof typeof ProductVariantStatus];
