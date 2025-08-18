# Build-Phase
FROM node:18 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# Serve mit Nginx
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
