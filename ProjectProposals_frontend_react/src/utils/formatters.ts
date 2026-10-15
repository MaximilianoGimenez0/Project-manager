/**
 * Utilities for locale-aware numerical and currency formatting.
 * Consistently applies dot ('.') as thousands and millions separator:
 * Example: 12000 -> "12.000", 1500000 -> "1.500.000"
 */

/**
 * Formats a number with dots as thousands and millions separators.
 * Handles integer and decimal values safely.
 *
 * @param value - The numeric value or string number to format
 * @returns Formatted string with dot thousands separator (e.g. "12.000")
 */
export function formatNumberWithDots(value: number | string | null | undefined): string {
  if (value === null || value === undefined || value === "") {
    return "0";
  }

  const num = typeof value === "string" ? parseFloat(value.replace(/,/g, "")) : value;
  if (isNaN(num)) {
    return "0";
  }

  // Use es-AR / de-DE locale pattern: dots for thousands, comma for decimals
  const parts = num.toString().split(".");
  const integerPart = parts[0];
  const decimalPart = parts[1];

  const formattedInteger = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, ".");

  return decimalPart !== undefined ? `${formattedInteger},${decimalPart}` : formattedInteger;
}

/**
 * Formats a monetary amount into a clean currency string with dot separators.
 * Example: 12000 -> "$ 12.000", 1500000 -> "$ 1.500.000"
 *
 * @param amount - The numeric amount
 * @param symbol - The currency symbol (default: "$")
 * @returns Formatted currency string
 */
export function formatCurrency(
  amount: number | string | null | undefined,
  symbol: string = "$"
): string {
  const formattedNumber = formatNumberWithDots(amount);
  return `${symbol} ${formattedNumber}`;
}
