import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Button, Card, Chip, SearchField } from "@heroui/react";
import { AlertCircle, Plus } from "lucide-react";

import { productApi } from "../api/product.api";
import { ProductTable } from "../components/product-list/ProductTable";
import type { ProductManagementDto } from "../types";

import type { UUID } from "@/shared/types/common";

export default function ProductsPage() {
  const navigate = useNavigate();
  const [products, setProducts] = useState<ProductManagementDto[]>([]);

  const [isLoading, setIsLoading] = useState(false);

  const [error, setError] = useState<string | null>(null);

  const [pageNumber, setPageNumber] = useState(1);

  const [pageSize, setPageSize] = useState(10);

  const [totalPages, setTotalPages] = useState(1);

  const [totalCount, setTotalCount] = useState(0);

  const [searchTerm, setSearchTerm] = useState("");

  const loadProducts = useCallback(async () => {
    try {
      setIsLoading(true);

      setError(null);

      const response = await productApi.getPaged({
        pageNumber,
        pageSize,
        keyword: searchTerm || undefined,
      });

      setProducts(response.items);

      setTotalPages(response.totalPages);

      setTotalCount(response.totalCount);
    } catch {
      setError("Failed to load products.");
    } finally {
      setIsLoading(false);
    }
  }, [pageNumber, pageSize, searchTerm]);

  useEffect(() => {
    void loadProducts();
  }, [loadProducts]);

  const handleDeleteProduct = async (productId: string) => {
    const confirmed = globalThis.confirm("Delete this product?");

    if (!confirmed) {
      return;
    }

    try {
      await productApi.delete(productId as UUID);

      await loadProducts();
    } catch {
      setError("Failed to delete product.");
    }
  };

  const handleCreateProduct = () => {
    navigate("/products/create");
  };

  const handleEditProduct = (productId: string) => {
    navigate(`/products/${productId}/edit`);
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <Card className="overflow-hidden border border-default-200">
        <div className="flex flex-col gap-6 bg-linear-to-r from-default-50 to-default-100 px-6 py-6 lg:flex-row lg:items-center lg:justify-between">
          {/* Left */}
          <div className="space-y-3">
            <Chip variant="secondary">Catalog Management</Chip>

            <div>
              <h1 className="text-3xl font-bold tracking-tight">Products</h1>

              <p className="mt-1 text-sm text-default-500">
                {totalCount} products in your catalog
              </p>
            </div>
          </div>

          {/* Right */}
          <div className="flex flex-col gap-3 sm:flex-row">
            {/* Search */}
            <div className="min-w-72">
              <SearchField>
                <SearchField.Group>
                  <SearchField.SearchIcon />

                  <SearchField.Input
                    placeholder="Search products..."
                    value={searchTerm}
                    onChange={(e) => {
                      setSearchTerm(e.target.value);

                      setPageNumber(1);
                    }}
                  />

                  <SearchField.ClearButton />
                </SearchField.Group>
              </SearchField>
            </div>

            {/* Create */}
            <Button variant="primary" onPress={handleCreateProduct}>
              <Plus className="size-4" />
              Create Product
            </Button>
          </div>
        </div>
      </Card>

      {/* Error */}
      {error && (
        <Card className="border border-danger-200 bg-danger-50 p-4">
          <div className="flex items-start gap-3">
            <AlertCircle className="mt-0.5 size-5 text-danger" />

            <div>
              <p className="font-semibold text-danger">
                Failed to load products
              </p>

              <p className="text-sm text-danger-700">{error}</p>
            </div>
          </div>
        </Card>
      )}

      {/* Table */}
      <ProductTable
        products={products}
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
        onEdit={handleEditProduct}
        onDelete={handleDeleteProduct}
      />
    </div>
  );
}
