# proyecto_biblioteca

proyecto_biblioteca

📘 Descripción del proyecto.

LibraryHub, un proyecto que busca optimizar la gestión de préstamos,
mejorar el seguimiento de ejemplares y reducir los inconvenientes tanto para los
bibliotecarios como para los estudiantes.

🛠️ Requisitos Previos

Antes de instalar y ejecutar el proyecto, asegúrate de tener lo siguiente:

- 💻 [Visual Studio](https://visualstudio.microsoft.com/) 2022 o superior
- 🧱 .NET SDK versión 6.0 o superior
- 🐘 [PostgreSQL](https://www.postgresql.org/download/) instalado y configurado
- 🔗 Git instalado en tu máquina
- 📦 Paquete NuGet: `Npgsql` (driver ADO.NET para PostgreSQL)

📦 Instalación

Sigue estos pasos para clonar e instalar el proyecto localmente:

```bash
# Clona el repositorio
git clone https://github.com/tu-usuario/nombre-del-repositorio.git

# Accede al directorio del proyecto
cd nombre-del-repositorio
```

Luego:

1. Abre Visual Studio.
2. Selecciona “Abrir un proyecto o solución”.
3. Navega hasta el archivo `.sln` del proyecto y ábrelo.
4. Restaura los paquetes NuGet si no se hace automáticamente (`Npgsql`, `EntityFrameworkCore`, etc.).

🗄️ Configuración de la Base de Datos

1. Asegúrate de tener PostgreSQL corriendo localmente o en un servidor accesible.
2. Crea una base de datos con el nombre especificado en el archivo de configuración (`appsettings.json`).
3. Actualiza la cadena de conexión en `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=nombre_bd;Username=tu_usuario;Password=tu_contraseña"
}
```

4. Ejecuta las migraciones si estás usando Entity Framework Core:

```bash
dotnet ef database update
```

▶️ Ejecución

Para ejecutar el proyecto:

1. En Visual Studio, selecciona el proyecto principal.
2. Verifica que el perfil de ejecución esté configurado correctamente.
3. Presiona `F5` o haz clic en “Iniciar depuración”.

 🧪 Pruebas

Si el proyecto incluye pruebas unitarias:

```bash
dotnet test
```

O desde Visual Studio:

- Ve a “Prueba” → “Ejecutar todas las pruebas”.

📂 Estructura del Proyecto

/proyecto_biblioteca
│
├── /SGBL 
          /SGBL.Application
	  /SGBL.Domain
	  /SGBL.Infraestructure
          /SGBL.Persistence
  	  /SGBL.Web
	  /.dockerignore
	  /SGBL.sln
	  /touch	    
├── README.md         

