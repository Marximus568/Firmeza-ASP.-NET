import apiClient from '@/services/api';
import { API_ENDPOINTS } from '@/lib/constants';

/**
 * Products Service
 * Handles all product-related API calls
 */

/**
 * Fetch all products
 * @returns {Promise<import('../types/products.types').ProductDto[]>} - Array of products
 */
export const getProducts = async () => {
    const response = await apiClient.get(API_ENDPOINTS.PRODUCTS.BASE);
    return response.data;
};

/**
 * Fetch single product by ID
 * @param {number} id - Product ID
 * @returns {Promise<import('../types/products.types').ProductDto>} - Single product
 */
export const getProduct = async (id) => {
    const response = await apiClient.get(API_ENDPOINTS.PRODUCTS.BY_ID(id));
    return response.data;
};

/**
 * Update an existing product
 * @param {number} id - Product ID
 * @param {import('../types/products.types').CreateProductDto} data - Product data to update
 * @returns {Promise<void>} - No content on success
 */
export const updateProduct = async (id, data) => {
    const response = await apiClient.put(API_ENDPOINTS.PRODUCTS.BY_ID(id), data);
    return response.data;
};
