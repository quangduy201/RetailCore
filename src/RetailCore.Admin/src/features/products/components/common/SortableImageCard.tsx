import {
  DndContext,
  PointerSensor,
  closestCenter,
  useSensor,
  useSensors,
  type DragEndEvent,
} from "@dnd-kit/core";
import {
  SortableContext,
  verticalListSortingStrategy,
  useSortable,
  arrayMove,
} from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";

import type { VariantImageForm } from "../../types/forms";
import { GripVertical, Trash2 } from "lucide-react";
import { Button, Input } from "@heroui/react";

interface SortableImageCardProps {
  image: VariantImageForm;
  index: number;

  variantId: string;

  onUpdate: (index: number, updates: Partial<VariantImageForm>) => void;

  onRemove: (index: number) => void;
}

export function SortableImageCard({
  image,
  index,
  variantId,
  onUpdate,
  onRemove,
}: Readonly<SortableImageCardProps>) {
  const { attributes, listeners, setNodeRef, transform, transition } =
    useSortable({
      id: image.id ?? `${variantId}-${index}`,
    });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  };

  return (
    <div
      ref={setNodeRef}
      style={style}
      className="rounded-xl border border-default-200 bg-background p-4"
    >
      <div className="grid grid-cols-12 gap-4">
        <div className="col-span-1 flex items-start justify-center pt-2">
          <button
            type="button"
            {...attributes}
            {...listeners}
            className="cursor-grab text-default-400 active:cursor-grabbing"
          >
            <GripVertical className="size-5" />
          </button>
        </div>

        <div className="col-span-3">
          <div className="aspect-square overflow-hidden rounded-xl border border-default-200 bg-default-100">
            {image.url ? (
              <img
                src={image.url}
                alt="Variant"
                className="h-full w-full object-cover"
              />
            ) : (
              <div className="flex h-full items-center justify-center text-default-400">
                Preview
              </div>
            )}
          </div>
        </div>

        <div className="col-span-8 space-y-3">
          <Input
            placeholder="https://..."
            value={image.url}
            onChange={(e) =>
              onUpdate(index, {
                url: e.target.value,
              })
            }
          />

          <div className="flex items-center gap-3">
            <Button
              size="sm"
              variant={image.isPrimary ? "primary" : "secondary"}
              onPress={() =>
                onUpdate(index, {
                  isPrimary: true,
                })
              }
            >
              {image.isPrimary ? "Primary Image" : "Set Primary"}
            </Button>

            <Button
              size="sm"
              variant="danger-soft"
              onPress={() => onRemove(index)}
            >
              <Trash2 className="size-5" />
              Remove
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
