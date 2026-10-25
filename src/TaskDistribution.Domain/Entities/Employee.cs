using System;
using System.Collections.Generic;
using System.Text;

using TaskDistribution.Domain.Common;

namespace TaskDistribution.Domain.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public ICollection<WorkItem> AssignedWorkItems { get; set; } = new List<WorkItem>();

    public string FullName => $"{FirstName} {LastName}";
}