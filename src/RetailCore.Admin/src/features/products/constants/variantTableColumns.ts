export type VariantColumnKey =
  | "sku"
  | "attributes"
  | "price"
  | "stock"
  | "status"
  | "active";

export interface VariantTableColumn {
  key: VariantColumnKey;
  label: string;
  defaultWidth: number;
  minWidth: number;
}

export const VARIANT_TABLE_COLUMNS: VariantTableColumn[] = [
  {
    key: "sku",
    label: "SKU",
    defaultWidth: 250,
    minWidth: 250,
  },
  {
    key: "attributes",
    label: "Attributes",
    defaultWidth: 180,
    minWidth: 180,
  },
  {
    key: "price",
    label: "Price",
    defaultWidth: 120,
    minWidth: 120,
  },
  {
    key: "stock",
    label: "Stock",
    defaultWidth: 120,
    minWidth: 120,
  },
  {
    key: "status",
    label: "Status",
    defaultWidth: 150,
    minWidth: 150,
  },
  {
    key: "active",
    label: "Active",
    defaultWidth: 80,
    minWidth: 80,
  },
];
