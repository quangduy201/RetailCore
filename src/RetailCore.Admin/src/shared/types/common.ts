declare const uuidBrand: unique symbol;
declare const dateTimeBrand: unique symbol;

export type UUID = string & { readonly [uuidBrand]: true };
export type DateTime = string & { readonly [dateTimeBrand]: true };

export type PagedResult<T> = {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

export type PaginationRequest = {
  pageNumber?: number;
  pageSize?: number;
};
