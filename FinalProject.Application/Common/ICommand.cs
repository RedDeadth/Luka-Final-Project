using MediatR;

namespace FinalProject.Application.Common;

/// <summary>
/// Interfaz base para Commands (operaciones de escritura)
/// </summary>
public interface ICommand<TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Interfaz base para Commands sin respuesta
/// </summary>
public interface ICommand : IRequest<Unit>
{
}
