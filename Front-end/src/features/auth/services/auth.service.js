import apiClient from '@/services/api';
import { API_ENDPOINTS, STORAGE_KEYS } from '@/lib/constants';

/**
 * Authentication Service
 * Handles all authentication-related API calls
 */

/**
 * Helper function to convert camelCase keys to PascalCase
 * @param {Object} obj - Object with camelCase keys
 * @returns {Object} - Object with PascalCase keys
 */
const toPascalCase = (obj) => {
    const pascalObj = {};
    for (const [key, value] of Object.entries(obj)) {
        // Convert first letter to uppercase
        const pascalKey = key.charAt(0).toUpperCase() + key.slice(1);
        pascalObj[pascalKey] = value;
    }
    return pascalObj;
};

/**
 * Register a new user
 * @param {import('../types/auth.types').RegisterDto} data - Registration data
 * @returns {Promise<Object>} - API response
 */
export const register = async (data) => {
    try {
        // Convert camelCase to PascalCase for C# backend
        const pascalCaseData = toPascalCase(data);
        const response = await apiClient.post(API_ENDPOINTS.AUTH.REGISTER, pascalCaseData);
        return response.data;
    } catch (error) {
        console.error("❌ Backend says:", error.response?.data);
        throw error;
    }
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

