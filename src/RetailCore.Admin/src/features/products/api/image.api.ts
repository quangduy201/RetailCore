import axiosInstance from "../../../shared/services/http";
import {
  type ProductVariantImage,
  type ProductVariantImageCreateDto,
  type ProductVariantImageUpdateDto,
} from "../../../shared/types/image";

const RESOURCE = "/product-variant-images";

export const imageApi = {
  getById: async (id: string): Promise<ProductVariantImage> => {
    const response = await axiosInstance.get<ProductVariantImage>(
      `${RESOURCE}/${id}`,
    );
    return response.data;
  },

  getByVariantId: async (variantId: string): Promise<ProductVariantImage[]> => {
    const response = await axiosInstance.get<ProductVariantImage[]>(
      `${RESOURCE}/by-variant/${variantId}`,
    );
    return response.data;
  },

  create: async (data: ProductVariantImageCreateDto): Promise<string> => {
    const response = await axiosInstance.post<{ id: string }>(RESOURCE, data);
    return response.data.id;
  },

  update: async (
    id: string,
    data: ProductVariantImageUpdateDto,
  ): Promise<void> => {
    await axiosInstance.put(`${RESOURCE}/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await axiosInstance.delete(`${RESOURCE}/${id}`);
  },

  setPrimary: async (variantId: string, imageId: string): Promise<void> => {
    await axiosInstance.post(`${RESOURCE}/${variantId}/set-primary/${imageId}`);
  },
};
