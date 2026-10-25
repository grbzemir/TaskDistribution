using System;
using System.Collections.Generic;
using System.Text;

using TaskDistribution.Domain.Common;

namespace TaskDistribution.Domain.Entities;

public class AssignmentLog : BaseEntity
{
    public int WorkItemId { get; set; }

    public WorkItem WorkItem { get; set; } = null!;

    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public int Difficulty { get; set; }

    public string AppliedRules { get; set; } = string.Empty;

    public bool IsRuleRelaxed { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime AssignedAt { get; set; }
}