import {
  Button,
  ListBox,
  Pagination,
  Select,
  Spinner,
  Table,
} from "@heroui/react";
import {
  Image as ImageIcon,
  Pencil,
  Trash2,
  PackageSearch,
} from "lucide-react";

import { getPageNumbers } from "@/shared/utils/pagination";
import { ProductStatusBadge } from "../common/StatusBadges";
import {
  PRODUCT_TABLE_COLUMNS,
  type ProductColumnKey,
} from "../../constants/productTableColumns";
import type { ProductManagementDto } from "../../types";
import { formatPrice } from "../../utils/priceFormatter";

const PAGE_SIZE_OPTIONS = [
  { id: "10", name: "10 / page" },
  { id: "20", name: "20 / page" },
  { id: "50", name: "50 / page" },
  { id: "100", name: "100 / page" },
];

interface ProductTableProps {
  readonly products: ProductManagementDto[];

  readonly isLoading?: boolean;

  readonly totalCount: number;
  readonly totalPages: number;

  readonly pageNumber: number;
  readonly pageSize: number;

  readonly onPageChange: (page: number) => void;
  readonly onPageSizeChange: (size: number) => void;

  readonly onEdit: (productId: string) => void;
  readonly onDelete: (productId: string) => void;
}

export function ProductTable({
  products,
  isLoading = false,

  totalCount,
  totalPages,

  pageNumber,
  pageSize,

  onPageChange,
  onPageSizeChange,

  onEdit,
  onDelete,
}: Readonly<ProductTableProps>) {
  const showingFrom = totalCount === 0 ? 0 : (pageNumber - 1) * pageSize + 1;

  const showingTo = Math.min(pageNumber * pageSize, totalCount);

  const renderCell = (
    product: ProductManagementDto,
    columnKey: ProductColumnKey,
  ) => {
    switch (columnKey) {
      case "thumbnail":
        return (
          <div className="flex items-center justify-center">
            {product.thumbnailUrl ? (
              <img
                src={product.thumbnailUrl}
                alt={product.name}
                className="aspect-square w-full max-w-20 rounded-xl border border-default-200 object-cover"
              />
            ) : (
              <div className="flex w-full max-w-20 items-center justify-center rounded-xl border border-dashed border-default-300 bg-default-100">
                <ImageIcon className="size-5 text-default-400" />
              </div>
            )}
          </div>
        );

      case "name":
        return (
          <div className="flex flex-col gap-1">
            <span className="truncate font-semibold">{product.name}</span>

            <span className="truncate text-xs font-mono text-default-400">
              /{product.slug}
            </span>
          </div>
        );

      case "shortDescription":
        return (
          <p className="line-clamp-2 text-sm text-default-500">
            {product.shortDescription ?? "—"}
          </p>
        );

      case "description":
        return (
          <p className="line-clamp-2 text-sm text-default-500">
            {product.description ?? "—"}
          </p>
        );

      case "brandName":
        return (
          <p className="truncate text-sm text-default-500">
            {product.brandName}
          </p>
        );

      case "categoryName":
        return (
          <p className="truncate text-sm text-default-500">
            {product.categoryName}
          </p>
        );

      case "status":
        return <ProductStatusBadge status={product.status} size="md" />;

      case "variantCount":
        return product.variantCount;

      case "stock":
        return (
          <span
            className={product.stock <= 10 ? "font-semibold text-warning" : ""}
          >
            {product.stock}
          </span>
        );

      case "priceRange":
        if (product.minPrice === null || product.minPrice === undefined) {
          return "—";
        }

        return (
          <div className="flex flex-col">
            <span className="font-medium">{formatPrice(product.minPrice)}</span>

            {product.maxPrice !== null &&
              product.maxPrice !== undefined &&
              product.minPrice !== product.maxPrice && (
                <span className="text-xs text-default-500">
                  to {formatPrice(product.maxPrice)}
                </span>
              )}
          </div>
        );

      case "createdAt":
        return new Date(product.createdAt).toLocaleString();

      case "updatedAt":
        return product.updatedAt
          ? new Date(product.updatedAt).toLocaleString()
          : "—";

      case "actions":
        return (
          <div className="flex flex-col items-stretch justify-end gap-2">
            <Button
              size="sm"
              variant="secondary"
              onPress={() => onEdit(product.id)}
            >
              <Pencil className="size-4" />
              Edit
            </Button>

            <Button
              size="sm"
              variant="danger-soft"
              onPress={() => onDelete(product.id)}
            >
              <Trash2 className="size-4" />
              Delete
            </Button>
          </div>
        );

      default:
        return "—";
    }
  };

  return (
    <Table variant="primary" className="border border-default">
      <Table.ResizableContainer>
        <Table.Content
          aria-label="Products table"
          className="max-h-175 overflow-auto"
        >
          <Table.Header columns={PRODUCT_TABLE_COLUMNS}>
            {(column) => (
              <Table.Column
                id={column.key}
                isRowHeader={column.key === "name"}
                defaultWidth={column.defaultWidth}
                minWidth={column.minWidth}
                className={`border-b border-default-200 bg-default ${column.key === "actions" ? "sticky right-0" : ""}`}
              >
                <div className="flex items-center gap-2">
                  <span>{column.label}</span>

                  <Table.ColumnResizer />
                </div>
              </Table.Column>
            )}
          </Table.Header>

          <Table.Body
            items={products}
            renderEmptyState={() => {
              if (isLoading) {
                return (
                  <div className="flex flex-col items-center justify-center py-20">
                    <Spinner />

                    <p className="mt-4 text-sm text-default-500">
                      Loading products...
                    </p>
                  </div>
                );
              }

              return (
                <div className="flex flex-col items-center justify-center py-20 text-center">
                  <PackageSearch className="size-14 text-default-300" />

                  <p className="mt-4 text-sm font-semibold text-default-600">
                    No products found
                  </p>
                </div>
              );
            }}
          >
            {(product) => (
              <Table.Row id={product.id}>
                {PRODUCT_TABLE_COLUMNS.map((column) => (
                  <Table.Cell
                    key={column.key}
                    className={
                      column.key === "actions"
                        ? "sticky right-0 border-l backdrop-blur-xs"
                        : ""
                    }
                  >
                    {renderCell(product, column.key)}
                  </Table.Cell>
                ))}
              </Table.Row>
            )}
          </Table.Body>
        </Table.Content>
      </Table.ResizableContainer>

      <Table.Footer className="border-t border-default-200 px-6 py-4">
        {/* Right */}
        <Pagination className="flex w-full flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <Pagination.Summary className="text-sm text-default-500">
            Showing {showingFrom} to {showingTo} of {totalCount} products
          </Pagination.Summary>

          <Pagination.Content>
            {/* Previous */}
            <Pagination.Item>
              <Pagination.Previous
                isDisabled={pageNumber <= 1}
                onPress={() => onPageChange(pageNumber - 1)}
              >
                <Pagination.PreviousIcon />
              </Pagination.Previous>
            </Pagination.Item>

            {/* Pages */}
            {getPageNumbers(pageNumber, totalPages).map((page, index) =>
              page === "ellipsis" ? (
                <Pagination.Item key={`ellipsis-${index}`}>
                  <Pagination.Ellipsis />
                </Pagination.Item>
              ) : (
                <Pagination.Item key={page}>
                  <Pagination.Link
                    isActive={page === pageNumber}
                    onPress={() => onPageChange(page)}
                  >
                    {page}
                  </Pagination.Link>
                </Pagination.Item>
              ),
            )}

            {/* Next */}
            <Pagination.Item>
              <Pagination.Next
                isDisabled={pageNumber >= totalPages}
                onPress={() => onPageChange(pageNumber + 1)}
              >
                <Pagination.NextIcon />
              </Pagination.Next>
            </Pagination.Item>
          </Pagination.Content>

          <Select
            value={String(pageSize)}
            onChange={(key) => {
              if (!key) {
                return;
              }

              onPageSizeChange(Number(key));
            }}
            className="min-w-30"
          >
            <Select.Trigger>
              <Select.Value className="text-center" />
              <Select.Indicator />
            </Select.Trigger>

            <Select.Popover>
              <ListBox aria-label="Page size options">
                {PAGE_SIZE_OPTIONS.map((option) => (
                  <ListBox.Item
                    key={option.id}
                    id={option.id}
                    textValue={option.id}
                  >
                    {option.name}
                  </ListBox.Item>
                ))}
              </ListBox>
            </Select.Popover>
          </Select>
        </Pagination>
      </Table.Footer>
    </Table>
  );
}
