# Firmeza Frontend

Modern React application for the Firmeza e-commerce platform. This SPA (Single Page Application) provides an intuitive customer interface for browsing products, managing shopping cart, and completing purchases.

## Technologies Used

- **React 19.2** - UI library
- **Vite 7.2** - Build tool and dev server
- **React Router DOM 7.9** - Client-side routing
- **Axios 1.13** - HTTP client for API communication
- **Zustand 5.0** - Lightweight state management
- **CSS3** - Custom styling with modern features
- **Nginx** - Production web server (Docker)

## Project Architecture

```
Front-end/
├── public/                   # Static assets
├── src/
│   ├── App.jsx               # Main application component
│   ├── App.css               # Global application styles
│   ├── main.jsx              # Application entry point
│   ├── index.css             # Global CSS reset and variables
│   ├── features/             # Feature-based modules
│   │   ├── auth/             # Authentication feature
│   │   │   ├── components/   # Login, Register, UserProfile
│   │   │   ├── hooks/        # useAuth custom hook
│   │   │   ├── services/     # authService API calls
│   │   │   ├── stores/       # Zustand auth store
│   │   │   └── types/        # TypeScript/JSDoc types
│   │   ├── products/         # Product catalog feature
│   │   │   ├── components/   # ProductCard, ProductList
│   │   │   ├── hooks/        # useProducts custom hook
│   │   │   ├── services/     # productsService API calls
│   │   │   └── types/        # Product types
│   │   └── sales/            # Shopping cart and checkout
│   │       ├── components/   # Cart, Checkout
│   │       ├── hooks/        # useCart, useSales hooks
│   │       ├── services/     # salesService API calls
│   │       ├── stores/       # Zustand cart store
│   │       └── types/        # Sale and cart types
│   ├── components/           # Shared components
│   ├── services/             # Shared API configuration
│   ├── lib/                  # Utility functions
│   ├── app/                  # App-level configuration
│   └── config/               # Environment configuration
├── Dockerfile                # Production container configuration
├── nginx.config              # Nginx server configuration
└── package.json              # Dependencies and scripts
```

## Features and Pages

### Authentication

**Components:**
- **Login** - User authentication form
- **Register** - New user registration with validation
- **UserProfile** - User account information and settings

**Functionality:**
- JWT-based authentication
- Persistent login state
- Automatic token refresh
- Protected routes for authenticated users
- Admin redirection to Admin Dashboard

### Product Catalog

**Components:**
- **ProductList** - Grid display of all available products
- **ProductCard** - Individual product card with image, price, and details
- **ProductDetails** - Detailed product view with description and specifications

**Functionality:**
- Browse all products
- Search and filter products
- View product details
- Add products to cart
- Real-time stock availability

### Shopping Cart & Checkout

**Components:**
- **Cart** - Shopping cart with item management
- **CartItem** - Individual cart item with quantity controls
- **Checkout** - Order summary and confirmation

**Functionality:**
- Add/remove products from cart
- Update product quantities
- Calculate totals and VAT
- Complete purchase
- Order confirmation
- State persistence across sessions

## How to Run

### Prerequisites

- Node.js 18+ and npm
- Firmeza Web API running (backend dependency)

### Running in Development Mode

1. **Navigate to the frontend directory:**
   ```bash
   cd Front-end
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Start the development server:**
   ```bash
   npm run dev
   ```

4. **Access the application:**
   - Frontend: `http://localhost:5173`
   - Hot reload enabled for development

### Building for Production

1. **Create production build:**
   ```bash
   npm run build
   ```

2. **Preview production build locally:**
   ```bash
   npm run preview
   ```

The build output will be in the `dist/` directory.

### Running with Docker

The frontend is configured to run as part of the Docker Compose stack. See the main README for instructions.

## API Integration

The frontend communicates with the Firmeza Web API:

- **Base URL:** `http://localhost:5000/api` (development)
- **Authentication:** JWT tokens stored in localStorage
- **Endpoints Used:**
  - `/auth/login` - User authentication
  - `/auth/register` - User registration
  - `/products` - Product catalog
  - `/sales` - Create and manage sales
  - `/clients` - User profile management

## State Management

The application uses Zustand for lightweight state management:

- **Auth Store** - User authentication state, login status, user info
- **Cart Store** - Shopping cart items, quantities, totals

## Routing

Client-side routing with React Router:

- `/` - Home page with product catalog
- `/login` - Login page
- `/register` - Registration page
- `/products` - Product listing
- `/cart` - Shopping cart
- `/checkout` - Checkout process
- `/profile` - User profile (protected)

## Styling

- Custom CSS with CSS variables for theming
- Responsive design for mobile, tablet, and desktop
- Modern UI with smooth animations and transitions
- No CSS framework dependency for lightweight bundle

## Development Scripts

```json
{
  "dev": "vite",              // Start development server
  "build": "vite build",      // Build for production
  "lint": "eslint .",         // Run linting
  "preview": "vite preview"   // Preview production build
}
```

## Environment Variables

Create a `.env` file (if needed) for environment-specific configuration:

```env
VITE_API_URL=http://localhost:5000/api
```

## Performance Optimizations

- Code splitting with React Router
- Lazy loading for routes
- Image optimization
- Minification and bundling with Vite
- Tree shaking for smaller bundle size

## Browser Support

- Modern browsers (Chrome, Firefox, Safari, Edge)
- ES6+ JavaScript features
- CSS Grid and Flexbox layouts

## Design Principles

- **Component-based architecture** - Reusable and maintainable components
- **Feature-first organization** - Related code grouped by feature
- **Separation of concerns** - Business logic separated from UI
- **Custom hooks** - Encapsulated logic for reusability
- **Responsive design** - Mobile-first approach
