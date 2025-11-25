import { create } from 'zustand';
import { STORAGE_KEYS } from '@/lib/constants';
import * as authService from '../services/auth.service';

/**
 * Authentication Store (Zustand)
 * Manages global authentication state
 */
const useAuthStore = create((set, get) => ({
    // State
    user: authService.getCurrentUser(),
    token: authService.getToken(),
    isAuthenticated: !!authService.getToken(),
    isLoading: false,
    error: null,

    // Actions

    /**
     * Login action
     * @param {string} email 
     * @param {string} password 
     */
    login: async (email, password) => {
        set({ isLoading: true, error: null });

        try {
            const response = await authService.login(email, password);

            // Store token and user data
            localStorage.setItem(STORAGE_KEYS.AUTH_TOKEN, response.token);
            // Assuming the user object returned by authService.login already contains the role
            // If the role needs to be explicitly extracted from the token,
            // a JWT decoding library would be needed here.
            localStorage.setItem(STORAGE_KEYS.USER_DATA, JSON.stringify(response.user));

            set({
                user: response.user,
                token: response.token,
                isAuthenticated: true,
                isLoading: false,
            });

            return response;
        } catch (error) {
            const errorMessage = error.response?.data?.message || 'Login failed';
            set({ error: errorMessage, isLoading: false });
            throw error;
        }
    },

    /**
     * Register action
     * @param {import('../types/auth.types').RegisterDto} data 
     */
    register: async (data) => {
        set({ isLoading: true, error: null });

        try {
            const response = await authService.register(data);
            set({ isLoading: false });
            return response;
        } catch (error) {
            const errorMessage = error.response?.data?.message || 'Registration failed';
            set({ error: errorMessage, isLoading: false });
            throw error;
        }
    },

    /**
     * Logout action
     */
    logout: () => {
        authService.logout();
        set({
            user: null,
            token: null,
            isAuthenticated: false,
            error: null,
        });
    },

    /**
     * Check authentication status on app init
     */
    checkAuth: () => {
        const token = authService.getToken();
        const user = authService.getCurrentUser();

        set({
            token,
            user,
            isAuthenticated: !!token,
        });
    },

    /**
     * Clear error
     */
    clearError: () => set({ error: null }),
}));

export default useAuthStore;
