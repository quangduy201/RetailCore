import { useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  Form,
  TextField,
  Label,
  Input,
  TextArea,
  Button,
  Card,
  Select,
  Spinner,
  FieldError,
  Description,
  ListBox,
} from "@heroui/react";
import { ChevronLeft } from "lucide-react";
import { toast } from "sonner";

import { brandApi } from "../api/brand.api";
import { generateSlug } from "@/features/products/utils/slugGenerator";
import { BrandStatus } from "@/shared/types/enums";
import type { BrandDetailDto, UpdateBrandRequest } from "../types";
import type { UUID } from "@/shared/types/common";

const BRAND_STATUS_OPTIONS = [
  { key: BrandStatus.Active, name: "Active" },
  { key: BrandStatus.Inactive, name: "Inactive" },
];

export default function BrandEditPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const formRef = useRef<HTMLFormElement>(null);

  const [brand, setBrand] = useState<BrandDetailDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    const loadBrand = async () => {
      if (!id) {
        toast.error("Invalid brand ID");
        navigate("/brands");
        return;
      }

      try {
        setIsLoading(true);
        const data = await brandApi.getById(id as UUID);
        setBrand(data);
      } catch (error) {
        console.error("Failed to load brand:", error);
        toast.error("Failed to load brand details");
        navigate("/brands");
      } finally {
        setIsLoading(false);
      }
    };

    void loadBrand();
  }, [id, navigate]);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!brand) return;

    try {
      setIsSubmitting(true);
      const formData = new FormData(e.currentTarget);
      const data: UpdateBrandRequest = {
        name: (formData.get("name") as string) || "",
        slug: (formData.get("slug") as string) || "",
        description: (formData.get("description") as string) || undefined,
        status: (Number(formData.get("status")) ||
          BrandStatus.Active) as BrandStatus,
      };

      await brandApi.update(brand.id, data);
      toast.success("Brand updated successfully");
      navigate("/brands");
    } catch (error) {
      console.error("Failed to update brand:", error);
      toast.error("Failed to update brand. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleNameChange = () => {
    const formElement = formRef.current;
    if (formElement) {
      const nameInput = formElement.querySelector(
        'input[name="name"]',
      ) as HTMLInputElement;
      const slugInput = formElement.querySelector(
        'input[name="slug"]',
      ) as HTMLInputElement;

      if (nameInput && slugInput && nameInput.value) {
        slugInput.value = generateSlug(nameInput.value);
      }
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <Spinner />
      </div>
    );
  }

  if (!brand) {
    return (
      <div className="py-12 text-center">
        <p className="text-default-500">Brand not found</p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-3">
        <Button
          isIconOnly
          variant="outline"
          onPress={() => navigate("/brands")}
        >
          <ChevronLeft className="size-5" />
        </Button>

        <div>
          <h1 className="text-2xl font-bold">Edit Brand</h1>
          <p className="text-sm text-default-500">Update brand information</p>
        </div>
      </div>

      {/* Form Card */}
      <Card className="border border-default-200">
        <div className="p-6">
          <Form
            ref={formRef}
            className="space-y-6"
            onSubmit={handleSubmit}
            validationBehavior="aria"
          >
            {/* Name */}
            <TextField isRequired className="w-full" defaultValue={brand.name}>
              <Label className="text-sm font-semibold">Brand Name</Label>
              <Input
                name="name"
                placeholder="e.g., Apple, Samsung, Dell"
                onBlur={handleNameChange}
              />
              <FieldError className="text-xs" />
            </TextField>

            {/* Slug */}
            <TextField isRequired className="w-full" defaultValue={brand.slug}>
              <Label className="text-sm font-semibold">Slug</Label>
              <Input name="slug" placeholder="url-friendly-identifier" />
              <Description className="text-xs text-default-500">
                URL-friendly identifier
              </Description>
              <FieldError className="text-xs" />
            </TextField>

            {/* Description */}
            <TextField
              className="w-full"
              defaultValue={brand.description ?? ""}
            >
              <Label className="text-sm font-semibold">Description</Label>
              <TextArea
                name="description"
                placeholder="Add a description for this brand"
                rows={4}
              />
              <Description className="text-xs text-default-500">
                Provide details about the brand
              </Description>
            </TextField>

            {/* Status */}
            <Select name="status" isRequired defaultValue={brand.status}>
              <Label className="text-sm font-semibold">Status</Label>
              <Select.Trigger>
                <Select.Value />
                <Select.Indicator />
              </Select.Trigger>
              <Select.Popover>
                <ListBox>
                  {BRAND_STATUS_OPTIONS.map((option) => (
                    <ListBox.Item key={option.key} id={option.key}>
                      {option.name}
                    </ListBox.Item>
                  ))}
                </ListBox>
              </Select.Popover>
            </Select>

            {/* Actions */}
            <div className="flex gap-3 border-t border-default-200 pt-6">
              <Button
                variant="secondary"
                onPress={() => navigate("/brands")}
                className="flex-1"
              >
                Cancel
              </Button>
              <Button
                type="submit"
                variant="primary"
                isDisabled={isSubmitting}
                className="flex-1"
              >
                {isSubmitting ? (
                  <>
                    <Spinner size="sm" />
                    Updating...
                  </>
                ) : (
                  "Update Brand"
                )}
              </Button>
            </div>
          </Form>
        </div>
      </Card>
    </div>
  );
}
