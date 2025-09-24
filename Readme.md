# **Sistema de Facturación y Gestión de Inventario (MVP)**

Este proyecto es un **MVP (Producto Mínimo Viable)** de un sistema web para la facturación y gestión de inventario, desarrollado como parte de mi formación y portafolio profesional.

<img width="1898" height="929" alt="Home" src="https://github.com/user-attachments/assets/9c5d6ccd-8390-448a-bc85-fbd3696f449c" />
<img width="1896" height="928" alt="image" src="https://github.com/user-attachments/assets/7003ae89-7efe-4a06-9d87-2e4780c843d0" />
<img width="1911" height="923" alt="image" src="https://github.com/user-attachments/assets/2452a557-b916-47fc-a888-7ad368c47002" />
<img width="1912" height="923" alt="image" src="https://github.com/user-attachments/assets/b76a60c2-d161-4267-b64b-224d43908553" />
<img width="1915" height="925" alt="image" src="https://github.com/user-attachments/assets/fb40a2ce-f66e-42ec-9604-4e3ee3e32221" />
<img width="1891" height="924" alt="image" src="https://github.com/user-attachments/assets/43f540e6-040b-4e83-9ce5-7a3967b80a7f" />



## **🤖 Desarrollo Asistido por IA**

Este proyecto fue desarrollado integrando **inteligencia artificial generativa** en cada fase del ciclo de vida del software. En la práctica, mi rol se centró en la arquitectura, dirección y supervisión de la calidad del código generado.

Fui responsable de:

* **Definir la arquitectura** y las tecnologías a utilizar.  
* **Dirigir el proyecto** guiando a la IA con prompts precisos para generar el código base.  
* **Depurar y refinar** los resultados para asegurar su funcionalidad.  
* **Integrar los diferentes módulos** y componentes en un sistema cohesivo.  
* **Optimizar el código** para mejorar su rendimiento y legibilidad.

Este enfoque me permitió construir un MVP funcional en un tiempo récord y, a la vez, centrarme en los aspectos de alto nivel del desarrollo de software.

## **📜 Descripción General**

Es una solución integral diseñada para administrar el inventario, la facturación y los clientes de un negocio. Permite optimizar operaciones y mantener un control total desde un solo lugar, con una interfaz limpia e intuitiva.

## **✨ Características Principales**

El sistema está organizado en módulos para una gestión eficiente:

### **👤 Autenticación y Roles**

* **Inicio de Sesión Seguro:** Sistema de login para acceder a las funcionalidades.  
* **Gestión de Contraseñas Seguras:** Las contraseñas de los usuarios se almacenan de forma segura utilizando el algoritmo de hashing **BCrypt**, garantizando que nunca se guarden en texto plano.  
* **Roles de Usuario:**  
  * **Administrador:** Acceso completo a todos los módulos del sistema.  
  * **Vendedor:** Acceso limitado a los módulos de Productos, Movimientos de Inventario y Clientes.  
  * **Bodeguero:** Acceso limitado a los módulos de Productos, Movimientos de Inventario y Clientes.  
* **Cierre de Sesión:** Permite salir del sistema de forma segura.

### **📦 Módulo de Productos**

* Visualización y búsqueda de productos por código, nombre o categoría.  
* Creación de nuevos productos (el código se genera automáticamente).  
* **Activación/Desactivación:** Los productos deben ser activados para ser utilizados en otros módulos. No se eliminan para mantener la integridad de los datos históricos.

### **🔄 Módulo de Movimientos de Inventario**

* Registro de entradas y salidas de stock para cada producto.  
* Búsqueda de productos activos para una gestión rápida.  
* Historial detallado de todos los movimientos por producto.

### **🧾 Módulo de Facturación**

* Creación de nuevas facturas seleccionando clientes y productos.  
* Consulta del historial de facturas y **generación de PDFs** con la utilidad **Rotativa**.  
* Configuración de los datos fiscales de la empresa y registro rápido de clientes.

### **👥 Módulo de Gestión de Clientes**

* Administración centralizada y búsqueda de clientes por ID o nombre.  
* Registro de nuevos clientes con su información de contacto.

### **⚙️ Módulo de Gestión de Usuarios (Solo Administradores)**

* Visualización y búsqueda de usuarios por nombre o rol.  
* Modificación de datos de usuario, incluyendo nombre, rol y contraseña.

## **🛠️ Tecnologías Utilizadas**

* **Backend:** C\# con ASP.NET Core  
* **Base de Datos:** Entity Framework Core (Enfoque Code-First) con SQLite  
* **Arquitectura:** MVC (Modelo-Vista-Controlador)  
* **Frontend:** Vistas con Razor Pages  
* **Seguridad:** BCrypt.Net para hashing de contraseñas  
* **Utilitarios:** Rotativa para la generación de PDFs

## **🚀 Puesta en Marcha**

Sigue estos pasos para ejecutar el proyecto en tu entorno local.

### **Requisitos Previos**

* .NET SDK 8.0 o superior.  
* Visual Studio 2022 o un editor de código de tu preferencia.

### **Instalación y Ejecución**

1. **Clona el repositorio:**  
   git clone \[https://github.com/CJosueA/Sistema-Facturacion\](https://github.com/CJosueA/Sistema-Facturacion)  
   cd Sistema-Facturacion

2. Ejecuta el proyecto:  
   Puedes iniciarlo directamente desde Visual Studio (presionando F5) o usando la terminal:  
   dotnet run

### **Cómo Probar la Aplicación**

El repositorio incluye una base de datos con datos de prueba para agilizar la evaluación.

* **Opción 1 (Recomendada \- Usar datos de prueba):**  
  * **Usuario:** admin  
  * **Contraseña:** 123456
  * **Usuario:** Pedro Gómez  
  * **Contraseña:** 123456  
* **Opción 2 (Crear base de datos desde cero):**  
  1. Elimina el archivo ProjectDatabase.db de la carpeta del proyecto.  
  2. Abre una terminal en la raíz del proyecto y ejecuta las migraciones:  
     dotnet ef database update

  3. **Importante:** Con la aplicación en ejecución, navega a la siguiente URL en tu navegador para crear un usuario administrador de prueba. (Reemplaza PORTNUMBER por el puerto que use tu aplicación, ej: 7123).  
     https://localhost:PORTNUMBER/Auth/CrearUsuarioPrueba

## **⚠️ Estado Actual y Próximos Pasos**

Este proyecto es un **trabajo en progreso**. Debido a limitaciones de tiempo, decidí publicarlo en su estado actual para poder avanzar con otras tareas.

Ten en cuenta lo siguiente:

* **Refactorización Pendiente:** El código está en proceso de ser refactorizado para mejorar su calidad, implementar mejores prácticas de seguridad y optimizar el rendimiento general.  
* **Traducción en Curso:** El proyecto está siendo traducido al inglés. Por esta razón, encontrarás una mezcla de español e inglés en el código y en los comentarios del código.  
* **Soluciones Temporales:** Algunas funcionalidades, como la creación del usuario de prueba o la inclusión de binarios en el repo, son soluciones temporales que serán abordadas en futuras actualizaciones.

¡Gracias por tu comprensión\!

## **👨‍💻 Autor**

* **Celer Josué Aguero Gamboa** \- [GitHub](https://github.com/CJosueA) \- [LinkedIn](https://www.linkedin.com/in/celerjosuea/)
