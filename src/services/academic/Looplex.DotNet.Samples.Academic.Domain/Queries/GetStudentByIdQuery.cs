﻿using System;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

namespace Looplex.DotNet.Samples.Academic.Domain.Queries
{
    public class GetStudentByIdQuery : IQuery<Student>
    {
        public required Guid UniqueId { get; init; }
    }
}
