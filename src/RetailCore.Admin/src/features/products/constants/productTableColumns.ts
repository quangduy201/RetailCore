export type ProductColumnKey =
  | "thumbnail"
  | "name"
  | "brandName"
  | "categoryName"
  | "shortDescription"
  | "description"
  | "status"
  | "variantCount"
  | "stock"
  | "priceRange"
  | "createdAt"
  | "updatedAt"
  | "actions";

export interface ProductTableColumn {
  key: ProductColumnKey;
  label: string;
  defaultWidth: number;
  minWidth: number;
}

export const PRODUCT_TABLE_COLUMNS: ProductTableColumn[] = [
  {
    key: "thumbnail",
    label: "Thumbnail",
    defaultWidth: 80,
    minWidth: 80,
  },
  {
    key: "name",
    label: "Name",
    defaultWidth: 200,
    minWidth: 180,
  },
  {
    key: "brandName",
    label: "Brand",
    defaultWidth: 100,
    minWidth: 100,
  },
  {
    key: "categoryName",
    label: "Category",
    defaultWidth: 120,
    minWidth: 120,
  },
  {
    key: "shortDescription",
    label: "Short Description",
    defaultWidth: 240,
    minWidth: 240,
  },
  {
    key: "description",
    label: "Description",
    defaultWidth: 320,
    minWidth: 320,
  },
  {
    key: "status",
    label: "Status",
    defaultWidth: 100,
    minWidth: 100,
  },
  {
    key: "variantCount",
    label: "Variants",
    defaultWidth: 100,
    minWidth: 100,
  },
  {
    key: "stock",
    label: "Stock",
    defaultWidth: 100,
    minWidth: 100,
  },
  {
    key: "priceRange",
    label: "Price Range",
    defaultWidth: 150,
    minWidth: 150,
  },
  {
    key: "createdAt",
    label: "Created At",
    defaultWidth: 180,
    minWidth: 180,
  },
  {
    key: "updatedAt",
    label: "Updated At",
    defaultWidth: 180,
    minWidth: 180,
  },
  {
    key: "actions",
    label: "Actions",
    defaultWidth: 80,
    minWidth: 80,
  },
];
