import { Link } from 'react-router-dom';
import useAuth from '@/features/auth/hooks/useAuth';
import { ROUTES } from '@/lib/constants';

/**
 * Main Layout Component
 * Provides navigation bar and main content area for authenticated pages
 * 
 * @param {Object} props
 * @param {React.ReactNode} props.children - Child components/pages
 */
const MainLayout = ({ children }) => {
    const { user, logout } = useAuth();

    return (
        <div className="app-layout">
            <nav className="navbar">
                <div className="navbar-container">
                    <Link to={ROUTES.PRODUCTS} className="navbar-brand">
                        Shop
                    </Link>

                    <div className="navbar-menu">
                        <Link to={ROUTES.PRODUCTS} className="nav-link">
                            Products
                        </Link>
                        <Link to={ROUTES.CART} className="nav-link">
                            Cart
                        </Link>

                        <div className="navbar-user">
                            <span className="user-name">
                                {user?.firstName || 'User'}
                            </span>
                            <button onClick={logout} className="btn-logout">
                                Logout
                            </button>
                        </div>
                    </div>
                </div>
            </nav>

            <main className="main-content">
                {children}
            </main>

            <footer className="footer">
                <p>© 2025 Shop. All rights reserved.</p>
            </footer>
        </div>
    );
};

export default MainLayout;
