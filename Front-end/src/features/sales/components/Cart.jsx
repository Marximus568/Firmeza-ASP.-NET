import { Link, useNavigate } from 'react-router-dom';
import useCartStore from '../stores/cart.store';
import { ROUTES } from '@/lib/constants';

/**
 * Shopping Cart Page Component
 * Displays cart items with quantity controls
 */
const Cart = () => {
    const navigate = useNavigate();
    const {
        items,
        subtotal,
        iva,
        totalWithTax,
        removeItem,
        updateQuantity,
    } = useCartStore();

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

            <div className="cart-items">
                {items.map((item) => (
                    <div key={item.id} className="cart-item">
                        <div className="cart-item-info">
                            <h3>{item.name}</h3>
                            <p className="cart-item-price">${item.unitPrice.toFixed(2)} each</p>
                        </div>

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

                            <div className="cart-item-total">
                                ${(item.unitPrice * item.quantity).toFixed(2)}
                            </div>

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
