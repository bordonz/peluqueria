# 💈 Sistema de Gestión y Reserva de Turnos para Salón

Un sistema web asincrónico y responsivo diseñado para optimizar la administración de turnos, la gestión de clientes y el catálogo de servicios de un salón de estilismo. 
Desarrollado con una arquitectura limpia en **ASP.NET Core MVC**, persistencia relacional en **MySQL** e interacción dinámica en el cliente mediante **Vue.js** y peticiones asincrónicas (**Fetch / AJAX**).

---

## 🚀 Características Principales

* **Gestión de Turnos:** Reserva, reprogramación y cancelación de citas.
* **Control de Disponibilidad:** Algoritmo para evitar superposición de horarios entre profesionales y servicios.
* **Catálogo de Servicios:** Administración de prestaciones (corte, peinado, tintura, etc.), precios y duración estimada.
* **Gestión de Clientes y Personal:** Registro de clientes y asignación de profesionales.
* **Turnos** Sección dinamica que muestra los turnos dependiendo de si es el cliente o el empleado.

---

## 🛠️ Tecnologías Utilizadas

### **Backend**
* **Lenguaje:** C# (.NET 8.0 / .NET Core)
* **Framework:** ASP.NET Core MVC
* **Acceso a Datos:** ADO.NET / MySQL Connector (Consultas SQL parametrizadas y control fino de transacciones)
* **Autenticación:** Cookie & Bearer JWT Authentication

### **Base de Datos**
* **Motor:** MySQL Database
* **Diseño:** Relacional, normalizado con tablas intermedias N:M y *soft deletes* (eliminación lógica).

### **Frontend**
* **Vistas:** Razor Views (`.cshtml`)
* **Reactividad:** Vue.js (Progressive Framework)
* **Peticiones:** Fetch API / AJAX para comunicación asincrónica con endpoints del controlador
* **Estilos:** HTML5, CSS3, JavaScript (ES6+)

---

### **Guia**
* Clonar el repositorio
* Ir a la carpeta **Doc** e importar la base de datos
* Configurar de ser necesario la cadena de conexión en el **appsettings.json**
* Levantar el proyecto con **dotnet run**
* http://localhost:PORT

---


### **Usuarios**
* **Rol administrador:** Admin@gmail.com - Admin
* **Rol Empleado:** harryp@gmail.com - Potter
* **Rol Empleado:** mabel@gmail.com - MaEs123
* **Rol Cliente:** dipperp@gmail.com - Pines