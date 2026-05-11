import { useLocation, useNavigate } from "react-router-dom";
import { sidebarConfig } from "../../config/sidebarConfig";
import { ChevronDown } from "lucide-react";
import { Accordion, Avatar } from "@heroui/react";

interface SidebarProps {
  collapsed: boolean;
}

export default function Sidebar({ collapsed }: Readonly<SidebarProps>) {
  const navigate = useNavigate();
  const location = useLocation();

  const isActive = (path: string | undefined) =>
    path && location.pathname.startsWith(path);

  return (
    <div
      className={`h-screen bg-zinc-900 text-white flex flex-col transition-all duration-300 ${
        collapsed ? "w-20" : "w-64"
      }`}
      style={{
        scrollbarWidth: "thin",
      }}
    >
      {/* Logo */}
      <div className="h-16 flex items-center justify-center font-bold border-b border-zinc-800">
        {collapsed ? "RC" : "RetailCore"}
      </div>

      {/* Menu */}
      <Accordion
        className="p-2 pr-0 flex-1 overflow-y-scroll"
        allowsMultipleExpanded
        hideSeparator
      >
        {sidebarConfig.map((item) => {
          const hasChildren = !!item.children?.length;

          return (
            <Accordion.Item
              key={item.key}
              id={item.key}
              className="mt-2 first:mt-0"
            >
              <Accordion.Heading>
                <Accordion.Trigger
                  onPress={() => {
                    if (!hasChildren && item.path) navigate(item.path);
                  }}
                  className={`flex justify-start items-center gap-3 px-3 py-2 rounded-md text-base
                    ${isActive(item.path) ? "bg-blue-600 text-white" : "text-zinc-400 hover:bg-zinc-800"}`}
                >
                  {item.icon && <item.icon />}

                  {!collapsed && (
                    <span
                      className={`transition-all duration-200 whitespace-nowrap overflow-hidden
                        ${collapsed ? "opacity-0 w-0" : "opacity-100 w-auto ml-2 delay-150"}`}
                    >
                      {item.label}
                    </span>
                  )}

                  {!collapsed && hasChildren && (
                    <Accordion.Indicator className="ml-auto">
                      <ChevronDown />
                    </Accordion.Indicator>
                  )}
                </Accordion.Trigger>
              </Accordion.Heading>

              {hasChildren && (
                <Accordion.Panel>
                  <div className="my-1 pl-2 flex flex-col gap-1">
                    {item.children?.map((child) => (
                      <button
                        key={child.key}
                        onClick={() => navigate(`${child.path}`)}
                        className={`flex items-center gap-3 px-3 py-2 rounded-md text-sm
                          ${isActive(child.path) ? "bg-blue-600 text-white" : "text-zinc-400 hover:bg-zinc-800"}`}
                      >
                        {child.icon && <child.icon />}

                        {!collapsed && (
                          <span
                            className={`transition-all duration-200 whitespace-nowrap overflow-hidden
                              ${collapsed ? "opacity-0 w-0" : "opacity-100 w-auto ml-2 delay-150"}`}
                          >
                            {child.label}
                          </span>
                        )}
                      </button>
                    ))}
                  </div>
                </Accordion.Panel>
              )}
            </Accordion.Item>
          );
        })}
      </Accordion>

      {/* Bottom section (fixed) */}
      <div className="border-t border-zinc-800 p-2">
        <button className="flex items-center gap-3 w-full px-3 py-2 rounded-md hover:bg-zinc-800 transition">
          <div className="relative">
            <Avatar size="md">
              <Avatar.Image src="https://0.gravatar.com/avatar/acec1add229b2a6d5a23d122e0e1e6450fb773b06dc83c3bcd7ed4456842facc?size=256&d=initials" />
            </Avatar>

            <span className="absolute bottom-0 right-0 w-2 h-2 bg-green-500 rounded-full ring-2 ring-zinc-900" />
          </div>

          {!collapsed && (
            <div className="flex flex-col text-left min-w-0">
              <span className="text-sm font-medium truncate">Admin User</span>
              <span className="text-xs text-zinc-400 truncate">
                admin@email.com
              </span>
            </div>
          )}
        </button>
      </div>
    </div>
  );
}
