import { http } from "@/shared/services/http";
import type { UUID } from "@/shared/types/common";
import type {
  ProductAttributeValueDto,
  CreateProductAttributeValueRequest,
  UpdateProductAttributeValueRequest,
} from "../types";

const BASE_URL = "/admin/products";

export const productAttributeValuesApi = {
  getByAttributeId: async (productId: UUID, attributeId: UUID) => {
    const response = await http.get<ProductAttributeValueDto[]>(
      `${BASE_URL}/${productId}/attributes/${attributeId}/values`,
    );

    return response.data;
  },

  create: async (
    productId: UUID,
    attributeId: UUID,
    request: CreateProductAttributeValueRequest,
  ) => {
    const response = await http.post<UUID>(
      `${BASE_URL}/${productId}/attributes/${attributeId}/values`,
      request,
    );

    return response.data;
  },

  update: async (
    productId: UUID,
    attributeId: UUID,
    valueId: UUID,
    request: UpdateProductAttributeValueRequest,
  ) => {
    await http.put(
      `${BASE_URL}/${productId}/attributes/${attributeId}/values/${valueId}`,
      request,
    );
  },

  delete: async (productId: UUID, attributeId: UUID, valueId: UUID) => {
    await http.delete(
      `${BASE_URL}/${productId}/attributes/${attributeId}/values/${valueId}`,
    );
  },
};
