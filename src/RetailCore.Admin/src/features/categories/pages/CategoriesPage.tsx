import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Button, Card, Chip, SearchField } from "@heroui/react";
import { AlertCircle, Plus } from "lucide-react";

import { categoryApi } from "../api/category.api";
import { CategoryTable } from "../components/category-list/CategoryTable";
import type { CategoryDetailDto } from "../types";

import type { UUID } from "@/shared/types/common";

export default function CategoriesPage() {
  const navigate = useNavigate();

  const [categories, setCategories] = useState<CategoryDetailDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  const [searchTerm, setSearchTerm] = useState("");

  const loadCategories = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await categoryApi.getPaged({
        pageNumber,
        pageSize,
        keyword: searchTerm || undefined,
      });

      setCategories(response.items);
      setTotalPages(response.totalPages);
      setTotalCount(response.totalCount);
    } catch {
      setError("Failed to load categories.");
    } finally {
      setIsLoading(false);
    }
  }, [pageNumber, pageSize, searchTerm]);

  useEffect(() => {
    void loadCategories();
  }, [loadCategories]);

  const handleCreate = () => navigate("/categories/create");
  const handleEdit = (id: string) => navigate(`/categories/${id}/edit`);

  const handleDelete = async (id: string) => {
    if (!globalThis.confirm("Delete this category?")) return;

    try {
      await categoryApi.delete(id as UUID);
      await loadCategories();
    } catch {
      setError("Failed to delete category.");
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <Card className="overflow-hidden border border-default-200">
        <div className="flex flex-col gap-6 bg-linear-to-r from-default-50 to-default-100 px-6 py-6 lg:flex-row lg:items-center lg:justify-between">
          <div className="space-y-3">
            <Chip variant="secondary">Catalog Management</Chip>

            <div>
              <h1 className="text-3xl font-bold tracking-tight">Categories</h1>
              <p className="mt-1 text-sm text-default-500">
                {totalCount} categories
              </p>
            </div>
          </div>

          <div className="flex flex-col gap-3 sm:flex-row">
            <div className="min-w-72">
              <SearchField
                value={searchTerm}
                onChange={(value) => {
                  setSearchTerm(value);
                  setPageNumber(1);
                }}
              >
                <SearchField.Group>
                  <SearchField.SearchIcon />
                  <SearchField.Input placeholder="Search categories..." />
                  <SearchField.ClearButton />
                </SearchField.Group>
              </SearchField>
            </div>

            <Button variant="primary" onPress={handleCreate}>
              <Plus className="size-4" />
              Create Category
            </Button>
          </div>
        </div>
      </Card>

      {/* Error */}
      {error && (
        <Card className="border border-danger-200 bg-danger-50 p-4">
          <div className="flex gap-3">
            <AlertCircle className="size-5 text-danger" />
            <div>
              <p className="font-semibold text-danger">Error</p>
              <p className="text-sm text-danger-700">{error}</p>
            </div>
          </div>
        </Card>
      )}

      {/* Table */}
      <CategoryTable
        categories={categories}
        isLoading={isLoading}
        totalCount={totalCount}
        totalPages={totalPages}
        pageNumber={pageNumber}
        pageSize={pageSize}
        onPageChange={setPageNumber}
        onPageSizeChange={(size) => {
          setPageSize(size);
          setPageNumber(1);
        }}
        onEdit={handleEdit}
        onDelete={handleDelete}
      />
    </div>
  );
}
