import { http } from "@/shared/services/http";

import type { UserDto } from "../types";

const BASE_URL = "/admin/users";

export const customerApi = {
  async getCustomers(): Promise<UserDto[]> {
    const response = await http.get<UserDto[]>(BASE_URL);

    return response.data;
  },

  async getById(id: UserDto["id"]): Promise<void> {
    await http.get(`${BASE_URL}/${id}`);
  },

  async toggleActiveStatus(id: UserDto["id"]): Promise<void> {
    await http.patch(`${BASE_URL}/${id}/toggle-active`);
  },
};
