# 🏦 Banking System --- Solución Fullstack

Bienvenido al repositorio de **Banking System**.\
Este proyecto es una solución integral para la gestión de transacciones
bancarias, compuesta por:

-   Una **API RESTful** robusta\
-   Un **frontend moderno en Angular**\
-   Una **base de datos relacional MariaDB**\
-   Todo orquestado mediante **Docker** para un despliegue fácil y
    consistente

## 🚀 Accesos Rápidos (Entorno Local)

Una vez ejecutados los contenedores, los servicios estarán disponibles
en:

  ------------------------------------------------------------------------------------------------
  Componente         Descripción           URL                             Credenciales
  ------------------ --------------------- ------------------------------- -----------------------
  Frontend           Angular UI            http://localhost:4200           N/A

  Backend API        Swagger               http://localhost:5000/swagger   N/A

  Base de Datos      MariaDB               localhost:3306                  usuario_dev /
                                                                           Pichincha2025

  phpMyAdmin         Gestión BD            http://localhost:8080           Mismas credenciales
  ------------------------------------------------------------------------------------------------

## 🛠️ Stack Tecnológico

-   **Backend:** .NET 8 (ASP.NET Core Web API)\
-   **Frontend:** Angular 18\
-   **Base de Datos:** MariaDB\
-   **Infraestructura:** Docker & Docker Compose\
-   **Herramientas:** EF Core, Swagger/OpenAPI

## 📋 Prerrequisitos

-   Docker Desktop\
-   Git

## ⚙️ Instalación y Ejecución

### 1. Clonar repositorio

``` bash
git clone https://github.com/Ronal1221997/bp-ejercicio-practico.git
cd bp-ejercicio-practico
```

### 2. Ejecutar proyecto

``` bash
docker compose up --build -d
```

### 3. Verificar estado

``` bash
docker compose ps
```

## 🔧 Estructura del Proyecto

    /banking-api     → Backend .NET
    /banking-web     → Frontend Angular
    /scripts         → Scripts SQL iniciales
    docker-compose.yml → Orquestación

## 🌐 Configuración Angular

``` ts
export const environment = {
  production: true,
  apiUrl: 'http://localhost:5000/api'
};
```

## 🐛 Solución de Problemas

### 1. Dockerfile no encontrado

Asegura que el archivo se llame exactamente **Dockerfile**.

### 2. Frontend sin datos

-   Verifica la API\
-   Refresca el navegador con **CTRL + F5**

### 3. Reiniciar Base de Datos

``` bash
docker compose down -v
docker compose up -d
```
