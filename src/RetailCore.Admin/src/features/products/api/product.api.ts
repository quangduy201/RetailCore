import { http } from "@/shared/services/http";
import type { PagedResult, UUID } from "@/shared/types/common";
import type {
  CreateProductRequest,
  GetProductsRequest,
  ProductDetailDto,
  ProductManagementDto,
  UpdateProductRequest,
} from "../types/types";

const BASE_URL = "/admin/products";

export const productApi = {
  getPaged: async (
    params?: GetProductsRequest,
  ): Promise<PagedResult<ProductManagementDto>> => {
    const res = await http.get<PagedResult<ProductManagementDto>>(BASE_URL, {
      params,
    });
    return res.data;
  },

  getById: async (id: UUID): Promise<ProductDetailDto> => {
    const res = await http.get<ProductDetailDto>(`${BASE_URL}/${id}`);
    return res.data;
  },

  create: async (data: CreateProductRequest): Promise<UUID> => {
    const res = await http.post<UUID>(BASE_URL, data);
    return res.data;
  },

  update: async (id: UUID, data: UpdateProductRequest): Promise<void> => {
    await http.put(`${BASE_URL}/${id}`, data);
  },

  delete: async (id: UUID): Promise<void> => {
    await http.delete(`${BASE_URL}/${id}`);
  },

  publish: async (id: UUID): Promise<void> => {
    await http.post(`${BASE_URL}/${id}/publish`);
  },

  unpublish: async (id: UUID): Promise<void> => {
    await http.post(`${BASE_URL}/${id}/unpublish`);
  },

  archive: async (id: UUID): Promise<void> => {
    await http.post(`${BASE_URL}/${id}/archive`);
  },

  restore: async (id: UUID): Promise<void> => {
    await http.post(`${BASE_URL}/${id}/restore`);
  },
};
