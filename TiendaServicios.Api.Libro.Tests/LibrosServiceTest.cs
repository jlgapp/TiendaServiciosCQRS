using AutoMapper;
using GenFu; // genera data casi real para pruebas
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TiendaServicios.Api.Autor.Aplicacion;
using TiendaServicios.Api.Libro.Aplicacion;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;
using Xunit;

namespace TiendaServicios.Api.Libro.Tests
{
    public class LibrosServiceTest
    {

        private IEnumerable<LibreriaMaterial> ObetnerDataPrueba()
        {
            A.Configure<LibreriaMaterial>()
                .Fill(x => x.Titulo).AsArticleTitle()
                .Fill(x => x.LibreriaMaterialId, ()=> { return Guid.NewGuid(); })
                ;
            var lista = A.ListOf<LibreriaMaterial>(30);
            lista[0].LibreriaMaterialId = Guid.Empty;

            return lista;
        }

        private Mock<ContextoLibreria> CrearContexto()
        {
            // simulamos data aqui
            var dataPrueba = ObetnerDataPrueba().AsQueryable();


            var dbset = new Mock<DbSet<LibreriaMaterial>>();
            /// esto es lo que tiene una clase de tipo entidad EF para que sea reconocida como tal
            dbset.As<IQueryable<LibreriaMaterial>>().Setup( x=> x.Provider).Returns(dataPrueba.Provider);
            dbset.As<IQueryable<LibreriaMaterial>>().Setup(x => x.Expression).Returns(dataPrueba.Expression);
            dbset.As<IQueryable<LibreriaMaterial>>().Setup(x => x.ElementType).Returns(dataPrueba.ElementType);
            dbset.As<IQueryable<LibreriaMaterial>>().Setup(x => x.GetEnumerator()).Returns(dataPrueba.GetEnumerator());
            // usamos las dos enumerator y enumrable para que evlaues como asybncrono
            dbset.As<IAsyncEnumerable<LibreriaMaterial>>()
                .Setup(x => x.GetAsyncEnumerator(new System.Threading.CancellationToken()))
                .Returns(new AsyncEnumerator<LibreriaMaterial>(dataPrueba.GetEnumerator()));

            // con esto ya puedo hacer filtros
            dbset.As<IQueryable<LibreriaMaterial>>()
                .Setup(x => x.Provider).Returns(new AsyncQueryProvider<LibreriaMaterial>(dataPrueba.Provider));

            var contexto = new Mock<ContextoLibreria>();
            contexto.Setup(x => x.LibreriaMaterial).Returns(dbset.Object);

            return contexto;
        }
        [Fact]
        public async void GetLibroPorId()
        {
            var mockContexto = CrearContexto();
            var mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingTest());
            });

            var mapper = mapConfig.CreateMapper();

            var request = new ConsultaFiltro.LibroUnico();
            request.LibroId = Guid.Empty;

            var manejador = new ConsultaFiltro.Manejador(mockContexto.Object, mapper);
            var libro = await manejador.Handle(request, new System.Threading.CancellationToken());

            Assert.NotNull(libro);
            Assert.True(libro.LibreriaMaterialId == Guid.Empty);
        }

        [Fact]
        public async void GetLibros()
        {
            //System.Diagnostics.Debugger.Launch();

            ///1. emular a la instancia de EF core _contexto, no una instancia es emular la funcionalidad
            /// para emular las acciones dentro de un objeto unitest usamos objetos de tipo mock
            /// mock es una representacion de in objeto que solo puede ser controlado, representa cualquier objeto del codigo
            /// 
            var mockContexto = CrearContexto();
            ///2. emular al mapping IMApers dentro de la clase manejador de consulta
            ///
            var mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingTest());
            });

            var mapper = mapConfig.CreateMapper();

            ///3. instancia a la clase manejador y pasarle como parametros los mocks creados
            ///
            Consulta.Manejador manejador = new Consulta.Manejador(mockContexto.Object, mapper);
            Consulta.Ejecuta request = new Consulta.Ejecuta();
            var lista = await manejador.Handle(request, new System.Threading.CancellationToken());
            // con any devuelve true o false si hay algun valor devuelve true
            Assert.True(lista.Any());


        }

        [Fact]
        public async void GuardarLibro()
        {
            // creamos una base en memoria para no usar la base principal
            // cambio detexto para azure
            var options = new DbContextOptionsBuilder<ContextoLibreria>()
                            .UseInMemoryDatabase(databaseName: "BaseDatosLibro")
                            .Options;

            var contexto = new ContextoLibreria(options);

            var request = new Nuevo.Ejecuta();
            request.Titulo = "testing Titulo";
            request.AutorLibro = Guid.Empty;
            request.FechaPublicacion = DateTime.Now;

            var manejador = new Nuevo.Manejador(contexto);

            var responde = await manejador.Handle(request, new System.Threading.CancellationToken());
            Assert.True(responde != null);
        }
    }
}
