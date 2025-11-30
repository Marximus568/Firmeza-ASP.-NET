import { Link } from 'react-router-dom';
import useProducts from '../hooks/useProducts';
import ProductCard from './ProductCard';
import { ROUTES } from '@/lib/constants';
import useCartStore from '@/features/sales/stores/cart.store';

/**
 * Products Catalog Page
 * Displays grid of all available products
 */
const ProductsCatalog = () => {
    const { products, isLoading, error, refetch } = useProducts();
    const cartItemsCount = useCartStore((state) =>
        state.items.reduce((sum, item) => sum + item.quantity, 0)
    );

    if (isLoading) {
        return (
            <div className="catalog-container">
                <div className="loading-state">
                    <div className="spinner"></div>
                    <p>Loading products...</p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="catalog-container">
                <div className="error-state">
                    <p className="error-message">{error}</p>
                    <button onClick={refetch} className="btn-secondary">
                        Try Again
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="catalog-container">
            <div className="catalog-header">
                <div>
                    <h1 className="catalog-title">Product Catalog</h1>
                    <p className="catalog-subtitle">Browse our selection</p>
                </div>

                {cartItemsCount > 0 && (
                    <Link to={ROUTES.CART} className="cart-button">
                        Cart ({cartItemsCount})
                    </Link>
                )}
            </div>

            {products.length === 0 ? (
                <div className="empty-state">
                    <p>No products available at the moment.</p>
                </div>
            ) : (
                <div className="products-grid">
                   {products.map((product) => {
   console.log("PRODUCTO RECIBIDO =>", product);
   return <ProductCard key={product.id} product={product} />;
})}
                </div>
            )}
        </div>
    );
};

export default ProductsCatalog;
