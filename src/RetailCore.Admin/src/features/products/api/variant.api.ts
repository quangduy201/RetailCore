import { http } from "@/shared/services/http";
import type { UUID } from "@/shared/types/common";
import type {
  ProductVariantDto,
  CreateProductVariantRequest,
  UpdateProductVariantRequest,
} from "../types/types";

const BASE_URL = "/admin/products";

export const variantsApi = {
  getByProductId: async (productId: UUID) => {
    const response = await http.get<ProductVariantDto[]>(
      `${BASE_URL}/${productId}/variants`,
    );

    return response.data;
  },

  getById: async (productId: UUID, variantId: UUID) => {
    const response = await http.get<ProductVariantDto>(
      `${BASE_URL}/${productId}/variants/${variantId}`,
    );

    return response.data;
  },

  create: async (productId: UUID, request: CreateProductVariantRequest) => {
    const response = await http.post<UUID>(
      `${BASE_URL}/${productId}/variants`,
      request,
    );

    return response.data;
  },

  update: async (
    productId: UUID,
    variantId: UUID,
    request: UpdateProductVariantRequest,
  ) => {
    await http.put(`${BASE_URL}/${productId}/variants/${variantId}`, request);
  },

  delete: async (productId: UUID, variantId: UUID) => {
    await http.delete(`${BASE_URL}/${productId}/variants/${variantId}`);
  },
};
