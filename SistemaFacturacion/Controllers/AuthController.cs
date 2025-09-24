using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaFacturacion.Models;
using SistemaFacturacion.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using SistemaFacturacion.Services; // Importante: Using para el servicio}
namespace SistemaFacturacion.Controllers
{
    public class AuthController : Controller
    {
        private readonly BillingSystemDbContext _context;
        private readonly IPasswordService _passwordService; // 1. Declara el servicio

        // 2. Inyecta el servicio en el constructor
        public AuthController(BillingSystemDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.NombreUsuario == model.NombreUsuario);

            // 3. Usa el servicio para verificar la contraseña
            if (usuario == null || !_passwordService.VerifyPassword(model.Contrasena, usuario.HashContrasena))
            {
                ModelState.AddModelError("", "Name de usuario o contraseña incorrectos.");
                TempData["ErrorMessage"] = "Credenciales inválidas. Intente de nuevo.";
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol ?? "Usuario"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                });

            TempData["SuccessMessage"] = $"¡Bienvenido, {usuario.NombreUsuario}!";
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData.Clear();
            TempData["InfoMessage"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Login");
        }

        // 4. Los métodos privados VerificarContrasena y GenerarHashContrasena han sido eliminados.

#if DEBUG
        [HttpGet]
        public async Task<IActionResult> CrearUsuarioPrueba()
        {
            try
            {
                var usuarioExistente = await _context.Users
                    .FirstOrDefaultAsync(u => u.NombreUsuario == "admin");

                if (usuarioExistente != null)
                {
                    TempData["InfoMessage"] = "El usuario de prueba ya existe";
                    return RedirectToAction("Login");
                }

                var usuarioPrueba = new User
                {
                    NombreUsuario = "admin",
                    // 3. Usa el servicio para hashear la contraseña de prueba
                    HashContrasena = _passwordService.HashPassword("123456"),
                    Rol = "Administrador"
                };

                _context.Users.Add(usuarioPrueba);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Usuario de prueba creado: admin/123456";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al crear usuario: {ex.Message}";
                if (ex.InnerException != null)
                {
                    TempData["ErrorMessage"] += $" | Detalle: {ex.InnerException.Message}";
                }
                Console.WriteLine($"Error en CrearUsuarioPrueba: {ex}");
                return RedirectToAction("Login");
            }
        }
#endif
    }
}