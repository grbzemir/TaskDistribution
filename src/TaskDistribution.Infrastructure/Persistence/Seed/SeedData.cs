using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Infrastructure.Persistence.Seed;

internal static class SeedData
{
    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private const int DeveloperRoleId = 1;
    private const int AnalystRoleId = 2;
    private const int ManagerRoleId = 3;

    internal static Role[] Roles =>
    [
        new() { Id = DeveloperRoleId, Name = RoleNames.Developer, CreatedAt = SeedDate },
        new() { Id = AnalystRoleId, Name = RoleNames.Analyst, CreatedAt = SeedDate },
        new() { Id = ManagerRoleId, Name = RoleNames.Manager, CreatedAt = SeedDate }
    ];

    internal static Employee[] Employees =>
    [
        CreateEmployee(1, "Emircan", "Gürbüz", DeveloperRoleId),
        CreateEmployee(2, "Ayşe", "Kaya", DeveloperRoleId),
        CreateEmployee(3, "Yalım", "Gürbüz", DeveloperRoleId),
        CreateEmployee(4, "Elif", "Şahin", DeveloperRoleId),
        CreateEmployee(5, "Can", "Çelik", DeveloperRoleId),
        CreateEmployee(6, "Zeynep", "Arslan", DeveloperRoleId),
        CreateEmployee(7, "Burak", "Koç", DeveloperRoleId),
        CreateEmployee(8, "Selin", "Aydın", DeveloperRoleId),
        CreateEmployee(9, "Emre", "Öztürk", AnalystRoleId),
        CreateEmployee(10, "Deniz", "Yıldız", AnalystRoleId),
        CreateEmployee(11, "Murat", "Aksoy", ManagerRoleId)
    ];

    internal static TaskType[] TaskTypes =>
    [
        CreateTaskType(1, "UI Text Update", "Changing labels, messages or static texts on existing screens.", 1),
        CreateTaskType(2, "Report Filter Addition", "Adding a new filter or column to an existing report.", 2),
        CreateTaskType(3, "Minor Bug Fix", "Fixing an isolated defect with a known root cause.", 3),
        CreateTaskType(4, "New Report Development", "Designing and developing a new report from scratch.", 4),
        CreateTaskType(5, "New Screen Development", "Building a new form or screen with its business logic.", 5),
        CreateTaskType(6, "Database Schema Change", "Changing tables or relations with data migration.", 6),
        CreateTaskType(7, "Third-Party Integration", "Integrating an external system such as a carrier API.", 7),
        CreateTaskType(8, "New Module Development", "Designing and developing a complete new module.", 8)
    ];

    private static Employee CreateEmployee(int id, string firstName, string lastName, int roleId) => new()
    {
        Id = id,
        FirstName = firstName,
        LastName = lastName,
        RoleId = roleId,
        IsActive = true,
        CreatedAt = SeedDate
    };

    private static TaskType CreateTaskType(int id, string name, string description, int difficulty) => new()
    {
        Id = id,
        Name = name,
        Description = description,
        Difficulty = difficulty,
        CreatedAt = SeedDate
    };
}