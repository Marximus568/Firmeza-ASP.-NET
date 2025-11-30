import { useState, useEffect } from 'react';
import * as productsService from '../services/products.service';

/**
 * Normalizes backend product format (PascalCase)
 * to frontend camelCase structure.
 */
const normalizeProduct = (p) => ({
    id: p.Id,
    name: p.Name,
    description: p.Description,
    unitPrice: p.UnitPrice,
    stock: p.Stock,
    categoryId: p.CategoryId,
    categoryName: p.CategoryName,
    createdAt: p.CreatedAt,
    updatedAt: p.UpdatedAt,
});

/**
 * Custom hook for fetching and managing products
 * @returns {Object} - Products data, loading state, and error
 */
const useProducts = () => {
    const [products, setProducts] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetchProducts();
    }, []);

    const fetchProducts = async () => {
        setIsLoading(true);
        setError(null);

        try {
            const data = await productsService.getProducts();

            // Normalize all products before storing them
            const normalizedProducts = Array.isArray(data)
                ? data.map(normalizeProduct)
                : [];

            setProducts(normalizedProducts);
        } catch (err) {
            const errorMessage =
                err.response?.data?.message || 'Failed to load products';
            setError(errorMessage);
            console.error('Error fetching products:', err);
        } finally {
            setIsLoading(false);
        }
    };

    const refetch = () => {
        fetchProducts();
    };

    return {
        products,
        isLoading,
        error,
        refetch,
    };
};

export default useProducts;
