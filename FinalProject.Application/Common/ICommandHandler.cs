using MediatR;

namespace FinalProject.Application.Common;

/// <summary>
/// Interfaz base para Command Handlers
/// </summary>
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}

/// <summary>
/// Interfaz base para Command Handlers sin respuesta
/// </summary>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Unit>
    where TCommand : ICommand
{
}
