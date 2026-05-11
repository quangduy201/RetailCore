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
  ListBox,
  Switch,
} from "@heroui/react";
import { ChevronLeft } from "lucide-react";
import { toast } from "sonner";

import { brandApi } from "../api/brand.api";
import { BrandStatus } from "@/shared/types/enums";
import type { BrandDetailDto, UpdateBrandRequest } from "../types";
import type { UUID } from "@/shared/types/common";
import { useAutoSlug } from "@/shared/hooks/useAutoSlug";

const BRAND_STATUS_OPTIONS = [
  { key: BrandStatus.Active, name: "Active" },
  { key: BrandStatus.Inactive, name: "Inactive" },
];

export default function BrandEditPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();

  const [brand, setBrand] = useState<BrandDetailDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [name, setName] = useState(brand?.name ?? "");
  const [slug, setSlug] = useState(brand?.slug ?? "");
  const [description, setDescription] = useState(brand?.description ?? "");
  const [status, setStatus] = useState<BrandStatus>(
    brand?.status ?? BrandStatus.Inactive,
  );

  const { auto, handleSlugChange, enableAuto, disableAuto } = useAutoSlug({
    name,
    onSlugChange: setSlug,
  });

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

        setName(data.name);
        setSlug(data.slug);
        setDescription(data.description ?? "");
        setStatus(data.status);
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
      const data: UpdateBrandRequest = {
        name: name,
        slug: slug,
        description: description,
        status: status,
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
            <TextField className="w-full" value={description}>
              <Label>Description</Label>
              <TextArea
                placeholder="Add a description for this brand"
                rows={4}
              />
            </TextField>

            {/* Status */}
            <Select
              isRequired
              value={status}
              onChange={(value) => setStatus(value as BrandStatus)}
            >
              <Label>Status</Label>
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
