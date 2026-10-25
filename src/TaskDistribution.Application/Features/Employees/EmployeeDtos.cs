namespace TaskDistribution.Application.Features.Employees;

public sealed record EmployeeDto(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    bool IsActive,
    int RoleId,
    string RoleName,
    DateTime CreatedAt);

public sealed record CreateEmployeeRequest(string FirstName, string LastName, int RoleId);

public sealed record UpdateEmployeeRequest(string FirstName, string LastName, int RoleId, bool IsActive);