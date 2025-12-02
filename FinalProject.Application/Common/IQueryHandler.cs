using MediatR;

namespace FinalProject.Application.Common;

/// <summary>
/// Interfaz base para Query Handlers
/// </summary>
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}
