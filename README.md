
 Proyecto Biblioteca – LibraryHub

 Descripción del Proyecto

LibraryHub es una aplicación desarrollada en C# con Visual Studio que busca optimizar la gestión de préstamos bibliotecarios, mejorar el seguimiento de ejemplares y reducir los inconvenientes tanto para bibliotecarios como para estudiantes.

---

 Requisitos Previos

Antes de instalar y ejecutar el proyecto, asegúrate de tener lo siguiente:

-  Visual Studio 2022 o superior  
-  .NET SDK versión 6.0 o superior  
-  Cuenta activa en un proveedor de PostgreSQL en la nube (Railway, Supabase, etc.)  
-  Git instalado en tu máquina  
-  Paquetes NuGet necesarios:
  - `Npgsql` (driver ADO.NET para PostgreSQL)
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.EntityFrameworkCore.Tools`

---

Instalación

Sigue estos pasos para clonar e instalar el proyecto localmente:

```bash
# Clona el repositorio
git clone https://github.com/tu-usuario/proyecto_biblioteca.git

# Accede al directorio del proyecto
cd proyecto_biblioteca
```

Luego:

1. Abre Visual Studio.
2. Selecciona “Abrir un proyecto o solución”.
3. Navega hasta el archivo `SGBL.sln` y ábrelo.
4. Restaura los paquetes NuGet si no se hace automáticamente.

---

🗄️ Configuración de la Base de Datos en la Nube

1. Accede a tu proveedor de PostgreSQL en la nube y crea una nueva base de datos.
2. Obtén la cadena de conexión proporcionada por el servicio (ejemplo: Railway, Supabase, etc.).
3. Actualiza el archivo `appsettings.json` con la cadena de conexión:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=tu_host;Port=5432;Database=nombre_bd;Username=usuario;Password=contraseña;SSL Mode=Require;Trust Server Certificate=true"
}
```

4. Ejecuta las migraciones (si usas Entity Framework Core):

```bash
dotnet ef database update
```

---

 Ejecución

Para ejecutar el proyecto:

1. En Visual Studio, selecciona el proyecto principal (`SGBL.Web`).
2. Verifica que el perfil de ejecución esté configurado correctamente.
3. Presiona `F5` o haz clic en “Iniciar depuración”.

---

 Pruebas

Si el proyecto incluye pruebas unitarias:

```bash
dotnet test
```

O desde Visual Studio:

- Ve a “Prueba” → “Ejecutar todas las pruebas”.

---

 Estructura del Proyecto

```
/proyecto_biblioteca
│
├── /SGBL
│   ├── /SGBL.Application
│   ├── /SGBL.Domain
│   ├── /SGBL.Infrastructure
│   ├── /SGBL.Persistence
│   └── /SGBL.Web
│
├── /.dockerignore
├── /touch
├── SGBL.sln
└── README.md
