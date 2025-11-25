import clsx from 'clsx';
import useCartStore from '@/features/sales/stores/cart.store';

/**
 * Product Card Component
 * Displays individual product information
 * 
 * @param {Object} props
 * @param {import('../types/products.types').ProductDto} props.product - Product data
 */
const ProductCard = ({ product }) => {
    const addItem = useCartStore((state) => state.addItem);

    const handleAddToCart = () => {
        addItem(product);
    };

    const isOutOfStock = (product.stock ?? 0) === 0;

    return (
        <div className="product-card">
            <div className="product-image-placeholder">
                <span className="product-icon"></span>
            </div>

            <div className="product-info">
                <h3 className="product-name">{product.name}</h3>

                {product.description && (
                    <p className="product-description">{product.description}</p>
                )}

                {product.categoryName && (
                    <span className="product-category">{product.categoryName}</span>
                )}

                <div className="product-footer">
                    <div className="product-pricing">
                        <span className="product-price">${(product.unitPrice ?? 0).toFixed(2)}</span>
                        <span className={clsx(
                            'product-stock',
                            isOutOfStock && 'out-of-stock'
                        )}>
                            {isOutOfStock ? 'Out of Stock' : `${product.stock} in stock`}
                        </span>
                    </div>

                    <button
                        onClick={handleAddToCart}
                        disabled={isOutOfStock}
                        className={clsx('btn-add-cart', isOutOfStock && 'disabled')}
                    >
                        {isOutOfStock ? 'Unavailable' : 'Add to Cart'}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default ProductCard;
