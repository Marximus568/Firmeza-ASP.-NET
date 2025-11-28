import { useNavigate } from 'react-router-dom';
import useAuthStore from '../stores/auth.store';
import { ROUTES, ADMIN_DASHBOARD_URL } from '@/lib/constants';

/**
 * Custom hook for authentication
 * Provides clean interface to auth store and navigation
 */
const useAuth = () => {
    const navigate = useNavigate();
    const {
        user,
        isAuthenticated,
        isLoading,
        error,
        login: loginAction,
        register: registerAction,
        logout: logoutAction,
        clearError,
    } = useAuthStore();

    /**
   * Login handler with role-based navigation
   * @param {string} email 
   * @param {string} password 
   */
    const login = async (email, password) => {
        try {
            const response = await loginAction(email, password);

            // Check user role and redirect accordingly
            if (response.user && response.user.role) {
                const userRole = response.user.role.toLowerCase();

                // If user is Admin, redirect to the main admin panel (ASP.NET MVC app)
                if (userRole === 'admin') {
                    // Store JWT token in a cookie for the admin dashboard to read
                    document.cookie = `auth_token=${response.token}; path=/; domain=localhost; SameSite=Lax`;
                    // Redirect with token as query param for SSO (optional)
                    window.location.href = ADMIN_DASHBOARD_URL;
                    return;
                }

                // If user is Client/Customer, redirect to React products catalog
                if (userRole === 'client' || userRole === 'customer') {
                    navigate(ROUTES.PRODUCTS);
                    return;
                }
            }

            // Default fallback to products
            navigate(ROUTES.PRODUCTS);
        } catch (error) {
            // Error is already set in store
            console.error('Login error:', error);
        }
    };

    /**
     * Register handler with navigation
     * @param {import('../types/auth.types').RegisterDto} data 
     */
    const register = async (data) => {
        try {
            await registerAction(data);
            navigate(ROUTES.LOGIN);
        } catch (error) {
            console.error('Registration error:', error);
        }
    };

    /**
     * Logout handler with navigation
     */
    const logout = () => {
        logoutAction();
        navigate(ROUTES.LOGIN);
    };

    return {
        user,
        isAuthenticated,
        isLoading,
        error,
        login,
        register,
        logout,
        clearError,
    };
};

export default useAuth;
