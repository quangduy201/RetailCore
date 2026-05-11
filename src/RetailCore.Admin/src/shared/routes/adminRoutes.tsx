import type { JSX } from "react";
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
  { path: "/", element: <DashboardPage /> },
  { path: "/products", element: <ProductsPage /> },
  { path: "/products/create", element: <ProductCreatePage /> },
  { path: "/products/:id/setup", element: <ProductSetupPage /> },
  { path: "/products/:id", element: <ProductDetailPage /> },
  { path: "/categories", element: <CategoriesPage /> },
  { path: "/categories/create", element: <CategoryCreatePage /> },
  { path: "/categories/:id/edit", element: <CategoryEditPage /> },
  { path: "/brands", element: <BrandsPage /> },
  { path: "/brands/create", element: <BrandCreatePage /> },
  { path: "/brands/:id/edit", element: <BrandEditPage /> },
  { path: "/orders", element: <OrdersPage /> },
  { path: "/customers", element: <CustomersPage /> },
];
