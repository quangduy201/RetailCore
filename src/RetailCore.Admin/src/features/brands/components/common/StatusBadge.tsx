import { Chip, type ChipVariants } from "@heroui/react";
import { BrandStatus } from "@/shared/types/enums";

interface BrandStatusBadgeProps {
  readonly status: BrandStatus;
  readonly variant?: ChipVariants["variant"];
  readonly size?: ChipVariants["size"];
}

export function BrandStatusBadge({
  status,
  variant = "soft",
  size = "sm",
}: Readonly<BrandStatusBadgeProps>) {
  const statusConfig: Record<
    BrandStatus,
    { label: string; color: ChipVariants["color"] }
  > = {
    [BrandStatus.Active]: { label: "Active", color: "success" },
    [BrandStatus.Inactive]: { label: "Inactive", color: "warning" },
  };

  const config = statusConfig[status];

  return (
    <Chip size={size} variant={variant} color={config.color}>
      {config.label}
    </Chip>
  );
}
