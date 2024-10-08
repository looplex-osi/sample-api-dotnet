﻿using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

namespace Looplex.DotNet.Samples.Academic.Domain.Commands
{
    public class UpdateStudentCommand : ICommand
    {
        public required Student Student { get; init; }
    }
}
