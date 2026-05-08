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

import { brandApi } from "../api/brand.api";
import type { CreateBrandRequest } from "../types";
import { useAutoSlug } from "@/shared/hooks/useAutoSlug";

export default function BrandCreatePage() {
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
      const data: CreateBrandRequest = {
        name: name,
        slug: slug,
        description: description ?? undefined,
      };

      await brandApi.create(data);
      toast.success("Brand created successfully");
      navigate("/brands");
    } catch (error) {
      console.error("Failed to create brand:", error);
      toast.error("Failed to create brand. Please try again.");
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
              <Label>Brand Name</Label>
              <Input placeholder="e.g., Apple, Samsung, Dell" />
              <FieldError className="text-xs" />
            </TextField>

            {/* Slug */}
            <div className="flex flex-row gap-3">
              <TextField
                isRequired
                className="flex-1"
                value={slug}
                onChange={handleSlugChange}
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
              <TextArea placeholder="Optional brand description" rows={4} />
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
