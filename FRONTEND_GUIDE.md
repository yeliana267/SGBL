# 📁 ESTRUCTURA DE VISTAS - SGBL

## 🗂️ Organización de Carpetas

### 🔐 **Auth/**
*Vistas de autenticación y autorización*

- `Login.cshtml` - Inicio de sesión
- `Register.cshtml` - Registro de usuarios  
- `ForgotPassword.cshtml` - Recuperación de contraseña
- `ResetPassword.cshtml` - Restablecer contraseña
- `AccessDenied.cshtml` - Acceso denegado

### 👑 **Admin/**
*Panel de Administración - Rol 7*

- `Dashboard.cshtml` - Dashboard principal
- **Users/** - Gestión de usuarios
  - `Index.cshtml` - Listado de usuarios
  - `Create.cshtml` - Crear usuario
  - `Edit.cshtml` - Editar usuario  
  - `Details.cshtml` - Detalles de usuario
- **Books/** - Gestión de libros
- **Reports/** - Reportes del sistema
- **Settings/** - Configuración del sistema

### 👤 **User/** 
*Panel de Usuario Normal - Rol 9*

- `Dashboard.cshtml` - Dashboard principal
- `Profile.cshtml` - Perfil de usuario
- **Books/** - Búsqueda y catálogo
- **MyLoans/** - Mis préstamos

### 📚 **Bibliotecario/**
*Panel de Bibliotecario - Rol 8*

- `Dashboard.cshtml` - Dashboard principal  
- **LoanManagement/** - Gestión de préstamos
- **BookManagement/** - Gestión de libros
- **Returns/** - Devoluciones

### 🎨 **Shared/**
*Layouts y componentes reutilizables*

- `_AdminLayout.cshtml` - Layout específico para Admin
- `_UserLayout.cshtml` - Layout específico para User
- `_BibliotecarioLayout.cshtml` - Layout específico para Bibliotecario
- `_AuthLayout.cshtml` - Layout para autenticación
- **Components/** - Componentes reutilizables

### 🏠 **Home/**
*Página pública*

- `Index.cshtml` - Página de inicio pública

## 🛣️ Rutas Principales

| Ruta | Controlador | Vista | Rol |
|------|-------------|-------|-----|
| `/` | Home | Index.cshtml | Público |
| `/Auth/Login` | AuthViews | Login.cshtml | Público |
| `/Auth/Register` | AuthViews | Register.cshtml | Público |
| `/Admin/Dashboard` | Admin | Dashboard.cshtml | Admin (7) |
| `/User/Dashboard` | UserDashboard | Dashboard.cshtml | User (9) |
| `/Bibliotecario/Dashboard` | Bibliotecario | Dashboard.cshtml | Bibliotecario (8) |

## 📋 Variables Disponibles en Views

```csharp
// Disponibles en todas las vistas mediante BaseController
ViewData["UserName"]    // Nombre del usuario
ViewData["UserRole"]    // Rol del usuario  
ViewData["UserEmail"]   // Email del usuario
ViewData["UserRoleName"] // Nombre del rol
ViewData["Title"]       // Título de la página
