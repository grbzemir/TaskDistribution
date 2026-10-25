using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDistribution.Application.Common.Exceptions;

public sealed class NotFoundException(string entityName, object key)
    : Exception($"{entityName} with id '{key}' was not found.");