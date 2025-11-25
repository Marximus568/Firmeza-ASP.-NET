import apiClient from '@/services/api';
import { API_ENDPOINTS, STORAGE_KEYS } from '@/lib/constants';

/**
 * Authentication Service
 * Handles all authentication-related API calls
 */

/**
 * Register a new user
 * @param {import('../types/auth.types').RegisterDto} data - Registration data
 * @returns {Promise<Object>} - API response
 */
export const register = async (data) => {
    const response = await apiClient.post(API_ENDPOINTS.AUTH.REGISTER, data);
    return response.data;
};

/**
 * Login user and retrieve JWT token
 * @param {string} email - User email
 * @param {string} password - User password
 * @returns {Promise<import('../types/auth.types').AuthResponse>} - Auth response with token
 */
export const login = async (email, password) => {
    const response = await apiClient.post(API_ENDPOINTS.AUTH.LOGIN, {
        email,
        password,
    });
    return response.data;
};

/**
 * Logout user by clearing local storage
 */
export const logout = () => {
    localStorage.removeItem(STORAGE_KEYS.AUTH_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.USER_DATA);
};

/**
 * Get current user from local storage
 * @returns {import('../types/auth.types').UserData | null} - User data or null
 */
export const getCurrentUser = () => {
    const userData = localStorage.getItem(STORAGE_KEYS.USER_DATA);
    return userData ? JSON.parse(userData) : null;
};

/**
 * Get current token from local storage
 * @returns {string | null} - JWT token or null
 */
export const getToken = () => {
    return localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);
};
