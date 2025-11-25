/**
 * Sales Type Definitions
 */

/**
 * @typedef {Object} SaleProductDto
 * @property {number} productId - Product ID
 * @property {string} productName - Product name
 * @property {number} quantity - Quantity purchased
 * @property {number} unitPrice - Price per unit
 * @property {number} total - Total for this line item
 */

/**
 * @typedef {Object} SaleReceiptDto
 * @property {number} saleId - Sale ID
 * @property {string} date - Sale date (ISO format)
 * @property {string} customerName - Customer full name
 * @property {string} customerEmail - Customer email
 * @property {SaleProductDto[]} products - List of products
 * @property {number} subtotal - Subtotal amount
 * @property {number} iva - Tax amount
 * @property {number} total - Total amount
 */

/**
 * @typedef {Object} CartItem
 * @property {number} id - Product ID
 * @property {string} name - Product name
 * @property {number} unitPrice - Unit price
 * @property {number} quantity - Quantity in cart
 * @property {number} stock - Available stock
 */

export { };
