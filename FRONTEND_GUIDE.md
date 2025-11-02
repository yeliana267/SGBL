# Guía para Desarrollador Frontend

## Estructura del Proyecto
- `Views/Auth/` - Autenticación (Login, Registro, etc.)
- `Views/Admin/` - Panel de Administración
- `Views/User/` - Panel de Usuario
- `Views/Bibliotecario/` - Panel de Bibliotecario

## Layouts Disponibles
- `_AdminLayout.cshtml` - Para vistas de admin
- `_UserLayout.cshtml` - Para vistas de usuario
- `_BibliotecarioLayout.cshtml` - Para bibliotecario
- `_AuthLayout.cshtml` - Para autenticación

## Rutas Principales
- `/` → Redirige al dashboard según rol
- `/Auth/Login` - Iniciar sesión
- `/Admin/Dashboard` - Dashboard administrador
- `/User/Dashboard` - Dashboard usuario
- `/Bibliotecario/Dashboard` - Dashboard bibliotecario

## Variables de ViewData Disponibles
- `ViewData["UserName"]` - Nombre del usuario
- `ViewData["UserRole"]` - Rol del usuario
- `ViewData["UserEmail"]` - Email del usuario

## Estilos y Scripts
- Bootstrap 5.3.0
- Font Awesome 6.0.0
- jQuery (si necesitas)
