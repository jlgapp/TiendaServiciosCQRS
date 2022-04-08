using Microsoft.EntityFrameworkCore;
using TiendaServicios.Api.Libro.Modelo;

namespace TiendaServicios.Api.Libro.Persistencia
{
    public class ContextoLibreria : DbContext
    {
        // agrgar un constructor por defecto para pruebas xunitest
        public ContextoLibreria()
        {

        }
        public ContextoLibreria(DbContextOptions<ContextoLibreria> options) : base(options)
        {}

        // con virtual puede sobrescribirse a futuro sin esto no se podra sobreescribir para los test
        public virtual DbSet<LibreriaMaterial> LibreriaMaterial { get; set; }
    }
}
