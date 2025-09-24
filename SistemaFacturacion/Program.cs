using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SistemaFacturacion.Models;
using SistemaFacturacion.Services;

/// <summary>
/// Clase principal de la aplicaci�n que configura los servicios y el pipeline de solicitudes HTTP.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

// --- INICIO DE CONFIGURACI�N DE SERVICIOS ---

/// <summary>
/// Registra el DbContext de la aplicaci�n en el contenedor de inyecci�n de dependencias.
/// Esto permite que los controladores y otros servicios reciban una instancia del contexto de la base de datos.
/// </summary>
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BillingSystemDbContext>(options =>
    options.UseSqlite(connectionString));

/// <summary>
/// Registra y configura los servicios de autenticaci�n de ASP.NET Core.
/// Se especifica que el esquema de autenticaci�n por defecto ser� basado en Cookies.
/// </summary>
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";       // Ruta a la que se redirige si el acceso es denegado
        options.LogoutPath = "/Auth/Logout";      // Ruta para cerrar sesi�n
        options.AccessDeniedPath = "/Home/AccessDenied"; // P�gina para roles no autorizados
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Duraci�n de la cookie
        options.SlidingExpiration = true; // Renueva la cookie si el usuario est� activo
        options.Cookie.HttpOnly = true; // Previene acceso desde JavaScript (protecci�n XSS)
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Requiere HTTPS
    });

/// <summary>
/// Registra los servicios necesarios para los controladores y las vistas (patr�n MVC).
/// </summary>
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPasswordService, PasswordService>();

// --- FIN DE CONFIGURACI�N DE SERVICIOS ---


/// <summary>
/// Construye la aplicaci�n web a partir de los servicios configurados.
/// </summary>
var app = builder.Build();

// --- C�DIGO DEFINITIVO PARA ROTATIVA ---
// Esta l�nea le dice a Rotativa: "Busca en el directorio ra�z de la aplicaci�n (ContentRootPath)
// una carpeta llamada 'Rotativa'".
Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.ContentRootPath, "Rotativa");

// --- INICIO DE CONFIGURACI�N DEL PIPELINE HTTP ---

// Configura el pipeline de solicitudes HTTP.
if (!app.Environment.IsDevelopment())
{
    // En entornos de producci�n, utiliza un manejador de excepciones global.
    app.UseExceptionHandler("/Home/Error");
    // Habilita HSTS (HTTP Strict Transport Security) para mayor seguridad.
    app.UseHsts();
}

/// <summary>
/// Redirige todas las solicitudes HTTP a HTTPS.
/// </summary>
app.UseHttpsRedirection();

/// <summary>
/// Habilita el servicio de archivos est�ticos (CSS, JavaScript, im�genes).
/// </summary>
app.UseStaticFiles();

/// <summary>
/// Habilita el enrutamiento para que las solicitudes puedan ser dirigidas a los controladores correctos.
/// </summary>
app.UseRouting();

/// <summary>
/// A�ade el middleware de autenticaci�n al pipeline.
/// Este middleware identifica al usuario bas�ndose en la cookie de autenticaci�n.
/// Debe ir ANTES de UseAuthorization.
/// </summary>
app.UseAuthentication();

/// <summary>
/// A�ade el middleware de autorizaci�n al pipeline.
/// Este middleware verifica si el usuario autenticado tiene permiso para acceder a un recurso.
/// </summary>
app.UseAuthorization();

/// <summary>
/// Configura la ruta por defecto para el patr�n MVC.
/// </summary>
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

/// <summary>
/// Ejecuta la aplicaci�n y comienza a escuchar las solicitudes HTTP.
/// </summary>
app.Run();
