import { useEffect } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import useAuthStore from '@/features/auth/stores/auth.store';
import { ROUTES } from '@/lib/constants';

// Auth
import LoginPage from '@/features/auth/components/LoginPage';
import RegisterPage from '@/features/auth/components/RegisterPage';
import ProtectedRoute from '@/features/auth/components/ProtectedRoute';

// Products
import ProductsCatalog from '@/features/products/components/ProductsCatalog';

// Sales
import Cart from '@/features/sales/components/Cart';
import Checkout from '@/features/sales/components/Checkout';

// Layout
import MainLayout from '@/components/layout/MainLayout';

import './App.css';

/**
 * Main App Component
 * Configures routing and authentication
 */
function App() {
  const checkAuth = useAuthStore((state) => state.checkAuth);

  // Check authentication status on app mount
  useEffect(() => {
    checkAuth();
  }, [checkAuth]);

  return (
    <BrowserRouter>
      <Routes>
        {/* Public Routes */}
        <Route path={ROUTES.LOGIN} element={<LoginPage />} />
        <Route path={ROUTES.REGISTER} element={<RegisterPage />} />

        {/* Protected Routes */}
        <Route
          path={ROUTES.PRODUCTS}
          element={
            <ProtectedRoute>
              <MainLayout>
                <ProductsCatalog />
              </MainLayout>
            </ProtectedRoute>
          }
        />
        <Route
          path={ROUTES.CART}
          element={
            <ProtectedRoute>
              <MainLayout>
                <Cart />
              </MainLayout>
            </ProtectedRoute>
          }
        />
        <Route
          path={ROUTES.CHECKOUT}
          element={
            <ProtectedRoute>
              <MainLayout>
                <Checkout />
              </MainLayout>
            </ProtectedRoute>
          }
        />

        {/* Default redirect */}
        <Route path={ROUTES.HOME} element={<Navigate to={ROUTES.LOGIN} replace />} />
        <Route path="*" element={<Navigate to={ROUTES.LOGIN} replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App
