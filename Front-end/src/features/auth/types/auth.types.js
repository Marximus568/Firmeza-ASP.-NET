/**
 * Authentication Type Definitions
 * All types related to authentication flow
 */

/**
 * @typedef {Object} RegisterDto
 * @property {string} firstName - User's first name
 * @property {string} lastName - User's last name
 * @property {string} email - User's email address
 * @property {string} password - User's password
 * @property {string} confirmPassword - Password confirmation
 * @property {string} phoneNumber - Phone number
 * @property {string} address - Physical address
 * @property {string} role - User role (default: "Client")
 */

/**
 * @typedef {Object} LoginDto
 * @property {string} email - User's email
 * @property {string} password - User's password
 */

/**
 * @typedef {Object} UserData
 * @property {number} id - User ID
 * @property {string} firstName - User's first name
 * @property {string} lastName - User's last name
 * @property {string} email - User's email
 * @property {string} role - User role
 */

/**
 * @typedef {Object} AuthResponse
 * @property {string} token - JWT token
 * @property {UserData} user - User data
 */

export { };
