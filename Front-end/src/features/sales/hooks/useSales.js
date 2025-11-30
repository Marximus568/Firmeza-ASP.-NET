import { useState } from 'react';
import * as salesService from '../services/sales.service';
import useCartStore from '../stores/cart.store';

/**
 * Custom hook for sales/checkout operations
 */
const useSales = () => {
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);
    const [pdfUrl, setPdfUrl] = useState(null);

    const { items, subtotal, iva, totalWithTax, clearCart } = useCartStore();

    /**
     * Checkout - create sale and get PDF receipt
     * @param {Object} customerInfo
     * @param {string} customerInfo.customerName
     * @param {string} customerInfo.customerEmail
     */
    const checkout = async (customerInfo) => {
        setIsLoading(true);
        setError(null);
        setPdfUrl(null);

        try {
            // Prepare sale data
            const saleReceiptDto = {
                saleId: 0, // Backend will assign
                date: new Date().toISOString(),
                customerName: customerInfo.customerName,
                customerEmail: customerInfo.customerEmail,
                products: items.map((item) => ({
                    name: item.name,
                    qty: item.quantity,
                    unitPrice: item.unitPrice,
                })),
                subtotal,
                iva,
                total: totalWithTax,
            };

            const response = await salesService.createSale(saleReceiptDto);

            // Extract PDF filename from response
            const pdfFilename = response.pdf.split('/').pop();
            const downloadUrl = salesService.getPdfDownloadUrl(pdfFilename);

            setPdfUrl(downloadUrl);
            clearCart();

            return { success: true, pdfUrl: downloadUrl };
        } catch (err) {
            const errorMessage = err.response?.data?.message || 'Failed to complete checkout';
            setError(errorMessage);
            console.error('Checkout error:', err);
            return { success: false, error: errorMessage };
        } finally {
            setIsLoading(false);
        }
    };

    return {
        checkout,
        isLoading,
        error,
        pdfUrl,
    };
};

export default useSales;
