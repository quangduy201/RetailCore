import { Badge, Button } from "@heroui/react";
import { Bell, PanelLeftClose, PanelLeftOpen } from "lucide-react";

interface HeaderProps {
  sidebarCollapsed: boolean;
  toggleSidebar: () => void;
}

export default function Header({
  sidebarCollapsed,
  toggleSidebar,
}: Readonly<HeaderProps>) {
  return (
    <header className="h-16 bg-white border-b flex items-center justify-between px-4">
      <Button
        isIconOnly
        variant="ghost"
        size="lg"
        onClick={() => toggleSidebar()}
        className="rounded-md hover:bg-gray-100"
      >
        {sidebarCollapsed ? (
          <PanelLeftOpen className="size-6" />
        ) : (
          <PanelLeftClose className="size-6" />
        )}
      </Button>

      <div className="font-bold text-gray-700">Admin Portal</div>

      <Badge.Anchor>
        <Button isIconOnly variant="tertiary" size="lg">
          <Bell className="size-5" />
        </Button>
        <Badge color="danger" size="sm">
          5
        </Badge>
      </Badge.Anchor>
    </header>
  );
}
