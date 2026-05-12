import type { JSX } from "react";
import AdminProfilePage from "@/features/auth/pages/AdminProfilePage";
import DashboardPage from "@/features/dashboard/pages/DashboardPage";
import ProductsPage from "@/features/products/pages/ProductsPage";
import ProductCreatePage from "@/features/products/pages/ProductCreatePage";
import ProductSetupPage from "@/features/products/pages/ProductSetupPage";
import ProductDetailPage from "@/features/products/pages/ProductDetailPage";
import CategoriesPage from "@/features/categories/pages/CategoriesPage";
import CategoryCreatePage from "@/features/categories/pages/CategoryCreatePage";
import CategoryEditPage from "@/features/categories/pages/CategoryEditPage";
import BrandsPage from "@/features/brands/pages/BrandsPage";
import BrandCreatePage from "@/features/brands/pages/BrandCreatePage";
import BrandEditPage from "@/features/brands/pages/BrandEditPage";
import OrdersPage from "@/features/orders/pages/OrdersPage";
import CustomersPage from "@/features/customers/pages/CustomersPage";

export type RouteConfig = {
  path: string;
  element: JSX.Element;
};

export const adminRoutes: RouteConfig[] = [
  { path: "/admin", element: <DashboardPage /> },
  { path: "/admin/profile", element: <AdminProfilePage /> },
  { path: "/admin/dashboard", element: <DashboardPage /> },
  { path: "/admin/products", element: <ProductsPage /> },
  { path: "/admin/products/create", element: <ProductCreatePage /> },
  { path: "/admin/products/:id/setup", element: <ProductSetupPage /> },
  { path: "/admin/products/:id", element: <ProductDetailPage /> },
  { path: "/admin/categories", element: <CategoriesPage /> },
  { path: "/admin/categories/create", element: <CategoryCreatePage /> },
  { path: "/admin/categories/:id/edit", element: <CategoryEditPage /> },
  { path: "/admin/brands", element: <BrandsPage /> },
  { path: "/admin/brands/create", element: <BrandCreatePage /> },
  { path: "/admin/brands/:id/edit", element: <BrandEditPage /> },
  { path: "/admin/orders", element: <OrdersPage /> },
  { path: "/admin/customers", element: <CustomersPage /> },
];
