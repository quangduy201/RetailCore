import { Chip, type ChipVariants } from "@heroui/react";
import { CategoryStatus } from "@/shared/types/enums";

interface CategoryStatusBadgeProps {
  readonly status: CategoryStatus;
  readonly variant?: ChipVariants["variant"];
  readonly size?: ChipVariants["size"];
}

export function CategoryStatusBadge({
  status,
  variant = "soft",
  size = "sm",
}: Readonly<CategoryStatusBadgeProps>) {
  const statusConfig: Record<
    CategoryStatus,
    { label: string; color: ChipVariants["color"] }
  > = {
    [CategoryStatus.Active]: { label: "Active", color: "success" },
    [CategoryStatus.Inactive]: { label: "Inactive", color: "warning" },
    [CategoryStatus.Archived]: { label: "Archived", color: "danger" },
  };

  const config = statusConfig[status];

  return (
    <Chip size={size} variant={variant} color={config.color}>
      {config.label}
    </Chip>
  );
}
