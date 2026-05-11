import { useEffect, useState } from "react";
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
  Switch,
} from "@heroui/react";
import { ChevronLeft } from "lucide-react";
import { toast } from "sonner";

import { categoryApi } from "../api/category.api";
import { CategoryStatus } from "@/shared/types/enums";
import type { CategoryDetailDto, UpdateCategoryRequest } from "../types";
import type { UUID } from "@/shared/types/common";
import { useAutoSlug } from "@/shared/hooks/useAutoSlug";

const CATEGORY_STATUS_OPTIONS = [
  { key: CategoryStatus.Active, name: "Active" },
  { key: CategoryStatus.Inactive, name: "Inactive" },
  { key: CategoryStatus.Archived, name: "Archived" },
];

export default function CategoryEditPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();

  const [category, setCategory] = useState<CategoryDetailDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [name, setName] = useState(category?.name ?? "");
  const [slug, setSlug] = useState(category?.slug ?? "");
  const [description, setDescription] = useState(category?.description ?? "");
  const [status, setStatus] = useState<CategoryStatus>(
    category?.status ?? CategoryStatus.Inactive,
  );

  const { auto, handleSlugChange, enableAuto, disableAuto } = useAutoSlug({
    name,
    onSlugChange: setSlug,
  });

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

        setName(data.name);
        setSlug(data.slug);
        setDescription(data.description ?? "");
        setStatus(data.status);
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
      const data: UpdateCategoryRequest = {
        name: name,
        slug: slug,
        description: description || undefined,
        status: status,
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
            className="space-y-6"
            onSubmit={handleSubmit}
            validationBehavior="aria"
          >
            {/* Name */}
            <TextField
              isRequired
              value={name}
              onChange={setName}
              className="w-full"
            >
              <Label>Category Name</Label>
              <Input placeholder="e.g., Laptops, Phones, Tablets" />
              <FieldError className="text-xs" />
            </TextField>

            {/* Slug */}
            <div className="flex flex-row gap-3">
              <TextField
                isRequired
                value={slug}
                onChange={handleSlugChange}
                className="flex-1"
              >
                <Label>Slug</Label>

                <Input placeholder="url-friendly-identifier" />

                <Description className="text-xs text-default-500">
                  URL-friendly identifier
                </Description>

                <FieldError className="text-xs" />
              </TextField>

              <TextField className="pb-2">
                <Label className="w-20">Auto slug</Label>

                <Switch
                  isSelected={auto}
                  onChange={(checked) => {
                    if (checked) enableAuto();
                    else disableAuto();
                  }}
                >
                  <Switch.Control>
                    <Switch.Thumb />
                  </Switch.Control>
                </Switch>
              </TextField>
            </div>

            {/* Description */}
            <TextField
              value={description}
              onChange={setDescription}
              className="w-full"
            >
              <Label>Description</Label>
              <TextArea
                placeholder="Add a description for this category"
                rows={4}
              />
            </TextField>

            {/* Status */}
            <Select
              isRequired
              value={status}
              onChange={(value) => setStatus(value as CategoryStatus)}
            >
              <Label>Status</Label>
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
