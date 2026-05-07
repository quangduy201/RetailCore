import { Chip, type ChipVariants } from "@heroui/react";
import { ProductStatus, ProductVariantStatus } from "@/shared/types/enums";

interface ProductStatusBadgeProps {
  readonly status: ProductStatus;
  readonly variant?: ChipVariants["variant"];
  readonly size?: ChipVariants["size"];
}

export function ProductStatusBadge({
  status,
  variant = "soft",
  size = "sm",
}: Readonly<ProductStatusBadgeProps>) {
  const statusConfig: Record<
    ProductStatus,
    { label: string; color: ChipVariants["color"] }
  > = {
    [ProductStatus.Draft]: { label: "Draft", color: "default" },
    [ProductStatus.Active]: { label: "Active", color: "success" },
    [ProductStatus.Inactive]: { label: "Inactive", color: "warning" },
    [ProductStatus.Archived]: { label: "Archived", color: "danger" },
  };

  const config = statusConfig[status];

  return (
    <Chip size={size} variant={variant} color={config.color}>
      {config.label}
    </Chip>
  );
}

interface VariantStatusBadgeProps {
  readonly status: ProductVariantStatus;
  readonly variant?: ChipVariants["variant"];
  readonly size?: ChipVariants["size"];
}

export function VariantStatusBadge({
  status,
  variant = "secondary",
  size = "sm",
}: Readonly<VariantStatusBadgeProps>) {
  const statusConfig: Record<
    ProductVariantStatus,
    { label: string; color: ChipVariants["color"] }
  > = {
    [ProductVariantStatus.Active]: { label: "Active", color: "success" },
    [ProductVariantStatus.Inactive]: {
      label: "Inactive",
      color: "warning",
    },
    [ProductVariantStatus.Discontinued]: {
      label: "Discontinued",
      color: "danger",
    },
  };

  const config = statusConfig[status];

  return (
    <Chip size={size} variant={variant} color={config.color}>
      {config.label}
    </Chip>
  );
}
