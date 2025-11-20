# Stage 1: Build Angular app
FROM node:25.2-alpine AS build

WORKDIR /usr/src/app

# Copy package manifests and install deps
COPY package*.json ./
RUN npm ci

# Copy source and build
COPY . .
RUN npm run build --prod

# Stage 2: Serve with Nginx
FROM nginx:1.29-alpine

# Remove default nginx static assets
RUN rm -rf /usr/share/nginx/html/*

# Copy Angular build output
COPY --from=build /usr/src/app/dist/angular.client/browser /usr/share/nginx/html

# Copy custom nginx config (optional)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]