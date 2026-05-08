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

import { categoryApi } from "../api/category.api";
import { generateSlug } from "@/features/products/utils/slugGenerator";
import { CategoryStatus } from "@/shared/types/enums";
import type { CategoryDetailDto, UpdateCategoryRequest } from "../types";
import type { UUID } from "@/shared/types/common";

const CATEGORY_STATUS_OPTIONS = [
  { key: CategoryStatus.Active, name: "Active" },
  { key: CategoryStatus.Inactive, name: "Inactive" },
  { key: CategoryStatus.Archived, name: "Archived" },
];

export default function CategoryEditPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const formRef = useRef<HTMLFormElement>(null);

  const [category, setCategory] = useState<CategoryDetailDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    const loadCategory = async () => {
      if (!id) {
        toast.error("Invalid category ID");
        navigate("/categories");
        return;
      }

      try {
        setIsLoading(true);
        const data = await categoryApi.getById(id as UUID);
        setCategory(data);
      } catch (error) {
        console.error("Failed to load category:", error);
        toast.error("Failed to load category details");
        navigate("/categories");
      } finally {
        setIsLoading(false);
      }
    };

    void loadCategory();
  }, [id, navigate]);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!category) return;

    try {
      setIsSubmitting(true);
      const formData = new FormData(e.currentTarget);
      const data: UpdateCategoryRequest = {
        name: (formData.get("name") as string) || "",
        slug: (formData.get("slug") as string) || "",
        description: (formData.get("description") as string) || undefined,
        status: (Number(formData.get("status") as string) ||
          CategoryStatus.Active) as CategoryStatus,
      };

      await categoryApi.update(category.id, data);
      toast.success("Category updated successfully");
      navigate("/categories");
    } catch (error) {
      console.error("Failed to update category:", error);
      toast.error("Failed to update category. Please try again.");
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

  if (!category) {
    return (
      <div className="py-12 text-center">
        <p className="text-default-500">Category not found</p>
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
          onPress={() => navigate("/categories")}
        >
          <ChevronLeft className="size-5" />
        </Button>

        <div>
          <h1 className="text-2xl font-bold">Edit Category</h1>
          <p className="text-sm text-default-500">
            Update category information
          </p>
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
            <TextField
              isRequired
              className="w-full"
              defaultValue={category.name}
            >
              <Label className="text-sm font-semibold">Category Name</Label>
              <Input
                name="name"
                placeholder="e.g., Laptops, Phones, Tablets"
                onBlur={handleNameChange}
              />
              <FieldError className="text-xs" />
            </TextField>

            {/* Slug */}
            <TextField
              isRequired
              className="w-full"
              defaultValue={category.slug}
            >
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
              defaultValue={category.description ?? ""}
            >
              <Label className="text-sm font-semibold">Description</Label>
              <TextArea
                name="description"
                placeholder="Add a description for this category"
                rows={4}
              />
              <Description className="text-xs text-default-500">
                Provide details about the category
              </Description>
            </TextField>

            {/* Status */}
            <Select name="status" isRequired defaultValue={category.status}>
              <Label className="text-sm font-semibold">Status</Label>
              <Select.Trigger>
                <Select.Value />
                <Select.Indicator />
              </Select.Trigger>
              <Select.Popover>
                <ListBox>
                  {CATEGORY_STATUS_OPTIONS.map((option) => (
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
                onPress={() => navigate("/categories")}
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
                  "Update Category"
                )}
              </Button>
            </div>
          </Form>
        </div>
      </Card>
    </div>
  );
}
