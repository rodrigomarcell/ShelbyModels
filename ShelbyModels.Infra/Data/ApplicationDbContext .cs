using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShelbyModels.Domain.Entities; // Camada de domínio
using ShelbyModels.Infra.Entities;   // Camada de infraestrutura

namespace ShelbyModels.Infra.Data
{
    public class ApplicationDbContext : IdentityDbContext<InfraUser, IdentityRole<int>, int>
    {


        public DbSet<InfraUser> InfraUsers { get; set; } // Usando Address da camada de domínio
        public DbSet<Address> Addresses { get; set; } // Usando Address da camada de domínio



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Evita que entidades relacionadas através de chaves estrangeiras sejam apagadas em cascata, quando a entidade pai for apagada.
            var cascadeFKs = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()).Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
            #endregion

            // Configuração do relacionamento 1:1 entre InfraUser e Address
            modelBuilder.Entity<InfraUser>()
                .HasOne(u => u.Address)
                .WithOne()
                .HasForeignKey<InfraUser>(u => u.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuração do relacionamento 1:N entre Address e InfraUser
            modelBuilder.Entity<Address>()
                .HasOne<InfraUser>() // Não cria uma tabela User
                .WithMany() // Relacionamento sem a necessidade de uma tabela User
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configuração das chaves primárias
            modelBuilder.Entity<InfraUser>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Address>()
                .HasKey(a => a.Id);
        }

    }
}
