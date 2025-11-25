import { create } from 'zustand';

/**
 * Shopping Cart Store (Zustand)
 * Manages cart items and totals
 */
const useCartStore = create((set, get) => ({
    // State
    items: [],

    // Computed getters
    get total() {
        return get().items.reduce((sum, item) => sum + (item.unitPrice * item.quantity), 0);
    },

    get itemCount() {
        return get().items.reduce((sum, item) => sum + item.quantity, 0);
    },

    get subtotal() {
        return get().total;
    },

    get iva() {
        // 16% IVA (adjust as needed)
        return get().subtotal * 0.16;
    },

    get totalWithTax() {
        return get().subtotal + get().iva;
    },

    // Actions

    /**
     * Add item to cart
     * @param {import('../../products/types/products.types').ProductDto} product
     */
    addItem: (product) => {
        set((state) => {
            const existingItem = state.items.find((item) => item.id === product.id);

            if (existingItem) {
                // Increment quantity if item already in cart
                return {
                    items: state.items.map((item) =>
                        item.id === product.id
                            ? { ...item, quantity: Math.min(item.quantity + 1, product.stock) }
                            : item
                    ),
                };
            } else {
                // Add new item
                return {
                    items: [
                        ...state.items,
                        {
                            id: product.id,
                            name: product.name,
                            unitPrice: product.unitPrice,
                            quantity: 1,
                            stock: product.stock,
                        },
                    ],
                };
            }
        });
    },

    /**
     * Remove item from cart
     * @param {number} productId
     */
    removeItem: (productId) => {
        set((state) => ({
            items: state.items.filter((item) => item.id !== productId),
        }));
    },

    /**
     * Update item quantity
     * @param {number} productId
     * @param {number} quantity
     */
    updateQuantity: (productId, quantity) => {
        set((state) => ({
            items: state.items.map((item) =>
                item.id === productId
                    ? { ...item, quantity: Math.min(Math.max(1, quantity), item.stock) }
                    : item
            ),
        }));
    },

    /**
     * Clear entire cart
     */
    clearCart: () => {
        set({ items: [] });
    },
}));

export default useCartStore;
