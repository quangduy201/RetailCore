export type BrandColumnKey =
  | "name"
  | "description"
  | "status"
  | "createdAt"
  | "updatedAt"
  | "actions";

export interface BrandTableColumn {
  key: BrandColumnKey;
  label: string;
  defaultWidth: number;
  minWidth: number;
}

export const BRAND_TABLE_COLUMNS: BrandTableColumn[] = [
  {
    key: "name",
    label: "Name",
    defaultWidth: 200,
    minWidth: 180,
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
    defaultWidth: 120,
    minWidth: 120,
  },
];
