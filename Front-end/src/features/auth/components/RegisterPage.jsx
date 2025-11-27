import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import useAuth from '../hooks/useAuth';
import { ROUTES } from '@/lib/constants';

/**
 * Register Page Component
 * Handles new user registration with comprehensive validation
 */
const RegisterPage = () => {
    const { register, isLoading, error, clearError } = useAuth();

    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        confirmPassword: '',
        phoneNumber: '',
        address: '',
        role: 'Client',
    });

    const [validationErrors, setValidationErrors] = useState({});
    const [touched, setTouched] = useState({});
    const [passwordRequirements, setPasswordRequirements] = useState({
        minLength: false,
        hasUpperCase: false,
        hasLowerCase: false,
        hasNumber: false,
        hasSpecialChar: false,
    });

    // Validate password requirements in real-time
    useEffect(() => {
        const password = formData.password;
        setPasswordRequirements({
            minLength: password.length >= 6,
            hasUpperCase: /[A-Z]/.test(password),
            hasLowerCase: /[a-z]/.test(password),
            hasNumber: /[0-9]/.test(password),
            hasSpecialChar: /[!@#$%^&*(),.?":{}|<>_\-+=[\]\\/'`~]/.test(password),
        });
    }, [formData.password]);

    // Validation rules
    const validateField = (name, value) => {
        let error = '';

        switch (name) {
            case 'firstName':
            case 'lastName':
                if (!value.trim()) {
                    error = `${name === 'firstName' ? 'First' : 'Last'} name is required`;
                } else if (value.length < 2) {
                    error = 'Must be at least 2 characters';
                } else if (value.length > 50) {
                    error = 'Must be less than 50 characters';
                } else if (!/^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$/.test(value)) {
                    error = 'Only letters are allowed';
                }
                break;

            case 'email':
                if (!value.trim()) {
                    error = 'Email is required';
                } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
                    error = 'Please enter a valid email address';
                }
                break;

            case 'password':
                if (!value) {
                    error = 'Password is required';
                } else if (value.length < 6) {
                    error = 'Password must be at least 6 characters';
                } else if (!/[A-Z]/.test(value)) {
                    error = 'Password must contain at least one uppercase letter';
                } else if (!/[a-z]/.test(value)) {
                    error = 'Password must contain at least one lowercase letter';
                } else if (!/[0-9]/.test(value)) {
                    error = 'Password must contain at least one number';
                } else if (!/[!@#$%^&*(),.?":{}|<>_\-+=[\]\\/'`~]/.test(value)) {
                    error = 'Password must contain at least one special character (!@#$%^&*...)';
                }
                break;

            case 'confirmPassword':
                if (!value) {
                    error = 'Please confirm your password';
                } else if (value !== formData.password) {
                    error = 'Passwords do not match';
                }
                break;

            case 'phoneNumber':
                if (!value.trim()) {
                    error = 'Phone number is required';
                } else if (!/^[+]?[0-9\s\-()]{7,15}$/.test(value)) {
                    error = 'Please enter a valid phone number';
                }
                break;

            case 'address':
                if (!value.trim()) {
                    error = 'Address is required';
                } else if (value.length < 5) {
                    error = 'Address must be at least 5 characters';
                } else if (value.length > 200) {
                    error = 'Address must be less than 200 characters';
                }
                break;

            default:
                break;
        }

        return error;
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));

        // Validate field if it has been touched
        if (touched[name]) {
            const error = validateField(name, value);
            setValidationErrors(prev => ({ ...prev, [name]: error }));
        }

        // Also validate confirmPassword when password changes
        if (name === 'password' && touched.confirmPassword) {
            const confirmError = validateField('confirmPassword', formData.confirmPassword);
            setValidationErrors(prev => ({ ...prev, confirmPassword: confirmError }));
        }

        if (error) clearError();
    };

    const handleBlur = (e) => {
        const { name, value } = e.target;
        setTouched(prev => ({ ...prev, [name]: true }));

        const error = validateField(name, value);
        setValidationErrors(prev => ({ ...prev, [name]: error }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        // Mark all fields as touched
        const allTouched = Object.keys(formData).reduce((acc, key) => {
            acc[key] = true;
            return acc;
        }, {});
        setTouched(allTouched);

        // Validate all fields
        const errors = {};
        Object.keys(formData).forEach(key => {
            const error = validateField(key, formData[key]);
            if (error) errors[key] = error;
        });

        setValidationErrors(errors);

        // If there are errors, don't submit
        if (Object.keys(errors).length > 0) {
            return;
        }

        await register(formData);
    };

    const getFieldClassName = (fieldName) => {
        if (!touched[fieldName]) return '';
        return validationErrors[fieldName] ? 'input-error' : 'input-success';
    };

    return (
        <div className="register-container">
            <div className="register-card">
                <h1 className="register-title">Create Account</h1>
                <p className="register-subtitle">Join us today</p>

                {error && (
                    <div className="error-message">
                        <i className="fas fa-exclamation-circle"></i>
                        <div>
                            {typeof error === 'string' ? (
                                error
                            ) : error.errors && Array.isArray(error.errors) ? (
                                <div>
                                    <strong>Please fix the following errors:</strong>
                                    <ul style={{ margin: '0.5rem 0 0 0', paddingLeft: '1.5rem' }}>
                                        {error.errors.map((err, index) => (
                                            <li key={index}>{err}</li>
                                        ))}
                                    </ul>
                                </div>
                            ) : (
                                'An error occurred during registration. Please try again.'
                            )}
                        </div>
                    </div>
                )}

                <form onSubmit={handleSubmit} className="register-form" noValidate>
                    <div className="form-row">
                        <div className="form-group">
                            <label htmlFor="firstName">
                                First Name <span className="required">*</span>
                            </label>
                            <input
                                id="firstName"
                                type="text"
                                name="firstName"
                                value={formData.firstName}
                                onChange={handleChange}
                                onBlur={handleBlur}
                                placeholder="John"
                                className={getFieldClassName('firstName')}
                                disabled={isLoading}
                            />
                            {touched.firstName && validationErrors.firstName && (
                                <span className="field-error">
                                    <i className="fas fa-exclamation-circle"></i>
                                    {validationErrors.firstName}
                                </span>
                            )}
                        </div>

                        <div className="form-group">
                            <label htmlFor="lastName">
                                Last Name <span className="required">*</span>
                            </label>
                            <input
                                id="lastName"
                                type="text"
                                name="lastName"
                                value={formData.lastName}
                                onChange={handleChange}
                                onBlur={handleBlur}
                                placeholder="Doe"
                                className={getFieldClassName('lastName')}
                                disabled={isLoading}
                            />
                            {touched.lastName && validationErrors.lastName && (
                                <span className="field-error">
                                    <i className="fas fa-exclamation-circle"></i>
                                    {validationErrors.lastName}
                                </span>
                            )}
                        </div>
                    </div>

                    <div className="form-group">
                        <label htmlFor="email">
                            Email <span className="required">*</span>
                        </label>
                        <input
                            id="email"
                            type="email"
                            name="email"
                            value={formData.email}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            placeholder="you@example.com"
                            className={getFieldClassName('email')}
                            disabled={isLoading}
                        />
                        {touched.email && validationErrors.email && (
                            <span className="field-error">
                                <i className="fas fa-exclamation-circle"></i>
                                {validationErrors.email}
                            </span>
                        )}
                    </div>

                    <div className="form-group">
                        <label htmlFor="password">
                            Password <span className="required">*</span>
                        </label>
                        <input
                            id="password"
                            type="password"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            placeholder="••••••••"
                            className={getFieldClassName('password')}
                            disabled={isLoading}
                        />
                        {touched.password && validationErrors.password && (
                            <span className="field-error">
                                <i className="fas fa-exclamation-circle"></i>
                                {validationErrors.password}
                            </span>
                        )}

                        {/* Password Requirements Checklist */}
                        {formData.password && (
                            <div className="password-requirements">
                                <p className="requirements-title">Password must contain:</p>
                                <ul className="requirements-list">
                                    <li className={passwordRequirements.minLength ? 'valid' : 'invalid'}>
                                        <i className={`fas fa-${passwordRequirements.minLength ? 'check-circle' : 'times-circle'}`}></i>
                                        At least 6 characters
                                    </li>
                                    <li className={passwordRequirements.hasUpperCase ? 'valid' : 'invalid'}>
                                        <i className={`fas fa-${passwordRequirements.hasUpperCase ? 'check-circle' : 'times-circle'}`}></i>
                                        One uppercase letter (A-Z)
                                    </li>
                                    <li className={passwordRequirements.hasLowerCase ? 'valid' : 'invalid'}>
                                        <i className={`fas fa-${passwordRequirements.hasLowerCase ? 'check-circle' : 'times-circle'}`}></i>
                                        One lowercase letter (a-z)
                                    </li>
                                    <li className={passwordRequirements.hasNumber ? 'valid' : 'invalid'}>
                                        <i className={`fas fa-${passwordRequirements.hasNumber ? 'check-circle' : 'times-circle'}`}></i>
                                        One number (0-9)
                                    </li>
                                    <li className={passwordRequirements.hasSpecialChar ? 'valid' : 'invalid'}>
                                        <i className={`fas fa-${passwordRequirements.hasSpecialChar ? 'check-circle' : 'times-circle'}`}></i>
                                        One special character (!@#$%^&*...)
                                    </li>
                                </ul>
                            </div>
                        )}
                    </div>

                    <div className="form-group">
                        <label htmlFor="confirmPassword">
                            Confirm Password <span className="required">*</span>
                        </label>
                        <input
                            id="confirmPassword"
                            type="password"
                            name="confirmPassword"
                            value={formData.confirmPassword}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            placeholder="••••••••"
                            className={getFieldClassName('confirmPassword')}
                            disabled={isLoading}
                        />
                        {touched.confirmPassword && validationErrors.confirmPassword && (
                            <span className="field-error">
                                <i className="fas fa-exclamation-circle"></i>
                                {validationErrors.confirmPassword}
                            </span>
                        )}
                        {touched.confirmPassword && !validationErrors.confirmPassword && formData.confirmPassword && (
                            <span className="field-success">
                                <i className="fas fa-check-circle"></i>
                                Passwords match
                            </span>
                        )}
                    </div>

                    <div className="form-group">
                        <label htmlFor="phoneNumber">
                            Phone Number <span className="required">*</span>
                        </label>
                        <input
                            id="phoneNumber"
                            type="tel"
                            name="phoneNumber"
                            value={formData.phoneNumber}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            placeholder="+1234567890"
                            className={getFieldClassName('phoneNumber')}
                            disabled={isLoading}
                        />
                        {touched.phoneNumber && validationErrors.phoneNumber && (
                            <span className="field-error">
                                <i className="fas fa-exclamation-circle"></i>
                                {validationErrors.phoneNumber}
                            </span>
                        )}
                    </div>

                    <div className="form-group">
                        <label htmlFor="address">
                            Address <span className="required">*</span>
                        </label>
                        <input
                            id="address"
                            type="text"
                            name="address"
                            value={formData.address}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            placeholder="123 Main St, City, State"
                            className={getFieldClassName('address')}
                            disabled={isLoading}
                        />
                        {touched.address && validationErrors.address && (
                            <span className="field-error">
                                <i className="fas fa-exclamation-circle"></i>
                                {validationErrors.address}
                            </span>
                        )}
                    </div>

                    <button
                        type="submit"
                        className="btn-primary"
                        disabled={isLoading}
                    >
                        {isLoading ? (
                            <>
                                <i className="fas fa-spinner fa-spin"></i>
                                Creating account...
                            </>
                        ) : (
                            <>
                                <i className="fas fa-user-plus"></i>
                                Create Account
                            </>
                        )}
                    </button>
                </form>

                <p className="register-footer">
                    Already have an account?{' '}
                    <Link to={ROUTES.LOGIN} className="link">
                        Sign in
                    </Link>
                </p>
            </div>
        </div>
    );
};

export default RegisterPage;
