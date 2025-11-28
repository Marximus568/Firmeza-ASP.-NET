# Stage 1 — Build SPA
FROM node:20 AS build
WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm install

# Copy source files and build
COPY . .

# Accept build arguments
ARG VITE_API_BASE_URL
ARG VITE_ADMIN_DASHBOARD_URL

# Set environment variables for build
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
ENV VITE_ADMIN_DASHBOARD_URL=$VITE_ADMIN_DASHBOARD_URL

RUN npm run build

# Stage 2 — NGINX
FROM nginx:stable-alpine

# Copy custom nginx configuration
COPY nginx.config /etc/nginx/conf.d/default.conf

# Copy built app
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
