import { http } from "@/shared/services/http";
import type { PagedResult, UUID } from "@/shared/types/common";
import type {
  GetCategoriesRequest,
  CategoryDetailDto,
  CreateCategoryRequest,
  UpdateCategoryRequest,
} from "../types";

const BASE_URL = "/admin/categories";

export const categoryApi = {
  getPaged: async (params?: GetCategoriesRequest) => {
    const res = await http.get<PagedResult<CategoryDetailDto>>(BASE_URL, {
      params,
    });
    return res.data;
  },

  getById: async (id: UUID) => {
    const res = await http.get<CategoryDetailDto>(`${BASE_URL}/${id}`);
    return res.data;
  },

  create: async (data: CreateCategoryRequest) => {
    const res = await http.post<UUID>(BASE_URL, data);
    return res.data;
  },

  update: async (id: UUID, data: UpdateCategoryRequest) => {
    await http.put(`${BASE_URL}/${id}`, data);
  },

  delete: async (id: UUID) => {
    await http.delete(`${BASE_URL}/${id}`);
  },
};
