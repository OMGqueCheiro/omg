using Microsoft.EntityFrameworkCore;
using OMG.Core.Entities;
using OMG.Domain.Events;

namespace OMG.Repository;

public class OMGDbContext : DbContext
{
    public OMGDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Aroma> Aromas { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Cor> Cores { get; set; }
    public DbSet<Formato> Formatos { get; set; }
    public DbSet<Embalagem> Embalagens { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<PedidoItem> PedidoItens { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<EventChangeStatus> EventChangeStatus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OMGDbContext).Assembly);
    }
    public async Task SeedData()
    {
        if (!await Clientes.AnyAsync())
        {
            var cliente = new Cliente
            {
                Nome = "Victor (Marido)",
                Telefone = "53 98416-3953",
                Endereco = "JK de Oliveira 2200 19B 201",
                IsDeleted = false,
                DeletedAt = null
            };

            await Clientes.AddAsync(cliente);
            await SaveChangesAsync();
        }
    }
}
