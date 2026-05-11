import { useEffect, useState } from "react";
import { Alert, Button, Card, Spinner } from "@heroui/react";
import { AlertCircle } from "lucide-react";
import { toast } from "sonner";

import { VariantTable } from "../variants/VariantTable";
import type { VariantFormValue } from "../../../types/forms";

import type { ProductDetailDto } from "@/features/products/types/types";
import type { UUID } from "@/shared/types/common";
import { ProductVariantStatus } from "@/shared/types/enums";
import { productApi } from "@/features/products/api/product.api";
import { variantsApi } from "@/features/products/api/variant.api";

interface Props {
  product?: ProductDetailDto | null;
  onBack: () => void;
  onCompleted: (product: ProductDetailDto) => void;
}

export function Step2Variants({
  product,
  onBack,
  onCompleted,
}: Readonly<Props>) {
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [variants, setVariants] = useState<VariantFormValue[]>([]);

  useEffect(() => {
    if (!product) return;

    if (product.attributes.length === 0) {
      setVariants([]);
      return;
    }

    const combinations: {
      combination: Record<string, string>;
      attributeValueIds: UUID[];
    }[] = [];

    const generateRecursive = (
      index: number,
      currentCombination: Record<string, string>,
      currentValueIds: UUID[],
    ) => {
      if (index >= product.attributes.length) {
        combinations.push({
          combination: { ...currentCombination },
          attributeValueIds: [...currentValueIds],
        });
        return;
      }

      const attribute = product.attributes[index];

      for (const value of attribute.values) {
        generateRecursive(
          index + 1,
          {
            ...currentCombination,
            [attribute.name]: value.value,
          },
          [...currentValueIds, value.id],
        );
      }
    };

    generateRecursive(0, {}, []);

    const mapped: VariantFormValue[] = combinations.map(
      ({ combination, attributeValueIds }) => {
        const matchedVariant = product.variants.find((variant) => {
          const existingIds = [
            ...variant.attributes.map((a) => a.attributeValueId),
          ].sort();
          const incomingIds = [...attributeValueIds].sort();
          return JSON.stringify(existingIds) === JSON.stringify(incomingIds);
        });

        const skuPart = Object.values(combination)
          .join("-")
          .toLowerCase()
          .replaceAll(/\s+/g, "-");

        return {
          id: matchedVariant?.id ?? null,
          tempId: crypto.randomUUID(),
          sku: matchedVariant?.sku ?? `${product.slug}-${skuPart}`,
          attributeCombination: combination,
          attributeValueIds,
          price: matchedVariant?.price ?? 0,
          stock: matchedVariant?.stock ?? 0,
          status: matchedVariant?.status ?? ProductVariantStatus.Draft,
          isSelected: matchedVariant != null,
          images:
            matchedVariant?.images.map((image) => ({
              id: image.id,
              url: image.url,
              sortOrder: image.sortOrder,
              isPrimary: image.isPrimary,
            })) ?? [],
        };
      },
    );

    setVariants(mapped);
  }, [product]);

  const updateVariant = (
    tempId: string,
    updates: Partial<VariantFormValue>,
  ) => {
    setVariants((prev) =>
      prev.map((variant) =>
        variant.tempId === tempId
          ? {
              ...variant,
              ...updates,
            }
          : variant,
      ),
    );
  };

  const handleSubmit = async () => {
    if (!product) return;

    if (variants.length === 0) {
      toast.error("Please generate variants first");
      return;
    }

    const selected = variants.filter((v) => v.isSelected);

    if (selected.length === 0) {
      toast.error("Please choose variants first");
      return;
    }

    try {
      setIsSubmitting(true);

      await Promise.all(
        selected.map(async (variant) => {
          const request = {
            sku: variant.sku,
            price: variant.price,
            stock: variant.stock,
            status: variant.status,
            attributeValueIds: variant.attributeValueIds,
            images: [],
          };

          if (variant.id) {
            await variantsApi.update(product.id, variant.id, request);
          } else {
            await variantsApi.create(product.id, request);
          }
        }),
      );

      const refreshedProduct = await productApi.getById(product.id);

      toast.success("Variants created");

      onCompleted(refreshedProduct);
    } catch (error) {
      console.error(error);
      toast.error("Failed to create variants");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Card className="border border-default-200 p-6">
      <div className="space-y-6">
        <div>
          <h3 className="mb-2 font-semibold">Product Variants</h3>

          <p className="text-sm text-default-500">
            Variants are generated from attribute combinations.
          </p>
        </div>

        {variants.length === 0 && (
          <Alert color="secondary">
            <AlertCircle className="size-4" />
            <p className="text-sm">Generate variants to continue</p>
          </Alert>
        )}

        <VariantTable variants={variants} onUpdate={updateVariant} />

        <div className="flex gap-3 border-t border-default-200 pt-6">
          <Button variant="secondary" onPress={onBack} className="flex-1">
            Back
          </Button>

          <Button
            variant="primary"
            onPress={handleSubmit}
            isDisabled={variants.length === 0}
            isPending={isSubmitting}
            className="flex-1"
          >
            {isSubmitting ? (
              <>
                <Spinner color="current" size="sm" />
                Saving...
              </>
            ) : (
              "Next: Variant Images"
            )}
          </Button>
        </div>
      </div>
    </Card>
  );
}
