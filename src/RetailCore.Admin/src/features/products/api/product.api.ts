import { http } from "../../../shared/services/http";
import type { PagedResult, UUID } from "../../../shared/types/common";
import type {
  CreateProductRequest,
  GetProductsRequest,
  ProductDetailDto,
  ProductManagementDto,
  UpdateProductRequest,
} from "../types";

const BASE_URL = "/admin/products";

export const productApi = {
  getPaged: async (params?: GetProductsRequest) => {
    const res = await http.get<PagedResult<ProductManagementDto>>(BASE_URL, {
      params,
    });
    return res.data;
  },

  getById: async (id: UUID) => {
    const res = await http.get<ProductDetailDto>(`${BASE_URL}/${id}`);
    return res.data;
  },

  create: async (data: CreateProductRequest) => {
    const res = await http.post<UUID>(BASE_URL, data);
    return res.data;
  },

  update: async (id: UUID, data: UpdateProductRequest) => {
    await http.put(`${BASE_URL}/${id}`, data);
  },

  delete: async (id: UUID) => {
    await http.delete(`${BASE_URL}/${id}`);
  },
};
