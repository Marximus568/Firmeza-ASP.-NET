import axios from 'axios';
import { API_BASE_URL, STORAGE_KEYS, ROUTES } from '@/lib/constants';

/**
 * Centralized Axios instance with JWT interceptors
 * Handles automatic token injection and expiration redirects
 */
const apiClient = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

/**
 * Request Interceptor
 * Attaches JWT token to all requests if available
 */
apiClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);

        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }

        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

/**
 * Response Interceptor
 * Handles 401 errors (unauthorized/expired token)
 * Redirects to login and clears local storage
 */
apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            // Clear authentication data
            localStorage.removeItem(STORAGE_KEYS.AUTH_TOKEN);
            localStorage.removeItem(STORAGE_KEYS.USER_DATA);

            // Redirect to login if not already there
            if (window.location.pathname !== ROUTES.LOGIN) {
                window.location.href = ROUTES.LOGIN;
            }
        }

        return Promise.reject(error);
    }
);

export default apiClient;
