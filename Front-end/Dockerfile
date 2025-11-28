# Stage 1 — Build SPA
FROM node:20 AS build
WORKDIR /app

COPY Front-end/package*.json ./
RUN npm install

COPY Front-end/ ./
RUN npm run build

# Stage 2 — NGINX
FROM nginx:stable-alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
