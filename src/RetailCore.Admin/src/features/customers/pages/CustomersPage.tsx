import { useCallback, useEffect, useMemo, useState } from "react";

import { customerApi } from "../api/customer.api";
import { CustomerErrorAlert } from "../components/CustomerErrorAlert";
import { CustomerPageHeader } from "../components/CustomerPageHeader";
import { CustomerSummaryCards } from "../components/CustomerSummaryCards";
import { CustomerTable } from "../components/CustomerTable";
import type { UserDto } from "../types";

export default function CustomersPage() {
  const [customers, setCustomers] = useState<UserDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [togglingCustomerId, setTogglingCustomerId] = useState<
    UserDto["id"] | null
  >(null);

  const loadCustomers = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);

      const data = await customerApi.getCustomers();

      setCustomers(data);
    } catch {
      setError("Unable to load customers. Please try again.");
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadCustomers();
  }, [loadCustomers]);

  const filteredCustomers = useMemo(() => {
    const normalizedSearch = searchTerm.trim().toLowerCase();

    if (!normalizedSearch) {
      return customers;
    }

    return customers.filter((customer) => {
      const fullName = customer.fullName.toLowerCase();
      const email = customer.email.toLowerCase();

      return (
        fullName.includes(normalizedSearch) || email.includes(normalizedSearch)
      );
    });
  }, [customers, searchTerm]);

  const activeCount = useMemo(
    () => customers.filter((customer) => customer.isActive).length,
    [customers],
  );

  const handleToggleActiveStatus = async (customer: UserDto) => {
    const action = customer.isActive ? "deactivate" : "activate";

    if (!globalThis.confirm(`Are you sure you want to ${action} this user?`)) {
      return;
    }

    try {
      setTogglingCustomerId(customer.id);
      setError(null);

      await customerApi.toggleActiveStatus(customer.id);

      setCustomers((currentCustomers) =>
        currentCustomers.map((currentCustomer) =>
          currentCustomer.id === customer.id
            ? { ...currentCustomer, isActive: !currentCustomer.isActive }
            : currentCustomer,
        ),
      );
    } catch {
      setError("Unable to update customer status. Please try again.");
    } finally {
      setTogglingCustomerId(null);
    }
  };

  return (
    <div className="space-y-6">
      <CustomerPageHeader
        searchTerm={searchTerm}
        onSearchChange={setSearchTerm}
      />

      <CustomerSummaryCards
        totalCount={customers.length}
        activeCount={activeCount}
        inactiveCount={customers.length - activeCount}
      />

      {error && (
        <CustomerErrorAlert
          message={error}
          onRetry={() => void loadCustomers()}
        />
      )}

      <div className="flex justify-center">
        <CustomerTable
          customers={filteredCustomers}
          totalCount={customers.length}
          isLoading={isLoading}
          hasSearchTerm={searchTerm.trim().length > 0}
          togglingCustomerId={togglingCustomerId}
          onToggleActiveStatus={(customer) =>
            void handleToggleActiveStatus(customer)
          }
        />
      </div>
    </div>
  );
}
