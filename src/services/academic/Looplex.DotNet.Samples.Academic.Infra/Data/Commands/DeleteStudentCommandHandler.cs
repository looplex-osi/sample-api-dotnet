﻿using Looplex.DotNet.Core.Application.Abstractions.Commands;
using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.Commands
{
    public class DeleteStudentCommandHandler : ICommandHandler<DeleteStudentCommand>
    {
        private readonly IDatabaseContext _context;

        public DeleteStudentCommandHandler(IDatabaseContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = "DELETE FROM students WHERE uuid = @UniqueId";

            using var connection = _context.CreateConnection();
            
            var count = await connection.ExecuteAsync(query, new { request.UniqueId });

            if (count == 0) throw new EntityNotFoundException(nameof(Student), request.UniqueId.ToString());
        }
    }
}