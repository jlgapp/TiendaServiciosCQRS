using FluentValidation;
using MediatR;
using TiendaServicios.Api.Autor.Modelo;
using TiendaServicios.Api.Autor.Persistencia;

namespace TiendaServicios.Api.Autor.Aplicacion
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            public string Nombre { get; set; } = String.Empty;
            public string Apellido { get; set; } = String.Empty;
            public DateTime? FechaNacimiento { get; set; } = null;

        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(p => p.Nombre)
                    .NotEmpty().WithMessage("{Nombre} no puede estar en blanco")
                    .NotNull().WithMessage("{Nombre} no puede estar en blanco")
                    .MaximumLength(50).WithMessage("{Nombre} no puede exceder 50 caracteres");

                RuleFor(p => p.Apellido)
                    .NotEmpty().WithMessage("{Apellido} no puede estar en blanco")
                    .NotNull().WithMessage("{Apellido} no puede estar en blanco");

            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly ContextoAutor _contexto;

            public Manejador(ContextoAutor contexto)
            {
                _contexto = contexto;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var autorLibro = new AutorLibro
                {
                    Nombre = request.Nombre,
                    Apellido = request.Apellido,
                    FechaNacimiento = request.FechaNacimiento,
                    AutorLibroGuid = Guid.NewGuid().ToString(),
                };

                _contexto.AutorLibros.Add(autorLibro);
                var valor = await _contexto.SaveChangesAsync(); //# transacciones que se realizaron
                
                if (valor > 0) return Unit.Value;

                throw new Exception("No se logro insertar el registro");

            }
        }
    }
}
