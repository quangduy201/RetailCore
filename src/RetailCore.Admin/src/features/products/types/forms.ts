import type { UUID } from "@/shared/types/common";
import type { ProductVariantStatus } from "@/shared/types/enums";

export interface ProductBasicInfoForm {
  name: string;
  slug: string;
  shortDescription: string;
  description: string;
  brandId: string;
  categoryId: string;
}

export interface AttributeFormValue {
  id: string;
  name: string;
  values: {
    id: string;
    value: string;
  }[];
}

export interface VariantFormValue {
  id: UUID | null;
  tempId: string;
  sku: string;
  attributeCombination: Record<string, string>;
  attributeValueIds: UUID[];
  price: number;
  stock: number;
  status: ProductVariantStatus;
  isSelected: boolean;
  images: VariantImageForm[];
}

export interface VariantImageForm {
  id: UUID | null;
  url: string;
  publicId?: string;
  sortOrder: number;
  isPrimary: boolean;
  file?: File;
  previewUrl?: string;
  isUploading?: boolean;
}
