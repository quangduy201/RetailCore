import { useNavigate } from "react-router-dom";
import { Button, Card } from "@heroui/react";
import { ShieldX } from "lucide-react";

export default function UnauthorizedPage() {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen flex items-center justify-center bg-zinc-950 px-4">
      <Card className="max-w-md w-full p-8 text-center space-y-5 bg-zinc-900 border border-zinc-800">
        {/* Icon */}
        <div className="flex justify-center">
          <div className="w-14 h-14 rounded-full bg-red-500/10 flex items-center justify-center">
            <ShieldX className="w-7 h-7 text-red-500" />
          </div>
        </div>

        {/* Title */}
        <div>
          <h1 className="text-xl font-semibold text-white">Access Denied</h1>
          <p className="text-sm text-zinc-400 mt-2">
            You don't have permission to view this page.
          </p>
        </div>

        {/* Actions */}
        <div className="flex flex-col gap-2 pt-2">
          <Button
            onPress={() => navigate("/admin/dashboard")}
            className="w-full"
          >
            Go to Dashboard
          </Button>

          <Button
            variant="secondary"
            onPress={() => navigate("/login")}
            className="w-full"
          >
            Back to Login
          </Button>
        </div>
      </Card>
    </div>
  );
}
