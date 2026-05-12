import { Button, Card } from "@heroui/react";
import { AlertCircle, RefreshCw } from "lucide-react";

interface CustomerErrorAlertProps {
  readonly message: string;
  readonly onRetry: () => void;
}

export function CustomerErrorAlert({
  message,
  onRetry,
}: Readonly<CustomerErrorAlertProps>) {
  return (
    <Card className="border border-danger-200 bg-danger-50 p-4">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex gap-3">
          <AlertCircle className="mt-0.5 size-5 text-danger" />
          <div>
            <p className="font-semibold text-danger">Error</p>
            <p className="text-sm text-danger-700">{message}</p>
          </div>
        </div>

        <Button variant="danger-soft" onPress={onRetry}>
          <RefreshCw className="size-4" />
          Retry
        </Button>
      </div>
    </Card>
  );
}
