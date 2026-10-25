using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDistribution.Application.Common.Exceptions;

public sealed class ConflictException(string message) : Exception(message);