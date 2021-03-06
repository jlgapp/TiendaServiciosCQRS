using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TiendaServicios.Api.Autor.Aplicacion;
using TiendaServicios.Api.Autor.Persistencia;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ContextoAutor>(
    options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionPg"))
);

builder.Services.AddMediatR(typeof(Nuevo.Manejador).Assembly); // solo una vez al iniciar el proyecto busca todas las clases heredadas que tengan el Irequest del mediatr
builder.Services.AddAutoMapper(typeof(Consulta.Manejador)); // no es necesario par futuras clases

builder.Services.AddControllers()
                .AddFluentValidation(cfg=> cfg.RegisterValidatorsFromAssemblyContaining<Nuevo>()); // busca dentro del pry todas las clases que heredan del abstract y listo no necesitas colcar para cada clase esta linea


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseAuthorization();

app.MapControllers();

app.Run();
