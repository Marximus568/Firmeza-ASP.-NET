import apiClient from '@/services/api';
import { API_ENDPOINTS } from '@/lib/constants';

/**
 * Sales Service
 * Handles all sales-related API calls
 */

/**
 * Register a sale and generate PDF receipt
 * @param {import('../types/sales.types').SaleReceiptDto} saleData - Sale data
 * @returns {Promise<{message: string, pdf: string}>} - API response with PDF URL
 */
export const createSale = async (saleData) => {
    const response = await apiClient.post(API_ENDPOINTS.SALES.REGISTER, saleData);
    return response.data;
};

/**
 * Get PDF download URL
 * @param {string} filename - PDF filename
 * @returns {string} - Full download URL
 */
export const getPdfDownloadUrl = (filename) => {
    return `${apiClient.defaults.baseURL}${API_ENDPOINTS.SALES.DOWNLOAD}?file=${filename}`;
};
