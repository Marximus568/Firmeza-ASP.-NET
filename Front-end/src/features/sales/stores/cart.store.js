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
 * Helper function to compute totals from items
 */
const computeTotals = (items) => {
    const subtotal = items.reduce((sum, item) => {
        const price = toNumber(item.unitPrice);
        const qty = toNumber(item.quantity);
        return sum + price * qty;
    }, 0);

    const iva = subtotal * 0.16;
    const totalWithTax = subtotal + iva;
    const itemCount = items.reduce((sum, item) => sum + toNumber(item.quantity), 0);

    return { subtotal, iva, totalWithTax, itemCount };
};

/**
 * Shopping Cart Store (Zustand)
 * Manages cart items and totals
 */
const useCartStore = create((set, get) => ({
    // State
    items: [],
    subtotal: 0,
    iva: 0,
    totalWithTax: 0,
    itemCount: 0,

    /**
     * Add an item to the cart
     */
    addItem: (product) => {
        set((state) => {
            const existingItem = state.items.find((item) => item.id === product.id);

            let newItems;
            if (existingItem) {
                // Already exists → increment
                newItems = state.items.map((item) =>
                    item.id === product.id
                        ? {
                            ...item,
                            quantity: Math.min(
                                toNumber(item.quantity) + 1,
                                toNumber(product.stock)
                            ),
                        }
                        : item
                );
            } else {
                // New item
                newItems = [
                    ...state.items,
                    {
                        id: product.id,
                        name: product.name,
                        unitPrice: toNumber(product.unitPrice),
                        quantity: 1,
                        stock: toNumber(product.stock),
                    },
                ];
            }

            const totals = computeTotals(newItems);
            return { items: newItems, ...totals };
        });
    },

    /**
     * Remove item by ID
     */
    removeItem: (productId) => {
        set((state) => {
            const newItems = state.items.filter((item) => item.id !== productId);
            const totals = computeTotals(newItems);
            return { items: newItems, ...totals };
        });
    },

    /**
     * Update item quantity
     */
    updateQuantity: (productId, quantity) => {
        set((state) => {
            const newItems = state.items.map((item) =>
                item.id === productId
                    ? {
                        ...item,
                        quantity: Math.min(
                            Math.max(1, toNumber(quantity)),
                            toNumber(item.stock)
                        ),
                    }
                    : item
            );
            const totals = computeTotals(newItems);
            return { items: newItems, ...totals };
        });
    },

    /**
     * Clear cart
     */
    clearCart: () => set({ items: [], subtotal: 0, iva: 0, totalWithTax: 0, itemCount: 0 }),
}));

export default useCartStore;
