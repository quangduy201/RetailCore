import {
  Chip,
  Input,
  Label,
  ListBox,
  Select,
  Switch,
  Table,
} from "@heroui/react";

import type { VariantFormValue } from "../../../types/forms";
import { ProductVariantStatus } from "@/shared/types/enums";
import { VARIANT_TABLE_COLUMNS } from "@/features/products/constants/variantTableColumns";

interface Props {
  variants: VariantFormValue[];
  onUpdate: (variantId: string, updates: Partial<VariantFormValue>) => void;
}

export function VariantTable({ variants, onUpdate }: Readonly<Props>) {
  const statusOptions = Object.entries(ProductVariantStatus)
    .filter(([, value]) => typeof value === "number")
    .map(([key, value]) => ({
      value,
      label: key,
    }));

  return (
    <Table className="border-2">
      <Table.ResizableContainer className="min-h-60 max-h-[calc(100vh-200px)] overflow-auto">
        <Table.Content>
          <Table.Header columns={VARIANT_TABLE_COLUMNS}>
            {(column) => (
              <Table.Column
                id={column.key}
                isRowHeader={column.key === "sku"}
                defaultWidth={column.defaultWidth}
                minWidth={column.minWidth}
                className={`border-b border-default-200 bg-default ${column.key === "active" ? "sticky right-0" : ""}`}
              >
                <div className="flex items-center gap-2">
                  <span>{column.label}</span>
                  <Table.ColumnResizer />
                </div>
              </Table.Column>
            )}
          </Table.Header>

          <Table.Body>
            {variants.map((variant) => (
              <Table.Row key={variant.tempId}>
                <Table.Cell className="w-30">
                  <span className="w-30 font-mono text-sm">{variant.sku}</span>
                </Table.Cell>

                <Table.Cell>
                  <div className="w-30 flex flex-wrap gap-1">
                    {Object.entries(variant.attributeCombination).map(
                      ([name, value]) => (
                        <Chip key={name} size="sm" variant="soft">
                          {name}: {value}
                        </Chip>
                      ),
                    )}
                  </div>
                </Table.Cell>

                <Table.Cell>
                  <Input
                    type="number"
                    min="0"
                    step="0.01"
                    value={variant.price}
                    onChange={(e) =>
                      onUpdate(variant.tempId, {
                        price: parseFloat(e.target.value) || 0,
                      })
                    }
                    className="w-24"
                  />
                </Table.Cell>

                <Table.Cell>
                  <Input
                    type="number"
                    min="0"
                    value={variant.stock}
                    onChange={(e) =>
                      onUpdate(variant.tempId, {
                        stock: parseInt(e.target.value) || 0,
                      })
                    }
                    className="w-20"
                  />
                </Table.Cell>

                <Table.Cell>
                  <Select
                    defaultValue={variant.status}
                    placeholder="Select"
                    className="w-30"
                  >
                    <Select.Trigger>
                      <Select.Value />
                      <Select.Indicator />
                    </Select.Trigger>
                    <Select.Popover>
                      <ListBox>
                        {statusOptions.map((status) => (
                          <ListBox.Item
                            key={status.value}
                            id={status.value}
                            textValue={status.label}
                          >
                            {status.label}
                          </ListBox.Item>
                        ))}
                      </ListBox>
                    </Select.Popover>
                  </Select>
                </Table.Cell>

                <Table.Cell className="sticky right-0 border-l backdrop-blur-xs">
                  <Switch
                    isSelected={variant.isSelected}
                    onChange={(value) =>
                      onUpdate(variant.tempId, {
                        isSelected: value,
                      })
                    }
                  >
                    <Switch.Control>
                      <Switch.Thumb />
                    </Switch.Control>
                  </Switch>
                </Table.Cell>
              </Table.Row>
            ))}
          </Table.Body>
        </Table.Content>
      </Table.ResizableContainer>
    </Table>
  );
}
