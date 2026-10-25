using System;
using System.Collections.Generic;
using System.Text;

using TaskDistribution.Domain.Common;

namespace TaskDistribution.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
