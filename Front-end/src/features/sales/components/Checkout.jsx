import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import useSales from '../hooks/useSales';
import useCartStore from '../stores/cart.store';
import useAuthStore from '@/features/auth/stores/auth.store';
import { ROUTES } from '@/lib/constants';

/**
 * Checkout Page Component
 * Handles customer info collection and order placement
 */
const Checkout = () => {
    const navigate = useNavigate();
    const { checkout, isLoading, error } = useSales();
    const { items, subtotal, iva, totalWithTax } = useCartStore();
    const user = useAuthStore((state) => state.user);

    const [customerInfo, setCustomerInfo] = useState({
        customerName: user ? `${user.firstName} ${user.lastName}` : '',
        customerEmail: user?.email || '',
    });

    const [orderComplete, setOrderComplete] = useState(false);
    const [pdfUrl, setPdfUrl] = useState(null);

    // Redirect if cart is empty
    if (items.length === 0 && !orderComplete) {
        navigate(ROUTES.CART);
        return null;
    }

    const handleChange = (e) => {
        const { name, value } = e.target;
        setCustomerInfo((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        const result = await checkout(customerInfo);

        if (result.success) {
            setOrderComplete(true);
            setPdfUrl(result.pdfUrl);
        }
    };

    if (orderComplete) {
        return (
            <div className="checkout-container">
                <div className="checkout-success">
                    <div className="success-icon">
                        <svg className="checkmark" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 52 52">
                            <circle className="checkmark__circle" cx="26" cy="26" r="25" fill="none" />
                            <path className="checkmark__check" fill="none" d="M14.1 27.2l7.1 7.2 16.7-16.8" />
                        </svg>
                    </div>

                    <h1 className="text-3xl font-bold text-gray-800 mb-4">¡Orden Completada!</h1>
                    <p className="text-lg text-gray-600 mb-6">Tu pedido ha sido procesado exitosamente.</p>

                    {/* Email confirmation notice */}
                    <div className="email-notification bg-blue-50 border border-blue-200 rounded-lg p-6 mb-6">
                        <div className="flex items-center gap-3 mb-3">
                            <svg className="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                            </svg>
                            <h3 className="text-lg font-semibold text-blue-900">Comprobante Enviado</h3>
                        </div>
                        <p className="text-blue-800">
                            Se ha enviado un comprobante detallado de tu compra al correo electrónico <strong>{customerInfo.customerEmail}</strong>
                        </p>
                        <p className="text-sm text-blue-600 mt-2">
                            Por favor, revisa tu bandeja de entrada (y la carpeta de spam si no lo encuentras).
                        </p>
                    </div>

                    {/* PDF download section */}
                    {pdfUrl && (
                        <div className="pdf-download bg-gray-50 border border-gray-200 rounded-lg p-6 mb-6">
                            <div className="flex items-center gap-3 mb-3">
                                <svg className="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
                                </svg>
                                <h3 className="text-lg font-semibold text-gray-900">Descargar Recibo</h3>
                            </div>
                            <p className="text-gray-700 mb-4">También puedes descargar el comprobante en formato PDF:</p>
                            <a
                                href={pdfUrl}
                                download
                                className="inline-flex items-center gap-2 bg-gradient-to-r from-primary-500 to-secondary-500 text-white px-6 py-3 rounded-lg font-semibold hover:shadow-lg transition-all duration-300"
                                target="_blank"
                                rel="noopener noreferrer"
                            >
                                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                                </svg>
                                Descargar Comprobante (PDF)
                            </a>
                        </div>
                    )}

                    <button
                        onClick={() => navigate(ROUTES.PRODUCTS)}
                        className="w-full bg-gray-200 hover:bg-gray-300 text-gray-800 font-semibold px-6 py-3 rounded-lg transition-colors duration-300"
                    >
                        Continuar Comprando
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="checkout-container">
            <h1 className="checkout-title">Checkout</h1>

            <div className="checkout-layout">
                <div className="checkout-form-section">
                    <h2>Customer Information</h2>

                    {error && (
                        <div className="error-message">{error}</div>
                    )}

                    <form onSubmit={handleSubmit} className="checkout-form">
                        <div className="form-group">
                            <label htmlFor="customerName">Full Name</label>
                            <input
                                id="customerName"
                                type="text"
                                name="customerName"
                                value={customerInfo.customerName}
                                onChange={handleChange}
                                placeholder="John Doe"
                                required
                                disabled={isLoading}
                            />
                        </div>

                        <div className="form-group">
                            <label htmlFor="customerEmail">Email</label>
                            <input
                                id="customerEmail"
                                type="email"
                                name="customerEmail"
                                value={customerInfo.customerEmail}
                                onChange={handleChange}
                                placeholder="john@example.com"
                                required
                                disabled={isLoading}
                            />
                        </div>

                        <button
                            type="submit"
                            className="btn-primary btn-full"
                            disabled={isLoading}
                        >
                            {isLoading ? 'Processing...' : 'Complete Order'}
                        </button>
                    </form>
                </div>

                <div className="checkout-summary-section">
                    <h2>Order Summary</h2>

                    <div className="order-items">
                        {items.map((item) => (
                            <div key={item.id} className="order-item">
                                <span>{item.name} × {item.quantity}</span>
                                <span>${(item.unitPrice * item.quantity).toFixed(2)}</span>
                            </div>
                        ))}
                    </div>

                    <div className="order-totals">
                        <div className="total-row">
                            <span>Subtotal:</span>
                            <span>${subtotal.toFixed(2)}</span>
                        </div>
                        <div className="total-row">
                            <span>IVA (16%):</span>
                            <span>${iva.toFixed(2)}</span>
                        </div>
                        <div className="total-row total-final">
                            <span>Total:</span>
                            <span>${totalWithTax.toFixed(2)}</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Checkout;
