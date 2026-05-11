export function formatPrice(price: number | undefined | null): string {
  if (price === null || price === undefined) return "N/A";

  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    minimumFractionDigits: 0,
    maximumFractionDigits: 2,
  }).format(price);
}

export function formatPriceRange(
  minPrice: number | undefined | null,
  maxPrice: number | undefined | null,
): string {
  if (!minPrice && !maxPrice) return "N/A";
  if (minPrice === maxPrice) return formatPrice(minPrice);
  return `${formatPrice(minPrice)} - ${formatPrice(maxPrice)}`;
}

export function calculateDiscountPercentage(
  price: number,
  compareAtPrice: number | undefined | null,
): number | null {
  if (!compareAtPrice || compareAtPrice <= 0) return null;
  if (price >= compareAtPrice) return null;

  const discount = ((compareAtPrice - price) / compareAtPrice) * 100;
  return Math.round(discount);
}

export function formatDiscountPercentage(
  price: number,
  compareAtPrice: number | undefined | null,
): string {
  const discount = calculateDiscountPercentage(price, compareAtPrice);
  if (discount === null) return "";
  return `${discount}%`;
}
