import {
  FolderTree,
  Grid2X2,
  LayoutDashboard,
  ShoppingBag,
  ShoppingCart,
  Tag,
  Users,
} from "lucide-react";
import type React from "react";

export interface SidebarItem {
  key: string;
  label: string;
  icon?: React.FC<any>;
  path?: string;
  children?: SidebarItem[];
}

export const sidebarConfig: SidebarItem[] = [
  {
    key: "dashboard",
    label: "Dashboard",
    path: "/",
    icon: LayoutDashboard,
  },
  {
    key: "catalog",
    label: "Catalog",
    icon: Grid2X2,
    children: [
      {
        key: "products",
        label: "Products",
        icon: ShoppingBag,
        path: "/products",
      },
      {
        key: "categories",
        label: "Categories",
        icon: FolderTree,
        path: "/categories",
      },
      {
        key: "brands",
        label: "Brands",
        icon: Tag,
        path: "/brands",
      },
    ],
  },
  {
    key: "orders",
    label: "Orders",
    icon: ShoppingCart,
    path: "/orders",
  },
  {
    key: "customers",
    label: "Customers",
    icon: Users,
    path: "/customers",
  },
];
