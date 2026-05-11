import { useState, useMemo, useEffect } from "react";
import { Alert, Button, Card, Chip, ScrollShadow } from "@heroui/react";
import { AlertCircle, ImagePlus } from "lucide-react";
import { toast } from "sonner";
import {
  closestCenter,
  DndContext,
  PointerSensor,
  useSensor,
  useSensors,
  type DragEndEvent,
} from "@dnd-kit/core";
import {
  arrayMove,
  SortableContext,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";

import { variantsApi } from "@/features/products/api/variant.api";
import type { VariantImageForm } from "@/features/products/types/forms";
import type { ProductDetailDto } from "@/features/products/types/types";

import { SortableImageCard } from "../../common/SortableImageCard";

interface Step3ImagesProps {
  product?: ProductDetailDto | null;
  onBack: () => void;
  onCompleted: () => void;
}

export function Step3Images({
  product,
  onBack,
  onCompleted,
}: Readonly<Step3ImagesProps>) {
  const [selectedVariantId, setSelectedVariantId] = useState<string | null>(
    null,
  );

  const [imagesMap, setImagesMap] = useState<
    Record<string, VariantImageForm[]>
  >({});

  const [isSaving, setIsSaving] = useState(false);

  const variants = useMemo(() => product?.variants ?? [], [product]);

  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 5,
      },
    }),
  );

  useEffect(() => {
    if (variants.length === 0) return;

    setSelectedVariantId(variants[0].id);

    const mapped: Record<string, VariantImageForm[]> = {};

    variants.forEach((variant) => {
      mapped[variant.id] = variant.images.map((image) => ({
        id: image.id,
        url: image.url,
        sortOrder: image.sortOrder,
        isPrimary: image.isPrimary,
      }));
    });

    setImagesMap(mapped);
  }, [variants]);

  const selectedVariant = useMemo(
    () => variants.find((variant) => variant.id === selectedVariantId) ?? null,
    [variants, selectedVariantId],
  );

  const selectedImages = selectedVariantId
    ? (imagesMap[selectedVariantId] ?? [])
    : [];

  const updateImages = (images: VariantImageForm[]) => {
    if (!selectedVariantId) return;

    setImagesMap((prev) => ({
      ...prev,
      [selectedVariantId]: images,
    }));
  };

  const addImage = () => {
    updateImages([
      ...selectedImages,
      {
        id: null,
        url: "",
        sortOrder: selectedImages.length + 1,
        isPrimary: selectedImages.length === 0,
      },
    ]);
  };

  const removeImage = (index: number) => {
    const updated = selectedImages.filter((_, i) => i !== index);

    if (!updated.some((x) => x.isPrimary) && updated.length > 0) {
      updated[0].isPrimary = true;
    }

    updateImages(updated);
  };

  const updateImage = (index: number, updates: Partial<VariantImageForm>) => {
    const updated = [...selectedImages];

    updated[index] = {
      ...updated[index],
      ...updates,
    };

    if (updates.isPrimary) {
      updated.forEach((img, i) => {
        img.isPrimary = i === index;
      });
    }

    updateImages(updated);
  };

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;

    if (!over || active.id === over.id) {
      return;
    }

    const oldIndex = selectedImages.findIndex(
      (image, index) =>
        (image.id ?? `${selectedVariantId}-${index}`) === active.id,
    );

    const newIndex = selectedImages.findIndex(
      (image, index) =>
        (image.id ?? `${selectedVariantId}-${index}`) === over.id,
    );

    if (oldIndex === -1 || newIndex === -1) {
      return;
    }

    const reordered = arrayMove(selectedImages, oldIndex, newIndex).map(
      (image, index) => ({
        ...image,
        sortOrder: index + 1,
      }),
    );

    updateImages(reordered);
  };

  const handleSave = async () => {
    if (!product) return;

    try {
      setIsSaving(true);

      await Promise.all(
        variants.map(async (variant) => {
          const images = imagesMap[variant.id] ?? [];

          await variantsApi.update(product.id, variant.id, {
            sku: variant.sku,
            name: variant.name ?? "",
            description: variant.description ?? "",
            price: variant.price,
            compareAtPrice: variant.compareAtPrice ?? 0,
            stock: variant.stock,
            status: variant.status,
            attributeValueIds: variant.attributes.map(
              (x) => x.attributeValueId,
            ),
            images,
          });
        }),
      );

      toast.success("Images saved");

      onCompleted();
    } catch (error) {
      console.error(error);
      toast.error("Failed to save images");
    } finally {
      setIsSaving(false);
    }
  };

  if (!product || variants.length === 0) {
    return (
      <Card className="border border-default-200 p-6">
        <Alert color="secondary">
          <AlertCircle className="size-4" />

          <p className="text-sm">
            No variants found. Please create variants first.
          </p>
        </Alert>
      </Card>
    );
  }

  return (
    <Card className="border border-default-200 p-6">
      <div className="space-y-6">
        <div>
          <h3 className="mb-2 font-semibold">Variant Images</h3>

          <p className="text-sm text-default-500">
            Upload and manage images for each variant.
          </p>
        </div>

        <div className="grid grid-cols-12 gap-6">
          <div className="col-span-5">
            <div className="rounded-xl border border-default-200">
              <div className="border-b border-default-200 p-4">
                <h4 className="font-medium">Variants</h4>
              </div>

              <ScrollShadow className="max-h-150">
                <div className="space-y-2 p-2">
                  {variants.map((variant) => {
                    const isSelected = variant.id === selectedVariantId;

                    return (
                      <button
                        key={variant.id}
                        type="button"
                        onClick={() => setSelectedVariantId(variant.id)}
                        className={`w-full rounded-xl border p-3 text-left transition ${
                          isSelected
                            ? "border-accent bg-accent/10"
                            : "border-default-200 hover:bg-default-100"
                        }`}
                      >
                        <div className="space-y-2">
                          <div className="font-medium">{variant.sku}</div>

                          <div className="flex flex-wrap gap-1">
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

                          <div className="text-xs text-default-500">
                            {(imagesMap[variant.id] ?? []).length} images
                          </div>
                        </div>
                      </button>
                    );
                  })}
                </div>
              </ScrollShadow>
            </div>
          </div>

          <div className="col-span-7">
            {selectedVariant && (
              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <div>
                    <h4 className="font-medium">{selectedVariant.sku}</h4>

                    <div className="mt-2 flex flex-wrap gap-1">
                      {selectedVariant.attributes.map((attr) => (
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

                  <Button variant="primary" onPress={addImage}>
                    <ImagePlus className="size-6" />
                    Add Image
                  </Button>
                </div>

                <ScrollShadow className="h-150 pr-2">
                  {selectedImages.length === 0 ? (
                    <Alert color="secondary">
                      <AlertCircle className="size-4" />

                      <p className="text-sm">
                        No images added for this variant.
                      </p>
                    </Alert>
                  ) : (
                    <div className="space-y-4">
                      <DndContext
                        sensors={sensors}
                        collisionDetection={closestCenter}
                        onDragEnd={handleDragEnd}
                      >
                        <SortableContext
                          items={selectedImages.map(
                            (image, index) =>
                              image.id ?? `${selectedVariantId}-${index}`,
                          )}
                          strategy={verticalListSortingStrategy}
                        >
                          <div className="grid grid-cols-1 xl:grid-cols-2 gap-4">
                            {selectedImages.map((image, index) => (
                              <SortableImageCard
                                key={
                                  image.id ?? `${selectedVariantId}-${index}`
                                }
                                image={image}
                                index={index}
                                variantId={selectedVariantId ?? ""}
                                onUpdate={updateImage}
                                onRemove={removeImage}
                              />
                            ))}
                          </div>
                        </SortableContext>
                      </DndContext>
                    </div>
                  )}
                </ScrollShadow>
              </div>
            )}
          </div>
        </div>

        <div className="flex gap-3 border-t border-default-200 pt-6">
          <Button variant="secondary" onPress={onBack} className="flex-1">
            Back
          </Button>

          <Button
            variant="primary"
            onPress={handleSave}
            isPending={isSaving}
            className="flex-1"
          >
            Complete
          </Button>
        </div>
      </div>
    </Card>
  );
}
