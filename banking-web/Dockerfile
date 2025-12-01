# Etapa 1: Build de Angular
FROM node:20-alpine as build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
# Asegúrate de que 'production' exista en tu angular.json, si no usa 'build' a secas
RUN npm run build --configuration=production

# Etapa 2: Servidor Nginx
FROM nginx:alpine
# Copiamos el build de Angular a la carpeta pública de Nginx
# IMPORTANTE: Revisa en tu dist/ si la carpeta se llama 'banking-web' o 'browser'
COPY --from=build /app/dist/banking-web/browser /usr/share/nginx/html

# Opcional: Configuración custom de Nginx si usas rutas internas de Angular
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80