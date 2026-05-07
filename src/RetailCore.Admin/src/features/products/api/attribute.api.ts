import { http } from "../../../shared/services/http";
import type { UUID } from "../../../shared/types/common";
import type {
  ProductAttributeDto,
  CreateProductAttributeRequest,
  UpdateProductAttributeRequest,
} from "../types";

const BASE_URL = "/admin/products";

export const productAttributesApi = {
  getByProductId: async (productId: UUID) => {
    const response = await http.get<ProductAttributeDto[]>(
      `${BASE_URL}/${productId}/attributes`,
    );

    return response.data;
  },

  getById: async (productId: UUID, attributeId: UUID) => {
    const response = await http.get<ProductAttributeDto>(
      `${BASE_URL}/${productId}/attributes/${attributeId}`,
    );

    return response.data;
  },

  create: async (productId: UUID, request: CreateProductAttributeRequest) => {
    const response = await http.post<UUID>(
      `${BASE_URL}/${productId}/attributes`,
      request,
    );

    return response.data;
  },

  update: async (
    productId: UUID,
    attributeId: UUID,
    request: UpdateProductAttributeRequest,
  ) => {
    await http.put(
      `${BASE_URL}/${productId}/attributes/${attributeId}`,
      request,
    );
  },

  delete: async (productId: UUID, attributeId: UUID) => {
    await http.delete(`${BASE_URL}/${productId}/attributes/${attributeId}`);
  },
};
