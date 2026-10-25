using FluentValidation;
using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Application.Common.Exceptions;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Application.Features.Employees;

public sealed class EmployeeService(
    IEmployeeRepository employeeRepository,
    IWorkItemRepository workItemRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateEmployeeRequest> createValidator,
    IValidator<UpdateEmployeeRequest> updateValidator) : IEmployeeService
{
    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.GetAllWithRoleAsync(cancellationToken);
        return employees.Select(e => e.ToDto()).ToList();
    }

    public async Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdWithRoleAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        return employee.ToDto();
    }

    public async Task<EmployeeDto> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        await createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var employee = new Employee
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            RoleId = request.RoleId,
            IsActive = true
        };

        await employeeRepository.AddAsync(employee, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(employee.Id, cancellationToken);
    }

    public async Task<EmployeeDto> UpdateAsync(
        int id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        await updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var employee = await employeeRepository.GetByIdWithRoleAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.RoleId = request.RoleId;
        employee.IsActive = request.IsActive;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        var hasWorkItems = await workItemRepository.AnyAsync(
            w => w.AssignedEmployeeId == id,
            cancellationToken);

        if (hasWorkItems)
        {
            throw new ConflictException(
                "This employee has assigned work items and cannot be deleted. Deactivate the employee instead.");
        }

        employeeRepository.Remove(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}