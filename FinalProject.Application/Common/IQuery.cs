using MediatR;

namespace FinalProject.Application.Common;

/// <summary>
/// Interfaz base para Queries (operaciones de lectura)
/// </summary>
public interface IQuery<TResponse> : IRequest<TResponse>
{
}
