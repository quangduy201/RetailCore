import { useRef } from "react";
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
  Description,
} from "@heroui/react";
import { ChevronLeft } from "lucide-react";
import { toast } from "sonner";

import { categoryApi } from "../api/category.api";
import { generateSlug } from "@/features/products/utils/slugGenerator";
import type { CreateCategoryRequest } from "../types";

export default function CategoryCreatePage() {
  const navigate = useNavigate();
  const formRef = useRef<HTMLFormElement>(null);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    try {
      const formData = new FormData(e.currentTarget);
      const data: CreateCategoryRequest = {
        name: (formData.get("name") as string) || "",
        slug: (formData.get("slug") as string) || "",
        description: (formData.get("description") as string) || undefined,
      };

      await categoryApi.create(data);
      toast.success("Category created successfully");
      navigate("/categories");
    } catch (error) {
      console.error("Failed to create category:", error);
      toast.error("Failed to create category. Please try again.");
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
            ref={formRef}
            className="space-y-6"
            onSubmit={handleSubmit}
            validationBehavior="aria"
          >
            {/* Name */}
            <TextField isRequired className="w-full">
              <Label className="text-sm font-semibold">Category Name</Label>
              <Input
                name="name"
                placeholder="e.g., Laptops, Phones, Tablets"
                onBlur={handleNameChange}
              />
              <FieldError className="text-xs" />
            </TextField>

            {/* Slug */}
            <TextField isRequired className="w-full">
              <Label className="text-sm font-semibold">Slug</Label>
              <Input name="slug" placeholder="auto-generated from name" />
              <Description className="text-xs text-default-500">
                URL-friendly identifier (auto-generated)
              </Description>
              <FieldError className="text-xs" />
            </TextField>

            {/* Description */}
            <TextField className="w-full">
              <Label className="text-sm font-semibold">Description</Label>
              <TextArea
                name="description"
                placeholder="Add a description for this category (optional)"
                rows={4}
              />
              <Description className="text-xs text-default-500">
                Provide details about the category
              </Description>
            </TextField>

            {/* Actions */}
            <div className="flex gap-3 border-t border-default-200 pt-6">
              <Button
                variant="secondary"
                onPress={() => navigate("/categories")}
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
