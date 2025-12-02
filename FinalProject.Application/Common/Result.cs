namespace FinalProject.Application.Common;

/// <summary>
/// Clase genérica para encapsular resultados de operaciones
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool Success => IsSuccess;
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? Error => ErrorMessage;
    public int? StatusCode { get; private set; }

    private Result(bool isSuccess, T? data, string? error, int? statusCode)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = error;
        StatusCode = statusCode;
    }

    public static Result<T> Ok(T data) => new(true, data, null, 200);
    public static Result<T> Created(T data) => new(true, data, null, 201);
    public static Result<T> Fail(string error, int statusCode = 400) => new(false, default, error, statusCode);
    public static Result<T> Failure(string error, int statusCode = 400) => new(false, default, error, statusCode);
    public static Result<T> NotFound(string error = "Resource not found") => new(false, default, error, 404);
    public static Result<T> Unauthorized(string error = "Unauthorized") => new(false, default, error, 401);
}

/// <summary>
/// Resultado sin datos
/// </summary>
public class Result
{
    public bool Success { get; private set; }
    public string? Error { get; private set; }
    public int? StatusCode { get; private set; }

    private Result(bool success, string? error, int? statusCode)
    {
        Success = success;
        Error = error;
        StatusCode = statusCode;
    }

    public static Result Ok() => new(true, null, 200);
    public static Result Fail(string error, int statusCode = 400) => new(false, error, statusCode);
    public static Result NotFound(string error = "Resource not found") => new(false, error, 404);
}

/// <summary>
/// Resultado paginado
/// </summary>
public class PaginatedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public PaginationInfo Pagination { get; set; } = new();
}

public class PaginationInfo
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
