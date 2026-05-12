import { Avatar, Button, Chip, Spinner, Table } from "@heroui/react";
import { Power, PowerOff, SearchX, Users } from "lucide-react";

import type { UserDto } from "../types";

const CUSTOMER_COLUMNS = [
  { key: "customer", label: "Customer", defaultWidth: 320, minWidth: 260 },
  { key: "status", label: "Status", defaultWidth: 100, minWidth: 80 },
  { key: "roles", label: "Roles", defaultWidth: 260, minWidth: 200 },
  { key: "joined", label: "Joined", defaultWidth: 150, minWidth: 120 },
  { key: "actions", label: "Actions", defaultWidth: 150, minWidth: 150 },
] as const;

type CustomerColumnKey = (typeof CUSTOMER_COLUMNS)[number]["key"];

interface CustomerTableProps {
  readonly customers: UserDto[];
  readonly totalCount: number;
  readonly isLoading: boolean;
  readonly hasSearchTerm: boolean;
  readonly togglingCustomerId: UserDto["id"] | null;
  readonly onToggleActiveStatus: (customer: UserDto) => void;
}

export function CustomerTable({
  customers,
  totalCount,
  isLoading,
  hasSearchTerm,
  togglingCustomerId,
  onToggleActiveStatus,
}: Readonly<CustomerTableProps>) {
  const renderCell = (customer: UserDto, columnKey: CustomerColumnKey) => {
    switch (columnKey) {
      case "customer":
        return <CustomerIdentity customer={customer} />;

      case "status":
        return (
          <Chip
            color={customer.isActive ? "success" : "warning"}
            variant="secondary"
          >
            {customer.isActive ? "Active" : "Inactive"}
          </Chip>
        );

      case "roles":
        return <CustomerRoles roles={customer.roles} />;

      case "joined":
        return (
          <span className="text-sm text-default-600">
            {formatJoinedDate(customer.createdAt)}
          </span>
        );

      case "actions":
        return (
          <Button
            size="sm"
            variant={customer.isActive ? "danger-soft" : "primary"}
            isDisabled={togglingCustomerId === customer.id}
            onPress={() => onToggleActiveStatus(customer)}
          >
            {customer.isActive ? (
              <PowerOff className="size-4" />
            ) : (
              <Power className="size-4" />
            )}
            {customer.isActive ? "Deactivate" : "Activate"}
          </Button>
        );

      default:
        return "—";
    }
  };

  return (
    <Table variant="primary" className="border border-default w-fit">
      <Table.ResizableContainer>
        <Table.Content
          aria-label="Customers table"
          className="max-h-175 overflow-auto"
        >
          <Table.Header columns={CUSTOMER_COLUMNS}>
            {(column) => (
              <Table.Column
                id={column.key}
                isRowHeader={column.key === "customer"}
                defaultWidth={column.defaultWidth}
                minWidth={column.minWidth}
                className={`border-b border-default-200 bg-default ${column.key === "actions" ? "sticky right-0" : ""}`}
              >
                <div className="flex items-center gap-2">
                  <span>{column.label}</span>
                  <Table.ColumnResizer />
                </div>
              </Table.Column>
            )}
          </Table.Header>

          <Table.Body
            items={customers}
            renderEmptyState={() => (
              <CustomerEmptyState
                isLoading={isLoading}
                hasSearchTerm={hasSearchTerm}
              />
            )}
          >
            {(customer) => (
              <Table.Row id={customer.id}>
                {CUSTOMER_COLUMNS.map((column) => (
                  <Table.Cell
                    key={column.key}
                    className={
                      column.key === "actions"
                        ? "sticky right-0 border-l backdrop-blur-xs"
                        : ""
                    }
                  >
                    {renderCell(customer, column.key)}
                  </Table.Cell>
                ))}
              </Table.Row>
            )}
          </Table.Body>
        </Table.Content>
      </Table.ResizableContainer>

      <Table.Footer className="border-t border-default-200 px-6 py-4">
        <div className="text-sm text-default-500">
          Showing {customers.length} of {totalCount} customers
        </div>
      </Table.Footer>
    </Table>
  );
}

function CustomerIdentity({ customer }: Readonly<{ customer: UserDto }>) {
  return (
    <div className="flex min-w-0 items-center gap-3">
      <Avatar size="md">
        {customer.avatarUrl && <Avatar.Image src={customer.avatarUrl} />}
        <Avatar.Fallback className="border-none bg-linear-to-br from-blue-600 to-green-600 text-white">
          {getInitials(customer.fullName, customer.email)}
        </Avatar.Fallback>
      </Avatar>

      <div className="min-w-0">
        <p className="truncate font-semibold text-default-900">
          {customer.fullName || "Unnamed customer"}
        </p>
        <p className="truncate text-sm text-default-500">{customer.email}</p>
      </div>
    </div>
  );
}

function CustomerRoles({ roles }: Readonly<{ roles: string[] }>) {
  if (roles.length === 0) {
    return <span className="text-sm text-default-400">No roles</span>;
  }

  return (
    <div className="flex flex-wrap gap-2">
      {roles.map((role) => (
        <Chip key={role} size="sm" variant="secondary">
          {role}
        </Chip>
      ))}
    </div>
  );
}

interface CustomerEmptyStateProps {
  readonly isLoading: boolean;
  readonly hasSearchTerm: boolean;
}

function CustomerEmptyState({
  isLoading,
  hasSearchTerm,
}: Readonly<CustomerEmptyStateProps>) {
  if (isLoading) {
    return (
      <div className="flex flex-col items-center justify-center py-20">
        <Spinner />
        <p className="mt-4 text-sm text-default-500">Loading customers...</p>
      </div>
    );
  }

  return (
    <div className="flex flex-col items-center justify-center py-20 text-center">
      {hasSearchTerm ? (
        <SearchX className="size-14 text-default-300" />
      ) : (
        <Users className="size-14 text-default-300" />
      )}

      <p className="mt-4 text-sm font-semibold text-default-600">
        {hasSearchTerm
          ? "No customers match your search"
          : "No customers found"}
      </p>
    </div>
  );
}

const getInitials = (name: string, email: string) => {
  const source = name.trim() || email.trim();
  const words = source.split(/\s+/).filter(Boolean);

  if (words.length >= 2) {
    return `${words[0][0]}${words[1][0]}`.toUpperCase();
  }

  return source.slice(0, 2).toUpperCase();
};

const formatJoinedDate = (value: string) =>
  new Date(value).toLocaleDateString(undefined, {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
