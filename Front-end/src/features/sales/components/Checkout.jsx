import { useState, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import useSales from "../hooks/useSales";
import useCartStore from "../stores/cart.store";
import useAuthStore from "@/features/auth/stores/auth.store";
import { ROUTES } from "@/lib/constants";

const Checkout = () => {
    const navigate = useNavigate();

    // Get store reactive data (correct Zustand selectors)
    const items = useCartStore((state) => state.items);

    // Totals calculated with useMemo (recommended)
    const { subtotal, iva, totalWithTax } = useMemo(() => {
        const subtotalCalc = items.reduce(
            (acc, item) => acc + item.unitPrice * item.quantity,
            0
        );

        const ivaCalc = subtotalCalc * 0.16;
        const totalCalc = subtotalCalc + ivaCalc;

        return {
            subtotal: subtotalCalc,
            iva: ivaCalc,
            totalWithTax: totalCalc
        };
    }, [items]);

    const { checkout, isLoading, error } = useSales();
    const user = useAuthStore((state) => state.user);

    const [customerInfo, setCustomerInfo] = useState({
        customerName: user ? `${user.firstName} ${user.lastName}` : "",
        customerEmail: user?.email || "",
    });

    const [orderComplete, setOrderComplete] = useState(false);
    const [pdfUrl, setPdfUrl] = useState(null);

    // Redirect if cart is empty (except after order completion)
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

    // ---------------------------
    // SUCCESS PAGE
    // ---------------------------
    if (orderComplete) {
        return (
            <div className="checkout-container">
                <div className="checkout-success">
                    <h1 className="text-3xl font-bold text-gray-800 mb-4">
                        ¡Orden Completada!
                    </h1>

                    <p className="text-lg text-gray-600 mb-6">
                        Tu pedido ha sido procesado exitosamente.
                    </p>

                    {/* Email confirmation info */}
                    <div className="email-notification bg-blue-50 border border-blue-200 rounded-lg p-6 mb-6">
                        <h3 className="text-lg font-semibold text-blue-900">Comprobante Enviado</h3>
                        <p className="text-blue-800">
                            Se ha enviado un comprobante al correo:{" "}
                            <strong>{customerInfo.customerEmail}</strong>
                        </p>
                    </div>

                    {/* PDF download */}
                    {pdfUrl && (
                        <div className="pdf-download bg-gray-50 border border-gray-200 rounded-lg p-6 mb-6">
                            <h3 className="text-lg font-semibold text-gray-900 mb-3">
                                Descargar Recibo
                            </h3>

                            <a
                                href={pdfUrl}
                                download
                                target="_blank"
                                rel="noopener noreferrer"
                                className="inline-flex items-center gap-2 bg-gradient-to-r from-primary-500 to-secondary-500 text-white px-6 py-3 rounded-lg font-semibold"
                            >
                                Descargar Comprobante (PDF)
                            </a>
                        </div>
                    )}

                    <button
                        onClick={() => navigate(ROUTES.PRODUCTS)}
                        className="w-full bg-gray-200 hover:bg-gray-300 text-gray-800 font-semibold px-6 py-3 rounded-lg"
                    >
                        Continuar Comprando
                    </button>
                </div>
            </div>
        );
    }

    // ---------------------------
    // CHECKOUT PAGE
    // ---------------------------
    return (
        <div className="checkout-container">
            <h1 className="checkout-title">Checkout</h1>

            <div className="checkout-layout">
                {/* LEFT SIDE FORM */}
                <div className="checkout-form-section">
                    <h2>Customer Information</h2>

                    {error && <div className="error-message">{error}</div>}

                    <form onSubmit={handleSubmit} className="checkout-form">
                        <div className="form-group">
                            <label htmlFor="customerName">Full Name</label>
                            <input
                                id="customerName"
                                type="text"
                                name="customerName"
                                value={customerInfo.customerName}
                                onChange={handleChange}
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
                                required
                                disabled={isLoading}
                            />
                        </div>

                        <button
                            type="submit"
                            className="btn-primary btn-full"
                            disabled={isLoading}
                        >
                            {isLoading ? "Processing..." : "Complete Order"}
                        </button>
                    </form>
                </div>

                {/* RIGHT SIDE ORDER SUMMARY */}
                <div className="checkout-summary-section">
                    <h2>Order Summary</h2>

                    <div className="order-items">
                        {items.map((item) => (
                            <div key={item.id} className="order-item">
                                <span>
                                    {item.name} × {item.quantity}
                                </span>
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
