import { Link, useNavigate } from 'react-router-dom';
import useCartStore from '../stores/cart.store';
import { ROUTES } from '@/lib/constants';

/**
 * Shopping Cart Page Component
 * Renders the cart items, quantity controls, and order summary
 */
const Cart = () => {
    const navigate = useNavigate();

    // Cart state selectors
    const items = useCartStore((state) => state.items);
    const subtotal = useCartStore((state) => state.subtotal);
    const iva = useCartStore((state) => state.iva);
    const totalWithTax = useCartStore((state) => state.totalWithTax);

    // Cart actions
    const removeItem = useCartStore((state) => state.removeItem);
    const updateQuantity = useCartStore((state) => state.updateQuantity);

    // Render message when cart is empty
    if (items.length === 0) {
        return (
            <div className="cart-container">
                <div className="cart-empty">
                    <h2>Your cart is empty</h2>
                    <p>Add some products to get started</p>
                    <Link to={ROUTES.PRODUCTS} className="btn-primary">
                        Browse Products
                    </Link>
                </div>
            </div>
        );
    }

    return (
        <div className="cart-container">
            <h1 className="cart-title">Shopping Cart</h1>

            {/* Cart item list */}
            <div className="cart-items">
                {items.map((item) => (
                    <div key={item.id} className="cart-item">
                        <div className="cart-item-info">
                            <h3>{item.name}</h3>
                            <p className="cart-item-price">${item.unitPrice.toFixed(2)} each</p>
                        </div>

                        {/* Quantity controls and item actions */}
                        <div className="cart-item-controls">
                            <div className="quantity-controls">
                                <button
                                    onClick={() => updateQuantity(item.id, item.quantity - 1)}
                                    disabled={item.quantity <= 1}
                                    className="btn-quantity"
                                >
                                    -
                                </button>

                                <span className="quantity-value">{item.quantity}</span>

                                <button
                                    onClick={() => updateQuantity(item.id, item.quantity + 1)}
                                    disabled={item.quantity >= item.stock}
                                    className="btn-quantity"
                                >
                                    +
                                </button>
                            </div>

                            {/* Total price for this item */}
                            <div className="cart-item-total">
                                ${(item.unitPrice * item.quantity).toFixed(2)}
                            </div>

                            {/* Remove item button */}
                            <button
                                onClick={() => removeItem(item.id)}
                                className="btn-remove"
                            >
                                Remove
                            </button>
                        </div>
                    </div>
                ))}
            </div>

            {/* Order summary */}
            <div className="cart-summary">
                <div className="summary-row">
                    <span>Subtotal:</span>
                    <span>${subtotal.toFixed(2)}</span>
                </div>

                <div className="summary-row">
                    <span>IVA (16%):</span>
                    <span>${iva.toFixed(2)}</span>
                </div>

                <div className="summary-row summary-total">
                    <span>Total:</span>
                    <span>${totalWithTax.toFixed(2)}</span>
                </div>

                {/* Summary actions */}
                <div className="cart-actions">
                    <Link to={ROUTES.PRODUCTS} className="btn-secondary">
                        Continue Shopping
                    </Link>

                    <button
                        onClick={() => navigate(ROUTES.CHECKOUT)}
                        className="btn-primary"
                    >
                        Proceed to Checkout
                    </button>
                </div>
            </div>
        </div>
    );
};

export default Cart;
