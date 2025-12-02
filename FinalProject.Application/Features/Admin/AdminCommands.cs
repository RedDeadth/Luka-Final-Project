using FinalProject.Application.Common;

namespace FinalProject.Application.Features.Admin;

public record ApproveCompanyCommand(int CompanyId, bool Approved, string? Reason) : ICommand<Result<bool>>;
public record EmitLukasCommand(int CompanyId, decimal Amount, string Reason) : ICommand<Result<bool>>;
