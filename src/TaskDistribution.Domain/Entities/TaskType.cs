using System;
using System.Collections.Generic;
using System.Text;

using TaskDistribution.Domain.Common;

namespace TaskDistribution.Domain.Entities;

public class TaskType : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Difficulty { get; set; }

    public ICollection<WorkItem> WorkItems { get; set; } = new List<WorkItem>();
}