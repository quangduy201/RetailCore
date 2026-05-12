import { useEffect, useMemo, useState } from "react";
import {
  Button,
  Card,
  Description,
  FieldError,
  Form,
  Input,
  Label,
  ListBox,
  Select,
  Spinner,
  Tag,
  TagGroup,
  TextArea,
  TextField,
} from "@heroui/react";
import { Plus, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { brandApi } from "@/features/brands/api/brand.api";
import type { BrandDetailDto, BrandSummaryDto } from "@/features/brands/types";
import { categoryApi } from "@/features/categories/api/category.api";
import type {
  CategoryDetailDto,
  CategorySummaryDto,
} from "@/features/categories/types";
import { productApi } from "@/features/products/api/product.api";
import type {
  CreateProductAttributeRequest,
  CreateProductAttributeValueRequest,
  ProductDetailDto,
} from "@/features/products/types/types";
import type { UUID } from "@/shared/types/common";
import { generateSlug } from "@/shared/utils/slugUtils";

import type {
  AttributeFormValue,
  ProductBasicInfoForm,
} from "../../../types/forms";

interface Props {
  product?: ProductDetailDto | null;
  onCompleted: (product: ProductDetailDto) => void;
  onCancel: () => void;
}

export function Step1BasicInfo({
  product,
  onCompleted,
  onCancel,
}: Readonly<Props>) {
  const isEditMode = !!product;
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [brands, setBrands] = useState<BrandSummaryDto[]>([]);
  const [categories, setCategories] = useState<CategorySummaryDto[]>([]);

  const [basicInfo, setBasicInfo] = useState<ProductBasicInfoForm>({
    name: "",
    slug: "",
    shortDescription: "",
    description: "",
    brandId: "",
    categoryId: "",
  });

  const [attributes, setAttributes] = useState<AttributeFormValue[]>(
    product?.attributes?.map((a) => ({
      id: a.id,
      name: a.name,
      values: a.values.map((av) => ({
        id: av.id,
        value: av.value,
      })),
    })) ?? [],
  );
  const [newAttributeName, setNewAttributeName] = useState("");

  const isFormValid = useMemo(
    () =>
      !!basicInfo.name &&
      !!basicInfo.slug &&
      !!basicInfo.brandId &&
      !!basicInfo.categoryId,
    [basicInfo],
  );

  useEffect(() => {
    const getBrandsAndCategories = async () => {
      try {
        const [brandData, categoryData] = await Promise.all([
          brandApi.getPaged({ pageSize: 1000 }),
          categoryApi.getPaged({ pageSize: 1000 }),
        ]);

        setBrands(
          brandData.items.map((b: BrandDetailDto) => ({
            id: b.id,
            name: b.name,
            slug: b.slug,
          })),
        );

        setCategories(
          categoryData.items.map((c: CategoryDetailDto) => ({
            id: c.id,
            name: c.name,
            slug: c.slug,
            description: c.description,
          })),
        );
      } catch (error) {
        console.error(error);
      }
    };

    void getBrandsAndCategories();
  }, []);

  useEffect(() => {
    if (!product) {
      return;
    }

    setBasicInfo({
      name: product.name,
      slug: product.slug,
      shortDescription: product.shortDescription ?? "",
      description: product.description ?? "",
      brandId: product.brandId,
      categoryId: product.categoryId,
    });

    setAttributes(
      product.attributes.map((attribute) => ({
        id: attribute.id,
        name: attribute.name,
        values: attribute.values.map((value) => ({
          id: value.id,
          value: value.value,
        })),
      })),
    );
  }, [product]);

  const handleNameChange = (name: string) => {
    setBasicInfo((prev) => ({
      ...prev,
      name,
      slug: generateSlug(name),
    }));
  };

  const handleBasicInfoChange = (
    field: keyof ProductBasicInfoForm,
    value: string,
  ) => {
    setBasicInfo((prev) => ({
      ...prev,
      [field]: value,
    }));
  };

  const handleAddAttribute = () => {
    const trimmed = newAttributeName.trim();

    if (!trimmed) {
      return;
    }

    const exists = attributes.some(
      (attribute) => attribute.name.toLowerCase() === trimmed.toLowerCase(),
    );

    if (exists) {
      toast.error("Attribute already exists");

      return;
    }

    setAttributes((prev) => [
      ...prev,
      {
        id: crypto.randomUUID(),
        name: trimmed,
        values: [],
      },
    ]);

    setNewAttributeName("");
  };

  const handleRemoveAttribute = (attributeId: string) => {
    setAttributes((prev) =>
      prev.filter((attribute) => attribute.id !== attributeId),
    );
  };

  const handleAddAttributeValue = (attributeId: string, rawValue: string) => {
    const trimmed = rawValue.trim();

    if (!trimmed) return;

    setAttributes((prev) =>
      prev.map((attribute) => {
        if (attribute.id !== attributeId) {
          return attribute;
        }

        const exists = attribute.values.some(
          (value) => value.value.toLowerCase() === rawValue.toLowerCase(),
        );

        if (exists) {
          toast.error("Value already exists");
          return attribute;
        }

        return {
          ...attribute,
          values: [
            ...attribute.values,
            {
              id: crypto.randomUUID(),
              value: rawValue,
            },
          ],
        };
      }),
    );
  };

  const handleRemoveAttributeValue = (attributeId: string, valueId: string) => {
    setAttributes((prev) =>
      prev.map((attribute) => {
        if (attribute.id !== attributeId) {
          return attribute;
        }

        return {
          ...attribute,
          values: attribute.values.filter((value) => value.id !== valueId),
        };
      }),
    );
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!isFormValid) {
      toast.error("Please fill all required fields");
      return;
    }

    try {
      setIsSubmitting(true);

      const attributesData: CreateProductAttributeRequest[] = attributes.map(
        (attribute) => ({
          name: attribute.name,
          values: attribute.values.map(
            (value): CreateProductAttributeValueRequest => ({
              value: value.value,
            }),
          ),
        }),
      );

      const request = {
        name: basicInfo.name,
        slug: basicInfo.slug,
        shortDescription: basicInfo.shortDescription || undefined,
        description: basicInfo.description || undefined,
        brandId: basicInfo.brandId as UUID,
        categoryId: basicInfo.categoryId as UUID,
        attributes: attributesData,
        variants: [],
      };

      let updatedProduct: ProductDetailDto;

      if (isEditMode && product) {
        await productApi.update(product.id, request);
        updatedProduct = await productApi.getById(product.id);
        toast.success("Product updated");
      } else {
        const productId = await productApi.create(request);
        updatedProduct = await productApi.getById(productId);
        toast.success("Product created");
      }

      onCompleted(updatedProduct);
    } catch (error) {
      console.error(error);
      toast.error(
        isEditMode ? "Failed to update product" : "Failed to create product",
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Card className="border border-default-200 p-6">
      <Form
        onSubmit={handleSubmit}
        validationBehavior="aria"
        className="space-y-6"
      >
        {/* Product Information */}
        <div className="space-y-4">
          <h3 className="font-semibold">Product Information</h3>

          {/* Product Name */}
          <TextField
            isRequired
            className="w-full"
            value={basicInfo.name}
            onChange={handleNameChange}
          >
            <Label className="text-sm font-semibold">Product Name</Label>
            <Input placeholder="e.g. MacBook Pro 16-inch" />
            <FieldError className="text-xs" />
          </TextField>

          {/* Slug */}
          <TextField
            isRequired
            className="w-full"
            value={basicInfo.slug}
            onChange={(value) => handleBasicInfoChange("slug", value)}
          >
            <Label className="text-sm font-semibold">Slug</Label>
            <Input placeholder="url-friendly-slug" />
            <Description className="text-xs text-default-500">
              Auto-generated from product name
            </Description>
            <FieldError className="text-xs" />
          </TextField>

          {/* Short Description */}
          <TextField
            className="w-full"
            value={basicInfo.shortDescription}
            onChange={(value) =>
              handleBasicInfoChange("shortDescription", value)
            }
          >
            <Label className="text-sm font-semibold">Short Description</Label>

            <TextArea placeholder="Brief overview" rows={2} />
          </TextField>

          {/* Description */}
          <TextField
            className="w-full"
            value={basicInfo.description}
            onChange={(value) => handleBasicInfoChange("description", value)}
          >
            <Label className="text-sm font-semibold">Description</Label>

            <TextArea placeholder="Detailed description" rows={4} />
          </TextField>

          <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
            {/* Brand */}
            <Select
              isRequired
              value={basicInfo.brandId}
              onChange={(key) =>
                handleBasicInfoChange("brandId", key as string)
              }
            >
              <Label>Brand</Label>
              <Select.Trigger>
                <Select.Value />
                <Select.Indicator />
              </Select.Trigger>
              <Select.Popover>
                <ListBox>
                  {brands.map((brand) => (
                    <ListBox.Item key={brand.id} id={brand.id}>
                      {brand.name}
                    </ListBox.Item>
                  ))}
                </ListBox>
              </Select.Popover>
            </Select>

            {/* Category */}
            <Select
              isRequired
              value={basicInfo.categoryId}
              onChange={(key) =>
                handleBasicInfoChange("categoryId", key as string)
              }
            >
              <Label>Category</Label>
              <Select.Trigger>
                <Select.Value />
                <Select.Indicator />
              </Select.Trigger>
              <Select.Popover>
                <ListBox>
                  {categories.map((category) => (
                    <ListBox.Item key={category.id} id={category.id}>
                      {category.name}
                    </ListBox.Item>
                  ))}
                </ListBox>
              </Select.Popover>
            </Select>
          </div>
        </div>

        {/* Attributes */}
        <div className="space-y-5 border-t border-default-200 pt-8">
          <h3 className="font-semibold">Product Attributes</h3>

          {/* Add Attribute */}
          <Card className="border border-default-200 bg-default-50 p-4">
            <div className="flex gap-2">
              <input
                value={newAttributeName}
                onChange={(e) => setNewAttributeName(e.target.value)}
                placeholder="e.g. Color, Size"
                className="flex-1 rounded-lg border border-default-300 bg-surface px-3 py-2 text-sm"
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault();
                    handleAddAttribute();
                  }
                }}
              />

              <Button isIconOnly variant="primary" onPress={handleAddAttribute}>
                <Plus className="size-4" />
              </Button>
            </div>
          </Card>

          {/* Attribute List */}
          {attributes.length === 0 ? (
            <Card className="border border-dashed border-default-300 p-8 text-center">
              <p className="text-sm text-default-500">
                No attributes added yet
              </p>
            </Card>
          ) : (
            <div className="space-y-4">
              {attributes.map((attribute) => (
                <Card
                  key={attribute.id}
                  className="border border-default-200 p-4"
                >
                  <div className="flex items-center justify-between">
                    <div>
                      <h3 className="font-semibold">{attribute.name}</h3>

                      <p className="text-xs text-default-500">
                        {attribute.values.length} values
                      </p>
                    </div>

                    <Button
                      isIconOnly
                      variant="danger"
                      onPress={() => handleRemoveAttribute(attribute.id)}
                    >
                      <Trash2 className="size-4" />
                    </Button>
                  </div>
                  <div className="space-y-3">
                    <div className="rounded-xl border border-default-300 bg-default-50 p-3">
                      {attribute.values.length > 0 && (
                        <TagGroup
                          aria-label={`${attribute.name} values`}
                          onRemove={(keys) => {
                            const ids = Array.from(keys) as string[];

                            ids.forEach((id) => {
                              handleRemoveAttributeValue(attribute.id, id);
                            });
                          }}
                        >
                          <TagGroup.List className="flex flex-wrap gap-2">
                            {attribute.values.map((value) => (
                              <Tag key={value.id} id={value.id}>
                                {value.value}
                              </Tag>
                            ))}
                          </TagGroup.List>
                        </TagGroup>
                      )}

                      <input
                        type="text"
                        placeholder={
                          attribute.values.length === 0
                            ? "Type value and press Enter..."
                            : "Add more..."
                        }
                        className={`w-full bg-transparent text-sm outline-none placeholder:text-default-400 ${attribute.values.length === 0 ? "" : "mt-3"}`}
                        onKeyDown={(e) => {
                          const input = e.currentTarget;

                          if (e.key === "Enter") {
                            e.preventDefault();

                            handleAddAttributeValue(attribute.id, input.value);

                            input.value = "";
                          }

                          if (
                            e.key === "Backspace" &&
                            input.value === "" &&
                            attribute.values.length > 0
                          ) {
                            const last = attribute.values.at(-1);

                            if (last) {
                              handleRemoveAttributeValue(attribute.id, last.id);
                            }
                          }
                        }}
                      />
                    </div>

                    <p className="text-xs text-default-500">
                      Press Enter to add values
                    </p>
                  </div>
                </Card>
              ))}
            </div>
          )}
        </div>

        {/* Actions */}
        <div className="flex gap-3 border-t border-default-200 pt-6">
          <Button variant="secondary" onPress={onCancel} className="flex-1">
            Cancel
          </Button>

          <Button
            type="submit"
            variant="primary"
            isPending={isSubmitting}
            className="flex-1"
          >
            {isSubmitting ? (
              <>
                <Spinner color="current" size="sm" />
                Saving...
              </>
            ) : (
              "Next: Generate Variants"
            )}
          </Button>
        </div>
      </Form>
    </Card>
  );
}
