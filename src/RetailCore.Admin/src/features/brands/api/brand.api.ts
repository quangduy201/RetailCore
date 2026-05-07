import { http } from "@/shared/services/http";
import type { PagedResult, UUID } from "@/shared/types/common";
import type {
  GetBrandsRequest,
  BrandDetailDto,
  CreateBrandRequest,
  UpdateBrandRequest,
} from "../types";

const BASE_URL = "/admin/brands";

export const brandApi = {
  getPaged: async (params?: GetBrandsRequest) => {
    const res = await http.get<PagedResult<BrandDetailDto>>(BASE_URL, {
      params,
    });
    return res.data;
  },

  getById: async (id: UUID) => {
    const res = await http.get<BrandDetailDto>(`${BASE_URL}/${id}`);
    return res.data;
  },

  getBySlug: async (slug: string) => {
    const res = await http.get<BrandDetailDto>(`${BASE_URL}/slug/${slug}`);
    return res.data;
  },

  create: async (data: CreateBrandRequest) => {
    const res = await http.post<UUID>(BASE_URL, data);
    return res.data;
  },

  update: async (id: UUID, data: UpdateBrandRequest) => {
    await http.put(`${BASE_URL}/${id}`, data);
  },

  delete: async (id: UUID) => {
    await http.delete(`${BASE_URL}/${id}`);
  },
};
