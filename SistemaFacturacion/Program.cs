using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SistemaFacturacion.Models;
using SistemaFacturacion.Services;

/// <summary>
/// Clase principal de la aplicación que configura los servicios y el pipeline de solicitudes HTTP.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

// --- INICIO DE CONFIGURACIÓN DE SERVICIOS ---

/// <summary>
/// Registra el DbContext de la aplicación en el contenedor de inyección de dependencias.
/// Esto permite que los controladores y otros servicios reciban una instancia del contexto de la base de datos.
/// </summary>
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BillingSystemDbContext>(options =>
    options.UseSqlite(connectionString));

/// <summary>
/// Registra y configura los servicios de autenticación de ASP.NET Core.
/// Se especifica que el esquema de autenticación por defecto será basado en Cookies.
/// </summary>
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";       // Ruta a la que se redirige si el acceso es denegado
        options.LogoutPath = "/Auth/Logout";      // Ruta para cerrar sesión
        options.AccessDeniedPath = "/Home/AccessDenied"; // Página para roles no autorizados
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Duración de la cookie
        options.SlidingExpiration = true; // Renueva la cookie si el usuario está activo
        options.Cookie.HttpOnly = true; // Previene acceso desde JavaScript (protección XSS)
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Requiere HTTPS
    });

/// <summary>
/// Registra los servicios necesarios para los controladores y las vistas (patrón MVC).
/// </summary>
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPasswordService, PasswordService>();

// --- FIN DE CONFIGURACIÓN DE SERVICIOS ---


/// <summary>
/// Construye la aplicación web a partir de los servicios configurados.
/// </summary>
var app = builder.Build();

// --- CÓDIGO DEFINITIVO PARA ROTATIVA ---
// Esta línea le dice a Rotativa: "Busca en el directorio raíz de la aplicación (ContentRootPath)
// una carpeta llamada 'Rotativa'".
Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.ContentRootPath, "Rotativa");

// --- INICIO DE CONFIGURACIÓN DEL PIPELINE HTTP ---

// Configura el pipeline de solicitudes HTTP.
if (!app.Environment.IsDevelopment())
{
    // En entornos de producción, utiliza un manejador de excepciones global.
    app.UseExceptionHandler("/Home/Error");
    // Habilita HSTS (HTTP Strict Transport Security) para mayor seguridad.
    app.UseHsts();
}

/// <summary>
/// Redirige todas las solicitudes HTTP a HTTPS.
/// </summary>
app.UseHttpsRedirection();

/// <summary>
/// Habilita el servicio de archivos estáticos (CSS, JavaScript, imágenes).
/// </summary>
app.UseStaticFiles();

/// <summary>
/// Habilita el enrutamiento para que las solicitudes puedan ser dirigidas a los controladores correctos.
/// </summary>
app.UseRouting();

/// <summary>
/// Añade el middleware de autenticación al pipeline.
/// Este middleware identifica al usuario basándose en la cookie de autenticación.
/// Debe ir ANTES de UseAuthorization.
/// </summary>
app.UseAuthentication();

/// <summary>
/// Añade el middleware de autorización al pipeline.
/// Este middleware verifica si el usuario autenticado tiene permiso para acceder a un recurso.
/// </summary>
app.UseAuthorization();

/// <summary>
/// Configura la ruta por defecto para el patrón MVC.
/// </summary>
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

/// <summary>
/// Ejecuta la aplicación y comienza a escuchar las solicitudes HTTP.
/// </summary>
app.Run();
