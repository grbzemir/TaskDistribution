using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDistribution.Domain.Enums;

public enum WorkItemStatus
{
    New = 1,
    Analyzed = 2,
    Assigned = 3,
    Completed = 4
}
