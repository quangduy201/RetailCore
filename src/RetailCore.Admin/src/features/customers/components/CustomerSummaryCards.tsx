import { Card } from "@heroui/react";

interface CustomerSummaryCardsProps {
  readonly totalCount: number;
  readonly activeCount: number;
  readonly inactiveCount: number;
}

export function CustomerSummaryCards({
  totalCount,
  activeCount,
  inactiveCount,
}: Readonly<CustomerSummaryCardsProps>) {
  return (
    <div className="grid gap-4 md:grid-cols-3">
      <SummaryCard label="Total customers" value={totalCount} />
      <SummaryCard label="Active customers" value={activeCount} />
      <SummaryCard label="Inactive customers" value={inactiveCount} />
    </div>
  );
}

interface SummaryCardProps {
  readonly label: string;
  readonly value: number;
}

function SummaryCard({ label, value }: Readonly<SummaryCardProps>) {
  return (
    <Card className="border border-default-200 p-5">
      <p className="text-sm text-default-500">{label}</p>
      <p className="mt-2 text-3xl font-bold tracking-tight">{value}</p>
    </Card>
  );
}
