import { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Form,
  TextField,
  Label,
  Input,
  TextArea,
  Button,
  Card,
  FieldError,
  Switch,
} from "@heroui/react";
import { ChevronLeft } from "lucide-react";
import { toast } from "sonner";

import { categoryApi } from "../api/category.api";
import type { CreateCategoryRequest } from "../types";
import { useAutoSlug } from "@/shared/hooks/useAutoSlug";

export default function CategoryCreatePage() {
  const navigate = useNavigate();

  const [name, setName] = useState("");
  const [slug, setSlug] = useState("");
  const [description, setDescription] = useState("");

  const { auto, handleSlugChange, enableAuto, disableAuto } = useAutoSlug({
    name,
    onSlugChange: setSlug,
  });

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    try {
      const data: CreateCategoryRequest = {
        name: name,
        slug: slug,
        description: description || undefined,
      };

      await categoryApi.create(data);
      toast.success("Category created successfully");
      navigate("/admin/categories");
    } catch (error) {
      console.error("Failed to create category:", error);
      toast.error("Failed to create category. Please try again.");
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-3">
        <Button
          isIconOnly
          variant="outline"
          onPress={() => navigate("/admin/categories")}
        >
          <ChevronLeft className="size-5" />
        </Button>

        <div>
          <h1 className="text-2xl font-bold">Create Category</h1>
          <p className="text-sm text-default-500">
            Add a new category to your catalog
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
                placeholder="Add a description for this category (optional)"
                rows={4}
              />
            </TextField>

            {/* Actions */}
            <div className="flex gap-3 border-t border-default-200 pt-6">
              <Button
                variant="secondary"
                onPress={() => navigate("/admin/categories")}
                className="flex-1"
              >
                Cancel
              </Button>
              <Button type="submit" variant="primary" className="flex-1">
                Create Category
              </Button>
            </div>
          </Form>
        </div>
      </Card>
    </div>
  );
}
