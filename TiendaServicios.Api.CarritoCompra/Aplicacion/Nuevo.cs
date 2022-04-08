using FluentValidation;
using MediatR;
using TiendaServicios.Api.CarritoCompra.Modelo;
using TiendaServicios.Api.CarritoCompra.Persistencia;

namespace TiendaServicios.Api.CarritoCompra.Aplicacion
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            public DateTime? FechaCreacionSesion { get; set; } = null;
            public List<string> ProductoLista { get; set; }

        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(p => p.FechaCreacionSesion)
                    .NotEmpty().WithMessage("{FechaCreacionSesion} no puede estar en blanco")
                    .NotNull().WithMessage("{FechaCreacionSesion} no puede estar en blanco");

                RuleFor(p => p.ProductoLista)
                    .NotEmpty().WithMessage("{ProductoLista} no puede estar en blanco")
                    .NotNull().WithMessage("{ProductoLista} no puede estar en blanco");

            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CarritoContexto _contexto;

            public Manejador(CarritoContexto contexto)
            {
                _contexto = contexto;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var carritoSession = new CarritoSesion()
                {
                    FechaCreacion = request.FechaCreacionSesion
                };

                _contexto.CarritoSesion.Add(carritoSession);
                var value = await _contexto.SaveChangesAsync();
                 
                if (value == 0) throw new Exception("No se logro insertar el registro");

                foreach (var item in request.ProductoLista)
                {
                    var carritoDetalle = new CarritoSesionDetalle
                    {
                        CarritoSesionId = carritoSession.CarritoSesionId,
                        FechaCreacion = DateTime.Now,
                        ProductoSeleccionado = item
                    };
                    _contexto.CarritoSesionDetalle.Add(carritoDetalle);

                };
                value = await _contexto.SaveChangesAsync();

                if (value > 0) return Unit.Value;

                throw new Exception("No se logro insertar el registro");

            }
        }
    }
}
