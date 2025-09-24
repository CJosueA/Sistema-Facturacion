using SistemaFacturacion.Models;
using SistemaFacturacion.Models.ViewModels;
using SistemaFacturacion.Services; // Importante: Using para el servicio
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaFacturacion.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsersController : Controller
    {
        private readonly BillingSystemDbContext _context;
        private readonly IPasswordService _passwordService; // 1. Declara el servicio

        // 2. Inyecta el servicio en el constructor
        public UsersController(BillingSystemDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        // GET: Users
        public async Task<IActionResult> Index(string terminoBusqueda)
        {
            var usuarioQuery = from p in _context.Users select p;
            if (!string.IsNullOrEmpty(terminoBusqueda))
            {
                var busquedaUpper = terminoBusqueda.ToUpper();
                usuarioQuery = usuarioQuery.Where(p =>
                    p.NombreUsuario.ToUpper().Contains(busquedaUpper) ||
                    p.Rol.ToUpper().Contains(busquedaUpper)
                );
            }
            return View(await usuarioQuery.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View(new UserRegistrationViewModel());
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRegistrationViewModel viewModel)
        {
            var usuarioExistente = await _context.Users.FirstOrDefaultAsync(u => u.NombreUsuario == viewModel.NombreUsuario);
            if (usuarioExistente != null)
            {
                ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya está registrado.");
            }

            if (ModelState.IsValid)
            {
                var nuevoUsuario = new User
                {
                    NombreUsuario = viewModel.NombreUsuario,
                    Rol = viewModel.Rol,
                    // 3. Usa el servicio para hashear la contraseña
                    HashContrasena = _passwordService.HashPassword(viewModel.Contrasena)
                };
                _context.Add(nuevoUsuario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Usuario creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null) return NotFound();
            var viewModel = new UserEditViewModel
            {
                Id = usuario.Id,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol
            };
            return View(viewModel);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserEditViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            var usuarioExistente = await _context.Users.FirstOrDefaultAsync(u => u.NombreUsuario == viewModel.NombreUsuario && u.Id != viewModel.Id);
            if (usuarioExistente != null)
            {
                ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya está en uso.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = await _context.Users.FindAsync(id);
                    if (usuario == null) return NotFound();

                    if (usuario.NombreUsuario.Equals("admin", StringComparison.OrdinalIgnoreCase) && viewModel.Rol != "Administrador")
                    {
                        TempData["ErrorMessage"] = "No se puede cambiar el rol del usuario administrador principal.";
                        return View(viewModel);
                    }

                    usuario.NombreUsuario = viewModel.NombreUsuario;
                    usuario.Rol = viewModel.Rol;

                    if (!string.IsNullOrEmpty(viewModel.NuevaContrasena))
                    {
                        // 3. Usa el servicio también al editar
                        usuario.HashContrasena = _passwordService.HashPassword(viewModel.NuevaContrasena);
                    }

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(viewModel.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario != null)
            {
                if (usuario.NombreUsuario.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["ErrorMessage"] = "No se puede eliminar al usuario administrador principal.";
                    return RedirectToAction(nameof(Index));
                }
                _context.Users.Remove(usuario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"El usuario {usuario.NombreUsuario} ha sido eliminado.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        // 4. El método de hashing antiguo ha sido eliminado. ¡Ya no existe!
    }
}