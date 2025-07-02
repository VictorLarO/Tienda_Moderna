# Tienda Moderna

**Tienda Moderna** es un sistema web de gestión para tiendas, desarrollado con ASP.NET Core Razor Pages (.NET 8). Permite administrar ventas, inventario, clientes, marcas, proveedores y reportes de manera sencilla y eficiente.

---

## Tabla de Contenidos

- [Características](#características)
- [Tecnologías Utilizadas](#tecnologías-utilizadas)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Instalación y Ejecución](#instalación-y-ejecución)
- [Guía de Uso](#guía-de-uso)
- [Créditos](#créditos)

---

## Características

- Gestión de ventas con generación de tickets.
- Administración de inventario con notificaciones de stock bajo y caducidad.
- Registro, edición y eliminación de clientes, marcas y proveedores.
- Búsqueda avanzada de productos.
- Reportes diarios y semanales de ventas y ganancias.
- Interfaz responsiva y fácil de usar.

---

## Tecnologías Utilizadas

- **.NET 8** (ASP.NET Core Razor Pages)
- **C# 12**
- **Entity Framework Core 8**
- **Bootstrap 5.3**
- **jQuery 3.6+**
- **HTML5, CSS3, JavaScript**

---

## Estructura del Proyecto

- `/Models`  
  Modelos de datos (entidades como Producto, Cliente, Marca, Proveedor, etc.).

- `/Controllers`  
  Controladores para la lógica de negocio y manejo de peticiones (CRUD, reportes, etc.).

- `/Views`  
  Vistas Razor (`.cshtml`) organizadas por área:
  - `/Home` — Página principal y navegación.
  - `/Ventas` — Gestión de ventas y tickets.
  - `/Clientes` — Administración de clientes.
  - `/Inventario` — Consulta, búsqueda y edición de productos.
  - `/Marcas` — Gestión de marcas.
  - `/Proveedores` — Gestión de proveedores.
  - `/Reportes` — Reportes diarios y semanales.

- `/wwwroot`  
  Archivos estáticos: CSS, JS, imágenes, librerías externas.

- `appsettings.json`  
  Configuración de la aplicación (conexión a base de datos, etc.).

---

## Instalación y Ejecución

1. **Clona el repositorio:**  git clone https://github.com/tu-usuario/tienda-moderna.git cd tienda-moderna
2. **Configura la base de datos:**
- Edita `appsettings.json` con tu cadena de conexión.

3. **Restaura los paquetes NuGet:**
4. **Aplica las migraciones (si usas EF Core):**
5. **Ejecuta la aplicación:**O desde Visual Studio, presiona `F5`.

6. **Accede a la aplicación:**  
Abre tu navegador en `https://localhost:5001` o la URL que indique la consola.

---

## Guía de Uso

- **Home:**  
Acceso rápido a todas las secciones principales mediante botones grandes.

- **Ventas:** 
Consulta el inventario, agrega productos al carrito, registra ventas y genera tickets imprimibles.

- **Clientes:**  
Visualiza, agrega, edita y elimina clientes mediante una tabla interactiva y formularios modales.

- **Inventario:** 
Consulta todos los productos, recibe notificaciones de stock bajo/caducidad, busca productos y gestiona el catálogo.

- **Marcas y Proveedores:** 
Administra marcas y proveedores con formularios modales para agregar, editar y eliminar.

- **Reportes:** 
Genera y consulta reportes diarios y semanales de ventas y ganancias.

---

## Créditos

Desarrollado por VILO_IV.  
Proyecto realizado con ASP.NET Core Razor Pages (.NET 8).

---

¿Tienes dudas o sugerencias? ¡Abre un issue o pull request!
