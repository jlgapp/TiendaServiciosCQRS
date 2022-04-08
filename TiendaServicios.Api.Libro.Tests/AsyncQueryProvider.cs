using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TiendaServicios.Api.Libro.Tests
{
    // esta clase toma los parametros genericos de entrada para soportar filtros a una entidad de EF
    public class AsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _innerQueryProvider;

        public AsyncQueryProvider(IQueryProvider innerQueryProvider)
        {
            _innerQueryProvider = innerQueryProvider;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new AsyncEnumerable<T>(expression);

        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new AsyncEnumerable<TElement>(expression);
        }

        public object? Execute(Expression expression)
        {
            return _innerQueryProvider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _innerQueryProvider.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var resultadoTipo = typeof(TResult).GetGenericArguments()[0];
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var ejecucionResultado = typeof(IQueryProvider)
                                        .GetMethod(
                                                    name: nameof(IQueryProvider.Execute),
                                                    genericParameterCount: 1,
                                                    types: new[] { typeof(Expression) }
                                        )
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                                        .MakeGenericMethod(resultadoTipo)
                                        .Invoke(this, new[] {expression});

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))?
                    .MakeGenericMethod(resultadoTipo).Invoke(null, new[] { ejecucionResultado });
        }
    }
}
