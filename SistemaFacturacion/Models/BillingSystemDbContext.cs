using Microsoft.EntityFrameworkCore;

namespace SistemaFacturacion.Models;

/// <summary>
/// Contexto de base de datos para el sistema de inventario
/// Maneja la conexión y configuración de todas las entidades del sistema
/// </summary>
public partial class BillingSystemDbContext : DbContext
{
    public BillingSystemDbContext(DbContextOptions<BillingSystemDbContext> options)
        : base(options)
    {
    }

    // --- DBSETS EXISTENTES ---
    public DbSet<CompanyData> CompanyData { get; set; }
    public virtual DbSet<Movement> Movements { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<User> Users { get; set; }

    // --- NUEVOS DBSETS PARA FACTURACIÓN ---
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Invoice> Invoices { get; set; }
    public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }


    /// <summary>
    /// Configuración del modelo de datos y restricciones
    /// Define relaciones, índices únicos y validaciones a nivel de base de datos
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Se mantiene toda tu configuración original para Movimiento, Product y Usuario...
        // ... (código original omitido por brevedad)

        // ========================================
        // CONFIGURACIÓN DE CLIENTE
        // ========================================
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.NumeroIdentificacion).IsUnique();
        });

        // ========================================
        // CONFIGURACIÓN DE FACTURA
        // ========================================
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.NumeroFactura).IsUnique();

            // Relación con Cliente: Un cliente puede tener muchas facturas.
            entity.HasOne(f => f.Cliente)
                  .WithMany(c => c.Invoices)
                  .HasForeignKey(f => f.ClienteId)
                  .OnDelete(DeleteBehavior.Restrict); // Evita borrar un cliente si tiene facturas
        });

        // ========================================
        // CONFIGURACIÓN DE FACTURADETALLE
        // ========================================
        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            // Relación con Invoice: Una factura tiene muchos detalles.
            entity.HasOne(d => d.Invoice)
                  .WithMany(f => f.InvoiceDetails)
                  .HasForeignKey(d => d.InvoiceId)
                  .OnDelete(DeleteBehavior.Cascade); // Si se borra una factura, se borran sus detalles.

            // Relación con Product: Un producto puede estar en muchos detalles de factura.
            entity.HasOne(d => d.Product)
                  .WithMany() // No necesitamos una colección de detalles en Product
                  .HasForeignKey(d => d.ProductId)
                  .OnDelete(DeleteBehavior.Restrict); // Evita borrar un producto si está en una factura.
        });

        // --- CÓDIGO ORIGINAL DE OnModelCreating ---
        // (Se asume que tu código original de OnModelCreating para las otras tablas va aquí)
        // ========================================
        // CONFIGURACIÓN DE MOVIMIENTO
        // ========================================
        modelBuilder.Entity<Movement>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.Property(e => e.Observation).HasMaxLength(200).IsUnicode(false);
            entity.Property(e => e.ProductId).HasColumnName("ProductoID").IsRequired();
            entity.Property(e => e.Type).HasMaxLength(20).IsUnicode(false).IsRequired();
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.HasOne(d => d.Product).WithMany(p => p.Movements).HasForeignKey(d => d.ProductId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Movimientos_Productos");
        });

        // ========================================
        // CONFIGURACIÓN DE PRODUCTO
        // ========================================
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Code, "UNQ_Codigo").IsUnique().HasDatabaseName("UNQ_Productos_Codigo");
            entity.Property(e => e.Code).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(75).IsUnicode(false).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(50).IsUnicode(false).IsRequired();
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)").IsRequired();
            entity.Property(e => e.Stock).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        });

        // ========================================
        // CONFIGURACIÓN DE USUARIO
        // ========================================
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasIndex(e => e.NombreUsuario, "UNQ_NombreUsuario").IsUnique().HasDatabaseName("UNQ_Usuarios_NombreUsuario");
            entity.Property(e => e.NombreUsuario).HasMaxLength(75).IsUnicode(false).IsRequired();
            entity.Property(e => e.HashContrasena).HasMaxLength(255).IsUnicode(false).IsRequired();
            entity.Property(e => e.Rol).HasMaxLength(20).IsUnicode(false);
        });

        // ========================================
        // Data Seeding para los datos de facturación de la empresa
        // ========================================
        modelBuilder.Entity<CompanyData>().HasData(
            new CompanyData
            {
                Id = 1, // Debes especificar el ID manualmente
                NombreEmpresa = "Nombre de tu Empresa (Default)",
                Direccion = "Dirección por defecto",
                Telefono = "0000-0000",
                CorreoElectronico = "correo@ejemplo.com",
                CedulaJuridica = "000-0000-000000"
                // Agrega aquí los demás campos necesarios con valores por defecto
            }
        );


        // Llamada al método parcial para extensiones futuras
        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
