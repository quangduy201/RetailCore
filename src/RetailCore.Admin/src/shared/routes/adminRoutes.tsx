import type { JSX } from "react";
import DashboardPage from "../../features/dashboard/pages/DashboardPage";
import ProductsPage from "../../features/products/pages/ProductsPage";
import CategoriesPage from "../../features/categories/pages/CategoriesPage";
import BrandsPage from "../../features/brands/pages/BrandsPage";
import OrdersPage from "../../features/orders/pages/OrdersPage";
import CustomersPage from "../../features/customers/pages/CustomersPage";

export type RouteConfig = {
  path: string;
  element: JSX.Element;
};

export const adminRoutes: RouteConfig[] = [
  { path: "/", element: <DashboardPage /> },
  { path: "/products", element: <ProductsPage /> },
  { path: "/categories", element: <CategoriesPage /> },
  { path: "/brands", element: <BrandsPage /> },
  { path: "/orders", element: <OrdersPage /> },
  { path: "/customers", element: <CustomersPage /> },
];
