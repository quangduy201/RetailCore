import { Card, Chip, SearchField } from "@heroui/react";

interface CustomerPageHeaderProps {
  readonly searchTerm: string;
  readonly onSearchChange: (value: string) => void;
}

export function CustomerPageHeader({
  searchTerm,
  onSearchChange,
}: Readonly<CustomerPageHeaderProps>) {
  return (
    <Card className="overflow-hidden border border-default-200">
      <div className="flex flex-col gap-6 bg-linear-to-r from-default-50 to-default-100 px-6 py-6 lg:flex-row lg:items-center lg:justify-between">
        <div className="space-y-3">
          <Chip variant="secondary">Customer Oversight</Chip>

          <div>
            <h1 className="text-3xl font-bold tracking-tight">Customers</h1>
            <p className="mt-1 text-sm text-default-500">
              View registered customer accounts
            </p>
          </div>
        </div>

        <div className="min-w-72">
          <SearchField value={searchTerm} onChange={onSearchChange}>
            <SearchField.Group>
              <SearchField.SearchIcon />
              <SearchField.Input placeholder="Search customers..." />
              <SearchField.ClearButton />
            </SearchField.Group>
          </SearchField>
        </div>
      </div>
    </Card>
  );
}
