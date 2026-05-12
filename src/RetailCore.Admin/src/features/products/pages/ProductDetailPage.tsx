import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Alert, Button, Card, Chip, Separator, Spinner } from "@heroui/react";
import {
  AlertCircle,
  ArrowLeft,
  CheckCircle2,
  Layers3,
  Pencil,
  Settings2,
  Tag,
  Trash2,
} from "lucide-react";
import { toast } from "sonner";

import type { UUID } from "@/shared/types/common";
import { ProductStatus } from "@/shared/types/enums";

import { productApi } from "../api/product.api";
import { type ProductDetailDto } from "../types/types";

export default function ProductDetailPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: UUID }>();

  const [product, setProduct] = useState<ProductDetailDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isPublishing, setIsPublishing] = useState(false);

  useEffect(() => {
    const loadProduct = async () => {
      if (!id) return;

      try {
        setIsLoading(true);

        const data = await productApi.getById(id);

        setProduct(data);
      } catch (error) {
        console.error(error);
        toast.error("Failed to load product");
      } finally {
        setIsLoading(false);
      }
    };

    void loadProduct();
  }, [id, navigate]);

  const setupProgress = useMemo(() => {
    if (!product) return 0;

    const checks = [
      !!product.name,
      product.attributes.length > 0,
      product.variants.length > 0,
      product.variants.some((v) => v.images.length > 0),
    ];
    return Math.round((checks.filter(Boolean).length / checks.length) * 100);
  }, [product]);

  const totalStock = useMemo(() => {
    if (!product) return 0;
    return product.variants.reduce((sum, variant) => sum + variant.stock, 0);
  }, [product]);

  const priceRange = useMemo(() => {
    if (!product || product.variants.length === 0) {
      return null;
    }

    const prices = product.variants.map((v) => v.price);
    return {
      min: Math.min(...prices),
      max: Math.max(...prices),
    };
  }, [product]);

  const galleryImages = useMemo(() => {
    if (!product) return [];

    return product.variants
      .flatMap((v) => v.images)
      .sort((a, b) => a.sortOrder - b.sortOrder)
      .slice(0, 5);
  }, [product]);

  const refresh = async () => {
    if (!id) return;

    const data = await productApi.getById(id);
    setProduct(data);
  };

  const handlePublish = async () => {
    if (!product) return;

    try {
      setIsPublishing(true);

      await productApi.publish(product.id);
      toast.success("Product published");
      await refresh();
    } catch (error) {
      console.error(error);
      toast.error("Failed to publish product");
    } finally {
      setIsPublishing(false);
    }
  };

  const handleUnpublish = async () => {
    if (!product) return;

    try {
      await productApi.unpublish(product.id);
      toast.success("Product unpublished");
      await refresh();
    } catch (error) {
      console.error(error);
      toast.error("Failed to unpublish product");
    }
  };

  if (isLoading) {
    return (
      <div className="flex min-h-[60vh] items-center justify-center">
        <Spinner size="lg" />
      </div>
    );
  }

  if (!product) {
    return (
      <Alert color="danger" className="py-16 text-center">
        Product not found
      </Alert>
    );
  }

  const isActive = product.status === ProductStatus.Active;

  return (
    <div className="space-y-6">
      {/* HERO */}
      <div className="overflow-hidden rounded-3xl border border-default-200 bg-content1">
        <div className="flex flex-col gap-8 p-8 lg:flex-row lg:items-start lg:justify-between">
          {/* LEFT */}
          <div className="space-y-6">
            <button
              onClick={() => navigate("/admin/products")}
              className="flex items-center gap-2 text-sm text-default-500 transition hover:text-foreground"
            >
              <ArrowLeft className="size-4" />
              Back to products
            </button>

            <div className="space-y-4">
              <div className="flex flex-wrap items-center gap-3">
                <h1 className="text-4xl font-bold tracking-tight">
                  {product.name}
                </h1>

                <Chip
                  color={
                    product.status === ProductStatus.Active
                      ? "success"
                      : product.status === ProductStatus.Archived
                        ? "danger"
                        : "warning"
                  }
                  variant="secondary"
                  size="lg"
                >
                  {Object.entries(ProductStatus)[product.status][0]}
                </Chip>
              </div>

              <p className="max-w-3xl text-default-500">
                {product.shortDescription || "No short description provided."}
              </p>

              <div className="flex flex-wrap gap-3">
                <Chip variant="secondary">{product.brandName}</Chip>

                <Chip variant="secondary">{product.categoryName}</Chip>

                <Chip variant="secondary">
                  {product.variants.length} variants
                </Chip>

                <Chip variant="secondary">{totalStock} in stock</Chip>
              </div>
            </div>
          </div>

          {/* ACTIONS */}
          <div className="flex flex-wrap gap-3">
            <Button
              variant="primary"
              onPress={() => navigate(`/admin/products/${product.id}/setup`)}
            >
              {setupProgress === 100 ? (
                <>
                  <Pencil className="size-4" />
                  Edit
                </>
              ) : (
                <>
                  <Settings2 className="size-4" />
                  Complete Setup
                </>
              )}
            </Button>

            {isActive ? (
              <Button variant="danger-soft" onPress={handleUnpublish}>
                Unpublish
              </Button>
            ) : (
              <Button
                variant="primary"
                onPress={handlePublish}
                isPending={isPublishing}
              >
                Publish
              </Button>
            )}

            <Button variant="danger">
              <Trash2 className="size-4" />
              Delete
            </Button>
          </div>
        </div>
      </div>

      {/* MAIN GRID */}
      <div className="grid grid-cols-12 gap-6">
        {/* LEFT CONTENT */}
        <div className="col-span-12 space-y-6 lg:col-span-8">
          {/* GALLERY */}
          <Card className="overflow-hidden border border-default-200 p-6">
            <div className="mb-6 flex items-center justify-between">
              <div>
                <h2 className="text-xl font-semibold">Product Gallery</h2>

                <p className="text-sm text-default-500">
                  Variant images across the product
                </p>
              </div>

              <Chip variant="secondary">{galleryImages.length} images</Chip>
            </div>

            {galleryImages.length === 0 ? (
              <Alert color="warning">
                <AlertCircle className="size-4" />
                No images uploaded yet
              </Alert>
            ) : (
              <div className="grid grid-cols-2 gap-4 md:grid-cols-3 lg:grid-cols-4">
                {galleryImages.map((image) => (
                  <div
                    key={image.id}
                    className="group relative overflow-hidden rounded-2xl border border-default-200"
                  >
                    <img
                      src={image.url}
                      alt=""
                      className="aspect-square w-full object-cover transition duration-300 group-hover:scale-105"
                    />

                    {image.isPrimary && (
                      <div className="absolute left-3 top-3">
                        <Chip variant="primary" size="sm">
                          Primary
                        </Chip>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}
          </Card>

          {/* DESCRIPTION */}
          <Card className="border border-default-200 p-6">
            <div className="mb-6">
              <h2 className="text-xl font-semibold">Product Description</h2>

              <p className="text-sm text-default-500">
                Detailed product information
              </p>
            </div>

            <div className="space-y-6">
              <div>
                <p className="mb-2 text-sm font-semibold">Short Description</p>

                <p className="text-default-600">
                  {product.shortDescription || "-"}
                </p>
              </div>

              <Separator />

              <div>
                <p className="mb-2 text-sm font-semibold">Full Description</p>

                <p className="whitespace-pre-wrap leading-7 text-default-600">
                  {product.description || "-"}
                </p>
              </div>
            </div>
          </Card>

          {/* ATTRIBUTES */}
          <Card className="border border-default-200 p-6">
            <div className="mb-6 flex items-center gap-2">
              <Tag className="size-5 text-default-500" />

              <h2 className="text-xl font-semibold">Product Attributes</h2>
            </div>

            <div className="space-y-6">
              {product.attributes.map((attribute) => (
                <div key={attribute.id}>
                  <p className="mb-3 font-semibold">{attribute.name}</p>

                  <div className="flex flex-wrap gap-2">
                    {attribute.values.map((value) => (
                      <Chip key={value.id} variant="secondary">
                        {value.value}
                      </Chip>
                    ))}
                  </div>
                </div>
              ))}
            </div>
          </Card>

          {/* VARIANTS */}
          <Card className="border border-default-200 p-6">
            <div className="mb-6 flex items-center justify-between">
              <div className="flex items-center gap-2">
                <Layers3 className="size-5 text-default-500" />

                <h2 className="text-xl font-semibold">Product Variants</h2>
              </div>

              <Chip variant="secondary">
                {product.variants.length} variants
              </Chip>
            </div>

            <div className="space-y-4">
              {product.variants.map((variant) => (
                <div
                  key={variant.id}
                  className="rounded-2xl border border-default-200 p-5"
                >
                  <div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
                    <div className="space-y-3">
                      <div className="flex flex-wrap items-center gap-3">
                        <p className="font-mono text-sm font-semibold">
                          {variant.sku}
                        </p>

                        <Chip
                          size="sm"
                          color={
                            variant.status === ProductStatus.Active
                              ? "success"
                              : "default"
                          }
                          variant="secondary"
                        >
                          {Object.entries(ProductStatus)[variant.status][0]}
                        </Chip>
                      </div>

                      <div className="flex flex-wrap gap-2">
                        {variant.attributes.map((attr) => (
                          <Chip
                            key={attr.attributeValueId}
                            size="sm"
                            variant="secondary"
                          >
                            {attr.attributeName}: {attr.attributeValue}
                          </Chip>
                        ))}
                      </div>
                    </div>

                    <div className="flex gap-8 text-sm">
                      <div>
                        <p className="text-default-500">Price</p>

                        <p className="font-semibold">
                          ${variant.price.toFixed(2)}
                        </p>
                      </div>

                      <div>
                        <p className="text-default-500">Stock</p>

                        <p className="font-semibold">{variant.stock}</p>
                      </div>

                      <div>
                        <p className="text-default-500">Images</p>

                        <p className="font-semibold">{variant.images.length}</p>
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </Card>
        </div>

        {/* RIGHT SIDEBAR */}
        <div className="col-span-12 space-y-6 lg:col-span-4">
          {/* STATS */}
          <Card className="border border-default-200 p-6">
            <h2 className="mb-6 text-xl font-semibold">Product Overview</h2>

            <div className="grid grid-cols-2 gap-4">
              <StatsCard
                label="Variants"
                value={String(product.variants.length)}
              />

              <StatsCard label="Stock" value={String(totalStock)} />

              <StatsCard
                label="Min Price"
                value={priceRange ? `$${priceRange.min}` : "-"}
              />

              <StatsCard
                label="Max Price"
                value={priceRange ? `$${priceRange.max}` : "-"}
              />
            </div>
          </Card>

          {/* SETUP */}
          <Card className="border border-default-200 p-6">
            <div className="mb-6 flex items-center gap-2">
              <CheckCircle2 className="size-5 text-success" />

              <h2 className="text-xl font-semibold">Setup Progress</h2>
            </div>

            <div className="space-y-4">
              <div className="h-3 overflow-hidden rounded-full bg-default-100">
                <div
                  className="h-full bg-success transition-all"
                  style={{
                    width: `${setupProgress}%`,
                  }}
                />
              </div>

              <p className="text-sm text-default-500">
                {setupProgress}% completed
              </p>
            </div>
          </Card>

          {/* META */}
          <Card className="border border-default-200 p-6">
            <h2 className="mb-6 text-xl font-semibold">Metadata</h2>

            <div className="space-y-5 text-sm">
              <MetaItem label="Product ID" value={product.id} />

              <MetaItem label="Slug" value={product.slug} />

              <MetaItem
                label="Created"
                value={new Date(product.createdAt).toLocaleString()}
              />

              <MetaItem
                label="Updated"
                value={
                  product.updatedAt
                    ? new Date(product.updatedAt).toLocaleString()
                    : "-"
                }
              />
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
}

interface StatsCardProps {
  label: string;
  value: string;
}

function StatsCard({ label, value }: Readonly<StatsCardProps>) {
  return (
    <div className="rounded-2xl border border-default-200 p-4">
      <p className="text-sm text-default-500">{label}</p>

      <p className="mt-2 text-2xl font-bold">{value}</p>
    </div>
  );
}

interface MetaItemProps {
  label: string;
  value: string;
}

function MetaItem({ label, value }: Readonly<MetaItemProps>) {
  return (
    <div className="space-y-1">
      <p className="text-xs uppercase tracking-wide text-default-500">
        {label}
      </p>

      <p className="break-all font-medium">{value}</p>
    </div>
  );
}
