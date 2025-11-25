// API Configuration Constants
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

// Admin Dashboard URL (ASP.NET MVC app)
export const ADMIN_DASHBOARD_URL = import.meta.env.VITE_ADMIN_DASHBOARD_URL || 'http://localhost:5001';

// API Endpoints
export const API_ENDPOINTS = {
    // Auth
    AUTH: {
        LOGIN: '/v1/auth/login',
        REGISTER: '/v1/auth/register',
    },

    // Products
    PRODUCTS: {
        BASE: '/v1/products',
        BY_ID: (id) => `/v1/products/${id}`,
    },

    // Sales
    SALES: {
        REGISTER: '/v1/sales/register-sale',
        DOWNLOAD: '/v1/sales/download',
    },
};

// Storage Keys
export const STORAGE_KEYS = {
    AUTH_TOKEN: 'auth_token',
    USER_DATA: 'user_data',
};

// Route Paths
export const ROUTES = {
    HOME: '/',
    LOGIN: '/login',
    REGISTER: '/register',
    PRODUCTS: '/products',
    CART: '/cart',
    CHECKOUT: '/checkout',
};
