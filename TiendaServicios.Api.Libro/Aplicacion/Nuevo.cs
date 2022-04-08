using FluentValidation;
using MediatR;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;

namespace TiendaServicios.Api.Libro.Aplicacion
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            public string Titulo { get; set; } = String.Empty;
            public Guid? AutorLibro { get; set; }
            public DateTime? FechaPublicacion { get; set; } = null;

        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(p => p.Titulo)
                    .NotEmpty().WithMessage("{Nombre} no puede estar en blanco")
                    .NotNull().WithMessage("{Nombre} no puede estar en blanco")
                    .MaximumLength(50).WithMessage("{Nombre} no puede exceder 50 caracteres");

                RuleFor(p => p.AutorLibro)
                    .NotEmpty().WithMessage("{Apellido} no puede estar en blanco")
                    .NotNull().WithMessage("{Apellido} no puede estar en blanco");

                RuleFor(p => p.FechaPublicacion)
                    .NotEmpty().WithMessage("{Apellido} no puede estar en blanco")
                    .NotNull().WithMessage("{Apellido} no puede estar en blanco");

            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly ContextoLibreria _contexto;

            public Manejador(ContextoLibreria contexto)
            {
                _contexto = contexto;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var libro = new LibreriaMaterial
                {
                    Titulo = request.Titulo,
                    FechaPublicacion = request.FechaPublicacion,
                    AutorLibro = request.AutorLibro
                };

                _contexto.LibreriaMaterial.Add(libro);
                var valor = await _contexto.SaveChangesAsync(); //# transacciones que se realizaron

                if (valor > 0) return Unit.Value;

                throw new Exception("No se logro insertar el registro");

            }
        }
    }
}
