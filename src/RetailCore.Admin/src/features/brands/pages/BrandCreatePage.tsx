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

import { brandApi } from "../api/brand.api";
import { generateSlug } from "@/features/products/utils/slugGenerator";
import type { CreateBrandRequest } from "../types";

export default function BrandCreatePage() {
  const navigate = useNavigate();
  const formRef = useRef<HTMLFormElement>(null);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    try {
      const formData = new FormData(e.currentTarget);
      const data: CreateBrandRequest = {
        name: (formData.get("name") as string) || "",
        slug: (formData.get("slug") as string) || "",
        description: (formData.get("description") as string) || undefined,
      };

      await brandApi.create(data);
      toast.success("Brand created successfully");
      navigate("/brands");
    } catch (error) {
      console.error("Failed to create brand:", error);
      toast.error("Failed to create brand. Please try again.");
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
          onPress={() => navigate("/brands")}
        >
          <ChevronLeft className="size-5" />
        </Button>

        <div>
          <h1 className="text-2xl font-bold">Create Brand</h1>
          <p className="text-sm text-default-500">
            Add a new brand to your catalog
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
              <Label className="text-sm font-semibold">Brand Name</Label>
              <Input
                name="name"
                placeholder="e.g., Apple, Samsung, Dell"
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
                placeholder="Add a description for this brand (optional)"
                rows={4}
              />
              <Description className="text-xs text-default-500">
                Provide details about the brand
              </Description>
            </TextField>

            {/* Actions */}
            <div className="flex gap-3 border-t border-default-200 pt-6">
              <Button
                variant="secondary"
                onPress={() => navigate("/brands")}
                className="flex-1"
              >
                Cancel
              </Button>
              <Button type="submit" variant="primary" className="flex-1">
                Create Brand
              </Button>
            </div>
          </Form>
        </div>
      </Card>
    </div>
  );
}
