using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Application.Features.Employees;

internal static class EmployeeMappings
{
    internal static EmployeeDto ToDto(this Employee employee) => new(
        employee.Id,
        employee.FirstName,
        employee.LastName,
        employee.FullName,
        employee.IsActive,
        employee.RoleId,
        employee.Role?.Name ?? string.Empty,
        employee.CreatedAt);
}