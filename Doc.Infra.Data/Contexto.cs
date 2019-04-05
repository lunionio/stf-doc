using Doc.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Doc.Infra.Data
{
    public class Contexto: DbContext
    {
        public DbSet<Documento> Documento { get; set; }
        public DbSet<DocumentoTipo> DocumentoTipo { get; set; }
        public DbSet<DocumentoLog> DocumentoLog { get; set; }
        public DbSet<DocumentoStatus> DocumentoStatus { get; set; }
        public DbSet<DocStatusObservacoes> DocStatusObservacoes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=WIN-2V8OOHSRMOD;Database=staffproDoc;Trusted_Connection=True;Integrated Security = True;");
            //optionsBuilder.UseSqlServer(@"Data Source=34.226.175.244;Initial Catalog=staffproDoc;Persist Security Info=True;User ID=sa;Password=StaffPro@123;");
        }
    }
}
