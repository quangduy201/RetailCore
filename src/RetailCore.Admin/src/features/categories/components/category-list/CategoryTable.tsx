import {
  Button,
  ListBox,
  Pagination,
  Select,
  Spinner,
  Table,
} from "@heroui/react";
import { Pencil, Trash2, PackageSearch } from "lucide-react";

import { getPageNumbers } from "@/shared/utils/pagination";
import { CategoryStatusBadge } from "../common/StatusBadge";
import {
  CATEGORY_TABLE_COLUMNS,
  type CategoryColumnKey,
} from "../../constants/categoryTableColumns";
import type { CategoryDetailDto } from "../../types";

const PAGE_SIZE_OPTIONS = [
  { id: "10", name: "10 / page" },
  { id: "20", name: "20 / page" },
  { id: "50", name: "50 / page" },
  { id: "100", name: "100 / page" },
];

interface CategoryTableProps {
  readonly categories: CategoryDetailDto[];

  readonly isLoading?: boolean;

  readonly totalCount: number;
  readonly totalPages: number;

  readonly pageNumber: number;
  readonly pageSize: number;

  readonly onPageChange: (page: number) => void;
  readonly onPageSizeChange: (size: number) => void;

  readonly onEdit: (categoryId: string) => void;
  readonly onDelete: (categoryId: string) => void;
}

export function CategoryTable({
  categories,
  isLoading = false,

  totalCount,
  totalPages,

  pageNumber,
  pageSize,

  onPageChange,
  onPageSizeChange,

  onEdit,
  onDelete,
}: Readonly<CategoryTableProps>) {
  const showingFrom = totalCount === 0 ? 0 : (pageNumber - 1) * pageSize + 1;

  const showingTo = Math.min(pageNumber * pageSize, totalCount);

  const renderCell = (
    category: CategoryDetailDto,
    columnKey: CategoryColumnKey,
  ) => {
    switch (columnKey) {
      case "name":
        return (
          <div className="flex flex-col gap-1">
            <span className="truncate font-semibold">{category.name}</span>

            <span className="truncate text-xs font-mono text-default-400">
              /{category.slug}
            </span>
          </div>
        );

      case "description":
        return (
          <p className="line-clamp-2 text-sm text-default-500">
            {category.description ?? "—"}
          </p>
        );

      case "status":
        return <CategoryStatusBadge status={category.status} size="md" />;

      case "createdAt":
        return new Date(category.createdAt).toLocaleString();

      case "updatedAt":
        return category.updatedAt
          ? new Date(category.updatedAt).toLocaleString()
          : "—";

      case "actions":
        return (
          <div className="flex flex-col items-stretch justify-end gap-2">
            <Button
              size="sm"
              variant="secondary"
              onPress={() => onEdit(category.id)}
            >
              <Pencil className="size-4" />
              Edit
            </Button>

            <Button
              size="sm"
              variant="danger-soft"
              onPress={() => onDelete(category.id)}
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
          aria-label="Categories table"
          className="max-h-175 overflow-auto"
        >
          <Table.Header columns={CATEGORY_TABLE_COLUMNS}>
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
            items={categories}
            renderEmptyState={() => {
              if (isLoading) {
                return (
                  <div className="flex flex-col items-center justify-center py-20">
                    <Spinner />

                    <p className="mt-4 text-sm text-default-500">
                      Loading categories...
                    </p>
                  </div>
                );
              }

              return (
                <div className="flex flex-col items-center justify-center py-20 text-center">
                  <PackageSearch className="size-14 text-default-300" />

                  <p className="mt-4 text-sm font-semibold text-default-600">
                    No categories found
                  </p>
                </div>
              );
            }}
          >
            {(category) => (
              <Table.Row id={category.id}>
                {CATEGORY_TABLE_COLUMNS.map((column) => (
                  <Table.Cell
                    key={column.key}
                    className={
                      column.key === "actions"
                        ? "sticky right-0 border-l backdrop-blur-xs"
                        : ""
                    }
                  >
                    {renderCell(category, column.key)}
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
            Showing {showingFrom} to {showingTo} of {totalCount} categories
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
