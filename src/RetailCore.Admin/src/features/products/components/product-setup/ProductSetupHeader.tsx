import type { UUID } from "@/shared/types/common";
import { Button } from "@heroui/react";
import { ChevronLeft } from "lucide-react";

interface Props {
  productId?: UUID;
  title: string;
  subtitle: string;
  onBack: () => void;
}

export function ProductSetupHeader({
  productId,
  title,
  subtitle,
  onBack,
}: Props) {
  return (
    <div className="flex items-start justify-between gap-4">
      <div className="flex items-center gap-3">
        <Button
          isIconOnly
          variant="outline"
          className="h-11 w-11 rounded-2xl"
          onPress={onBack}
        >
          <ChevronLeft className="size-5" />
        </Button>

        <div>
          <h1 className="text-3xl font-bold tracking-tight">{title}</h1>

          <p className="mt-1 text-sm text-default-500">{subtitle}</p>
        </div>
      </div>

      {productId && (
        <div className="hidden rounded-2xl border border-default-200 bg-default-50 px-4 py-3 md:block">
          <p className="text-xs text-default-500">Product ID</p>

          <p className="mt-1 font-mono text-sm">{productId}</p>
        </div>
      )}
    </div>
  );
}
