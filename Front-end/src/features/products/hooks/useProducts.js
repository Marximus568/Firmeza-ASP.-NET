import { useState, useEffect } from 'react';
import * as productsService from '../services/products.service';

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
            setProducts(data);
        } catch (err) {
            const errorMessage = err.response?.data?.message || 'Failed to load products';
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
