// src/features/cart/stores/cart.store.js
import { create } from "zustand";

/**
 * Helper to convert potential strings to numbers safely
 */
const toNumber = (v) => {
    const n = Number(v);
    return Number.isNaN(n) ? 0 : n;
};

/**
 * Shopping Cart Store (Zustand)
 * Manages cart items and totals
 */
const useCartStore = create((set, get) => ({
    // State
    items: [],

    // Computed totals (always derived reactively)
    get subtotal() {
        return get().items.reduce((sum, item) => {
            const price = toNumber(item.unitPrice);
            const qty = toNumber(item.quantity);
            return sum + price * qty;
        }, 0);
    },

    get iva() {
        return get().subtotal * 0.16;
    },

    get totalWithTax() {
        return get().subtotal + get().iva;
    },

    get itemCount() {
        return get().items.reduce((sum, item) => sum + toNumber(item.quantity), 0);
    },

    /**
     * Add an item to the cart
     */
    addItem: (product) => {
        set((state) => {
            const existingItem = state.items.find((item) => item.id === product.id);

            if (existingItem) {
                // Already exists → increment
                return {
                    items: state.items.map((item) =>
                        item.id === product.id
                            ? {
                                  ...item,
                                  quantity: Math.min(
                                      toNumber(item.quantity) + 1,
                                      toNumber(product.stock)
                                  ),
                              }
                            : item
                    ),
                };
            }

            // New item
            return {
                items: [
                    ...state.items,
                    {
                        id: product.id,
                        name: product.name,
                        unitPrice: toNumber(product.unitPrice),
                        quantity: 1,
                        stock: toNumber(product.stock),
                    },
                ],
            };
        });
    },

    /**
     * Remove item by ID
     */
    removeItem: (productId) => {
        set((state) => ({
            items: state.items.filter((item) => item.id !== productId),
        }));
    },

    /**
     * Update item quantity
     */
    updateQuantity: (productId, quantity) => {
        set((state) => ({
            items: state.items.map((item) =>
                item.id === productId
                    ? {
                          ...item,
                          quantity: Math.min(
                              Math.max(1, toNumber(quantity)),
                              toNumber(item.stock)
                          ),
                      }
                    : item
            ),
        }));
    },

    /**
     * Clear cart
     */
    clearCart: () => set({ items: [] }),
}));

export default useCartStore;
